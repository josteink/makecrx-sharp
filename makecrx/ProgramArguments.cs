using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace makecrx
{
    public class ProgramArguments
    {
        [Argument(ArgumentType.Required, HelpText = "The directory containing the package source.", ShortName = "s", LongName = "source")]
        public string SourceDir;

        [Argument(ArgumentType.AtMostOnce, HelpText = "The path to the private key used to sign the package. If omitted, this file will be generated with a default name.", ShortName = "k", LongName = "key")]
        public string KeyFile;

        [Argument(ArgumentType.AtMostOnce, HelpText = "The directory for the resulting package and files. If omitted, parent directory of source will be used.", ShortName = "t", LongName = "target", DefaultValue = null)]
        public string TargetDir;

        [Argument(ArgumentType.AtMostOnce, HelpText = "The name of the package (without extension). If omitted, this file will be generated with a default name.", ShortName = "p", LongName = "package", DefaultValue = null)]
        public string PackageName; 
    }
}
