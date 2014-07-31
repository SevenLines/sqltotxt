using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using CommandLine;
using CommandLine.Text;

namespace sqltotxt
{
    class Options
    {
        [Option('i', "input", Required = true, HelpText = "folder with dir structure and sql scripts")]
        public string InputDir { get; set; }
        [Option('o', "output", HelpText = "folder to which output goes")]
        public string OutputDir { get; set; }

        [Option('a', "append", HelpText = "append output to existing files in output directory",
            DefaultValue = false)]
        public bool Append { get; set; }

        [OptionList('p', "params",
            HelpText = "pass parameters as list of tuples: key1=value1 key2=value2 ...")]
        public List<string> Parameters { get; set; }

        [Option('s', "show parameters", HelpText = "show available parameters")]
        public string ListParameters { get; set; }

        [Option('c', "connection options", HelpText = "file with connection options:\n"
            + "UserID=YourUsername\n"
            + "Password=YourPassword\n"
            + "DataSource=YourDataSourceName\n"
            + "InitialCatalog=YourInitialCatalogName\n")]
        public string CredentialFile { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
                (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
