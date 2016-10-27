using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace makecrx
{
    public static class ZipUtil
    {
        public static void Zip(DirectoryInfo sourceDirectoryInfo, string zipFileName)
        {
            // always create new file.
            if (File.Exists(zipFileName))
            {
                File.Delete(zipFileName);
            }

            using (var zipFile = new ZipFile(zipFileName))
            {
                zipFile.AddDirectory(sourceDirectoryInfo.FullName);
                zipFile.Save();
            }
        }
    }
}
