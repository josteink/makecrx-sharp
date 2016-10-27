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
                Console.WriteLine("Sanitizing parameters...");

                // apply default values
                if (string.IsNullOrWhiteSpace(pa.PackageName))
                {
                    pa.PackageName = sourceDirInfo.Parent.Name;
                    Console.WriteLine("- No package-name provided. Applying default.");
                }

                if (string.IsNullOrWhiteSpace(pa.TargetDir))
                {
                    pa.TargetDir = sourceDirInfo.Parent.FullName;
                    Console.WriteLine("- No target-directory provided. Applying default.");
                }
                var targetDirInfo = new DirectoryInfo(pa.TargetDir);

                if (string.IsNullOrWhiteSpace(pa.KeyFile))
                {
                    pa.KeyFile = GetDefaultFileNameFor(targetDirInfo, pa.PackageName, "pem");
                    Console.WriteLine("- No key-parameter provided. Applying default.");
                }

                // where do we zip it?
                var zipFile = GetDefaultFileNameFor(targetDirInfo, pa.PackageName, "zip");

                var packageFile = GetDefaultFileNameFor(targetDirInfo, pa.PackageName, "crx");
                var packageFileInfo = new FileInfo(packageFile);

                // output configuration
                Console.WriteLine("");
                Console.WriteLine("Configuration:");
                Console.WriteLine("- Package-name:     {0}", pa.PackageName);
                Console.WriteLine("- Source-directory: {0}", sourceDirInfo.FullName);
                Console.WriteLine("- Target-directory: {0}", targetDirInfo.FullName);
                Console.WriteLine("- Key-file:         {0}", pa.KeyFile);
                Console.WriteLine("- Zip-file:         {0}", zipFile);
                Console.WriteLine("- CRX package-file: {0}", packageFileInfo.FullName);
                

                // package it up!
                Console.WriteLine("");
                Console.WriteLine("Processing:");

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

                Console.Write("- Zipping package-contents...");
                ZipUtil.Zip(sourceDirInfo, zipFile);
                Console.WriteLine(" Done!");

                Console.Write("- Creating package '{0}'...", packageFileInfo.Name);
                var packager = new CrxPackager();
                packager.Package(zipFile, rsaUtil, packageFileInfo.FullName);
                Console.WriteLine(" Done!");

                Console.WriteLine("");
                Console.WriteLine("Package-file '{0}' created succesfully!", packageFileInfo.Name);
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

        private static string GetDefaultFileNameFor(DirectoryInfo targetDirInfo, string packageName, string extension)
        {
            return String.Format("{0}\\{1}.{2}", targetDirInfo.FullName, packageName, extension);
        }
    }
}
