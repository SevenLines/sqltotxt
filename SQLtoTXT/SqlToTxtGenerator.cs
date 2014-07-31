using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace sqltotxt
{
    class SqlToTxtGenerator
    {
        public SqlToTxtGenerator(String inputDirPath, String CredentialFile)
        {
            FindScripts(new DirectoryInfo(inputDirPath));
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
            var connString = new SqlConnectionStringBuilder
            {
                UserID = "sa",
                DataSource = ".",
                InitialCatalog = "RoadsDB_DIRECT"
            };
            connection.ConnectionString = connString.ToString();

            connection.Open();
        }

        void Disconnect()
        {
            connection.Close();
        }

        public bool Generate(string outputDir, bool append)
        {
            if (String.IsNullOrEmpty(outputDir))
                return false;

            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            Connect();
            var result = new StringBuilder();
            foreach (var s in scripts)
            {
                result.Clear();
                String path = Path.Combine(outputDir, s.Title.Replace(".sql", ".txt"));
                String sqlText = s.Text(parameters);
                if (String.IsNullOrEmpty(sqlText))
                    continue;

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
            return true;
        }

    }
}
