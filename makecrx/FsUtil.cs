using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace makecrx
{
    public static class FsUtil
    {
        public static void EnsureExists(DirectoryInfo directoryInfo)
        {
            if (directoryInfo.Exists)
            {
                return;
            }

            // we need to create ourselves... but before that we need to ensure our parents exist.
            EnsureExists(directoryInfo.Parent);
            directoryInfo.Create();
        }
    }
}
