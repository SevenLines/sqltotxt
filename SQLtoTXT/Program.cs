using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using sqltotxt.Properties;


namespace sqltotxt
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new Options();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                if (!Directory.Exists(options.InputDir))
                {
                    Console.WriteLine(Resources.DirectoryDoesntExist, options.InputDir);
                    return;
                }
                var generator = new SqlToTxtGenerator(options.InputDir, options.CredentialFile);
                if (options.ListParameters)
                {
                    Console.WriteLine(generator.ParametersInfo);
                    return;
                }
                if (generator.Generate(options.OutputDir, options.Append, options.Parameters))
                {
                    Console.WriteLine(Resources.Closing);        
                }
            }
        }
    }
}
