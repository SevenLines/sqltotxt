using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


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
                if (!String.IsNullOrEmpty(options.ListParameters))
                {
                    Console.WriteLine(generator.ShowParameters);
                    return;
                }
                generator.Generate(options.OutputDir, options.Append);
            }

            /* var parser = new ArgumentParser("sqltotxt", Properties.Resources.Description);

            var outputDirArg = new StringArgument("output", "path to output dir", "");
            var inputDirArg = new StringArgument("input dir", "directory with sql files", "");

            parser.Add("-", "output", outputDirArg);
            parser.Add("-", "input", inputDirArg);


            try
            {
                parser.Parse(args);

                if (parser.HelpMode)
                {
                    Console.WriteLine(parser.UsageText);
                    return;
                }

                if (outputDirArg.Defined && inputDirArg.Defined)
                {
                    Console.WriteLine(outputDirArg.Value);
                    Console.WriteLine(inputDirArg.Value);
                }
                else
                {
                    throw new Exception("output and input dir should be defined");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }*/
        }
    }
}
