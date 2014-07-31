using System;
using System.Collections.Generic;
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
                var generator = new SqlToTxtGenerator(options.InputDir, 
                    options.CredentialFile,
                    options.Parameters
                );
                if (options.ListParameters)
                {
                    Console.WriteLine(generator.ParametersInfo);
                    return;
                }
                if (generator.Generate(options.OutputDir, options.Append))
                {
                    Console.WriteLine(Resources.Closing);        
                }
            }
        }
    }
}
