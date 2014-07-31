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
        List<Script> _scripts = new List<Script>();
        Dictionary<string, string> _parameters = new Dictionary<string, string>();
        SqlConnection _connection = new SqlConnection();

        public SqlToTxtGenerator(string inputDirPath, string credentialFile)
        {
            FindScripts(new DirectoryInfo(inputDirPath));
//            _parameters = parameters;
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
            _connection.ConnectionString = connString.ToString();
        }

        public KeyValuePair<string, string>? ParseOptionLine(string line)
        {
            var regex = new Regex(@"(\w+):\s*(\w+|[\(.)/]+)");
            var match = regex.Match(line);

            if(!match.Success) return null;

            return new KeyValuePair<string, string>(match.Groups[1].Value, match.Groups[2].Value);
        }

 

        public void GetScriptsPaths(String dirPath, List<String> sciptPaths)
        {
            try
            {
                sciptPaths.AddRange(Directory.GetFiles(dirPath, "*.sql"));
                foreach (string d in Directory.GetDirectories(dirPath))
                {
                    GetScriptsPaths(d, sciptPaths);
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }

        public void ShowParameters()
        {
            Console.WriteLine(Resources.SeparatorLine);
            Console.WriteLine(Resources.Parameters);
            Console.WriteLine(ParametersInfo); 
            Console.WriteLine(Resources.SeparatorLine);
        }

        List<Script> FindScripts(DirectoryInfo dir)
        {
            _scripts.Clear();
            _parameters.Clear();

            var scriptPaths = new List<string>();
            GetScriptsPaths(dir.FullName, scriptPaths);

            foreach (var f in scriptPaths)
            {
                var title = f.Replace(dir.FullName, "");
                if (title.StartsWith("\\"))
                {
                    title = title.Substring(1);
                }
                var s = new Script(title, new FileInfo(f));
                _scripts.Add(s);
            }

            foreach (var s in _scripts)
            {
                foreach (var p in s.Params)
                {
                    if (!_parameters.ContainsKey(p.Key))
                    {
                        _parameters.Add(p.Key, p.Value);
                    }
                }
            }

            return _scripts;
        }

        public string ParametersInfo
        {
            get { return String.Join("\n", 
                _parameters.Select(x => String.Format("{0}={1}", x.Key, x.Value)).ToArray()); }
        }



        bool Connect()
        {
            Console.WriteLine(Resources.Connecting);
            try
            {
                _connection.Open();
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
                _connection.Close();
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

        public bool Generate(string outputDir, bool append, Dictionary<string, string> parameters)
        {
            // check output path
            if (String.IsNullOrEmpty(outputDir))
            {
                Console.WriteLine(Resources.OutputDirDoesNotExists);
                Console.WriteLine(Resources.Failed);
                return false;
            }

            // opening connection
            if (!Connect())
            {
                Console.WriteLine(Resources.Failed);
                return false;
            }


            // itterate over script files
            var result = new StringBuilder();
            Console.WriteLine(Resources.ScriptsCountInfo, _scripts.Count);
            ShowParameters();
            for (var j=0; j<_scripts.Count;++j)
            {
                var s = _scripts[j];
                result.Clear();
                
                // get script text
                // binding parameters
                String sqlText = s.Text(parameters);
                if (String.IsNullOrEmpty(sqlText))
                    continue;


                Console.WriteLine(Resources.ScriptExecutionBegin, j, _scripts.Count, s.Title);

                // execute command
                var sql = new SqlCommand(sqlText, _connection);
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

                // itterate over result
                while (reader.Read())
                {
                    result.AppendLine(
                        String.Join("\t",
                            Enumerable.Range(0, reader.FieldCount).Select(i => reader.GetValue(i)))
                        );
                }
                reader.Close();

                // create directories if not exists
                String path = Path.Combine(outputDir, s.Title.Replace(".sql", ".txt"));
                var fi = new FileInfo(path);
                if (fi.DirectoryName != null && !Directory.Exists(fi.DirectoryName))
                {
                    Directory.CreateDirectory(fi.DirectoryName);
                }

                // write result into output file
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
