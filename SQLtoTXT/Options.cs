using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
        public List<string> ParametersArg { get; set; }

        [Option('s', "show parameters", HelpText = "show available parameters")]
        public bool ListParameters { get; set; }

        [Option('c', "connection options", HelpText = "file with connection options:"
            + "\n\tUserID=YourUsername"
            + "\n\tPassword=YourPass"
            + "\n\tDataSource=YourDataSourceName"
            + "\n\tInitialCatalog=YourInitialCatalogName")]
        public string CredentialFile { get; set; }

        public Dictionary<string, string> Parameters
        {
            get
            {
                var reg = new Regex(@"(\w+)=(\w+)");
                var output = new Dictionary<string, string>();
                foreach (var item in ParametersArg)
                {
                    var m = reg.Match(item);
                    if (m.Success)
                    {
                        output.Add(m.Groups[1].Value, m.Groups[2].Value);
                    }
                }
                return output;
            }
        }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
                (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
