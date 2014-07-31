using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using sqltotxt.Properties;

namespace sqltotxt
{
    class SqlToTxtGenerator
    {
        public SqlToTxtGenerator(String inputDirPath, String credentialFile)
        {
            FindScripts(new DirectoryInfo(inputDirPath));
            var connectionDict = new Dictionary<string, string>();
            if (!String.IsNullOrEmpty(credentialFile)) { 
                foreach (var line in File.ReadLines(credentialFile))
                {
                    var tuple = ParseOptionLine(line);
                    if (!tuple.HasValue) 
                        continue;
                    connectionDict.Add(tuple.Value.Key, tuple.Value.Value);
                }
            }
                

            var connString = new SqlConnectionStringBuilder
            {
                UserID = connectionDict.Default("UserID", "sa"), 
                DataSource = connectionDict.Default("DataSource", "."), 
                InitialCatalog = connectionDict.Default("InitialCatalog", "RoadsDB_DIRECT"), 
                Password = connectionDict.Default("Password", ""),
            };
            connection.ConnectionString = connString.ToString();
        }

        public KeyValuePair<string, string>? ParseOptionLine(string line)
        {
            var regex = new Regex(@"(\w+):\s*(\w+)");
            var match = regex.Match(line);

            if(!match.Success) return null;

            return new KeyValuePair<string, string>(match.Groups[1].Value, match.Groups[2].Value);
        }

        List<Script> scripts = new List<Script>();
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        SqlConnection connection = new SqlConnection();

        List<Script> FindScripts(DirectoryInfo dir)
        {
            scripts.Clear();
            parameters.Clear();

            foreach (var f in dir.GetFiles())
            {
                var s = new Script(f);
                scripts.Add(s);
            }

            foreach (var s in scripts)
            {
                foreach (var p in s.Params)
                {
                    if (!parameters.ContainsKey(p.Key))
                    {
                        parameters.Add(p.Key, p.Value);
                    }
                }
            }

            return scripts;
        }

        public string ShowParameters
        {
            get { return String.Join("\n", 
                parameters.Select(x => String.Format("{0}={1}", x.Key, x.Value)).ToArray()); }
        }



        void Connect()
        {
            Console.WriteLine(Resources.Connecting);
            connection.Open();
        }

        void Disconnect()
        {
            Console.WriteLine(Resources.Disconnecting);
            connection.Close();
        }

        public bool Generate(string outputDir, bool append)
        {
            if (String.IsNullOrEmpty(outputDir))
            {
                Console.WriteLine(Resources.Success);
                return false;
            }
                
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            Connect();
            var result = new StringBuilder();

            Console.WriteLine(Resources.ScriptsCountInfo, scripts.Count);
            for (var j=0; j<scripts.Count;++j)
            {
                var s = scripts[j];
                result.Clear();
                String path = Path.Combine(outputDir, s.Title.Replace(".sql", ".txt"));
                String sqlText = s.Text(parameters);
                if (String.IsNullOrEmpty(sqlText))
                    continue;

                Console.WriteLine(Resources.ScriptExecutionBegin, j, scripts.Count, s.Title);

                var sql = new SqlCommand(sqlText, connection);
                using (var reader = sql.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.AppendLine(
                            String.Join("\t",
                                Enumerable.Range(0, reader.FieldCount).Select(i => reader.GetValue(i)))
                            );
                    }
                }
                if (append)
                {
                    File.AppendAllText(path, result.ToString(), Encoding.Default);
                }
                else
                {
                    File.WriteAllText(path, result.ToString(), Encoding.Default);
                }
            }
            
            Disconnect();
            Console.WriteLine(Resources.Success);
            return true;
        }
    }
}
