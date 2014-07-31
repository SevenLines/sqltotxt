using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                var generator = new SqlToTxtGenerator(options.InputDir, options.CredentialFile);
                if (options.ListParameters)
                {
                    Console.WriteLine(generator.ShowParameters);
                    return;
                }
                generator.Generate(options.OutputDir, options.Append);
            }
            Console.WriteLine(Resources.Closing);
        }
    }
}
