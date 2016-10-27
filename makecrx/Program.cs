using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace makecrx
{
    public class Program
    {
        public static int Main(string[] args)
        {
            Console.WriteLine("makecrx-sharp");
            Console.WriteLine("");

            // parse arguments
            var pa = new ProgramArguments();
            var success = CommandLine.Parser.ParseArgumentsWithUsage(args, pa);
            if (!success)
            {
                return 1;
            }

            // validate
            var sourceDirInfo = new DirectoryInfo(pa.SourceDir);
            if (!sourceDirInfo.Exists)
            {
                Console.WriteLine("ERROR: Source-directory for package '{0}' cannot be found!", pa.SourceDir);
                return 1;
            }

            try
            {

                // apply default values
                if (string.IsNullOrWhiteSpace(pa.TargetPath))
                {
                    pa.TargetPath = GetDefaultFileNameFor(sourceDirInfo, "crx");
                    Console.WriteLine("- No target-parameter provided. Using: '{0}'.", pa.TargetPath);
                }

                if (string.IsNullOrWhiteSpace(pa.KeyFile))
                {
                    pa.KeyFile = GetDefaultFileNameFor(sourceDirInfo, "pem");
                    Console.WriteLine("- No key-parameter provided. Using:    '{0}'.", pa.KeyFile);
                }


                // generate key if missing
                var rsaUtil = new RsaUtil(pa.KeyFile);
                if (!File.Exists(pa.KeyFile))
                {
                    Console.Write("- OpenSSL key-file not found. Generating...");
                    rsaUtil.GenerateKey();
                    Console.WriteLine(" Done!");
                }
                else
                {
                    Console.Write("- OpenSSL key-file found. Loading...");
                    rsaUtil.LoadKey();
                    Console.WriteLine(" Done!");
                }

                var targetPathInfo = new FileInfo(pa.TargetPath);
                Console.WriteLine("- Creating package '{0}'...", targetPathInfo.Name);
                var packager = new CrxPackager();
                packager.Package(pa.SourceDir, rsaUtil, pa.TargetPath);
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("");
                Console.WriteLine("ERROR: Failure trying to create package-file:");
                Console.WriteLine(ex);
                return 1;
            }
        }

        private static string GetDefaultFileNameFor(DirectoryInfo dirInfo, string extension)
        {
            return String.Format("{0}\\{1}.{2}", dirInfo.Parent.FullName, dirInfo.Name, extension);
        }
    }
}
