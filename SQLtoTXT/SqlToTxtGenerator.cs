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

            int timeout;
            int.TryParse(connectionDict.Default("UserID", "15"), out timeout);
            var connString = new SqlConnectionStringBuilder
            {
                ApplicationName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name,
                ConnectTimeout = timeout,
                UserID = connectionDict.Default("UserID", ""), 
                DataSource = connectionDict.Default("DataSource", ""), 
                InitialCatalog = connectionDict.Default("InitialCatalog", ""), 
                Password = connectionDict.Default("Password", ""),
                
            };
            connection.ConnectionString = connString.ToString();
        }

        public KeyValuePair<string, string>? ParseOptionLine(string line)
        {
            var regex = new Regex(@"(\w+):\s*(\w+|[\(.)/]+)");
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



        bool Connect()
        {
            Console.WriteLine(Resources.Connecting);
            try
            {
                connection.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

        bool Disconnect()
        {
            Console.WriteLine(Resources.Disconnecting);
            try
            {
                connection.Close();
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

        public bool Generate(string outputDir, bool append)
        {
            if (String.IsNullOrEmpty(outputDir))
            {
                Console.WriteLine(Resources.Failed);
                return false;
            }
                
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            if (!Connect())
            {
                Console.WriteLine(Resources.Failed);
                return false;
            }

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

                SqlDataReader reader;
                try
                {
                    reader = sql.ExecuteReader();
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(Resources.SkipFile);
                    continue;
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine(ex.Message);
                    break;
                }


                while (reader.Read())
                {
                    result.AppendLine(
                        String.Join("\t",
                            Enumerable.Range(0, reader.FieldCount).Select(i => reader.GetValue(i)))
                        );
                }
                reader.Close();

                try
                {
                    if (append)
                    {
                        File.AppendAllText(path, result.ToString(), Encoding.Default);
                    }
                    else
                    {
                        File.WriteAllText(path, result.ToString(), Encoding.Default);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(Resources.SkipFile);
                }
            }

            Disconnect();
            Console.WriteLine(Resources.Success);
            return true;
        }
    }
}
