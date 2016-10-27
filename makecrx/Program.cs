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
                if (string.IsNullOrWhiteSpace(pa.TargetDir))
                {
                    pa.TargetDir = sourceDirInfo.Parent.FullName;
                    Console.WriteLine("- No target-directory provided. Using: {0}", pa.TargetDir);
                }
                var targetDirInfo = new DirectoryInfo(pa.TargetDir);

                if (string.IsNullOrWhiteSpace(pa.PackageName))
                {
                    pa.PackageName = GetDefaultFileNameFor(sourceDirInfo, targetDirInfo, "crx");
                    Console.WriteLine("- No package-name provided. Using:     '{0}'.", pa.PackageName);
                }

                if (string.IsNullOrWhiteSpace(pa.KeyFile))
                {
                    pa.KeyFile = GetDefaultFileNameFor(sourceDirInfo, targetDirInfo, "pem");
                    Console.WriteLine("- No key-parameter provided. Using:    '{0}'.", pa.KeyFile);
                }

                // ensure file-system objects are in place
                FsUtil.EnsureExists(targetDirInfo);

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

                // package it up!
                var zipFile = GetDefaultFileNameFor(sourceDirInfo, targetDirInfo, "zip");
                Console.Write("- Zipping package-contents...");
                ZipUtil.Zip(sourceDirInfo, zipFile);
                Console.WriteLine(" Done!");

                var packagePathInfo = new FileInfo(pa.PackageName);
                Console.WriteLine("- Creating package '{0}'...", packagePathInfo.Name);
                var packager = new CrxPackager();
                packager.Package(zipFile, rsaUtil, packagePathInfo.FullName);
                Console.WriteLine(" Done!");

                Console.WriteLine("");
                Console.WriteLine("Package-file '{0}' created succesfully!", packagePathInfo.Name);
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

        private static string GetDefaultFileNameFor(DirectoryInfo sourceDirInfo, DirectoryInfo targetDirInfo, string extension)
        {
            return String.Format("{0}\\{1}.{2}", targetDirInfo.FullName, sourceDirInfo.Name, extension);
        }
    }
}
