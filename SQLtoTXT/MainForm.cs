﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;


namespace SQLtoTXT
{
    public partial class MainForm : Form
    {
        List<Script> scripts = new List<Script>();
        Dictionary<String, String> prms = new Dictionary<string, string>();
        SqlConnection connection = new SqlConnection();

        public MainForm()
        {
            InitializeComponent();
        }

        void Connect()
        {
            var connString = new SqlConnectionStringBuilder();
            connString.UserID = "sa";
            connString.DataSource = ".";
            connString.InitialCatalog = "RoadsDB_DIRECT";
            connection.ConnectionString = connString.ToString();

            connection.Open();
        }

        void Disconnect()
        {
            connection.Close();
        }

        List<Script> FindScripts(String path)
        {
            scripts.Clear();
            prms.Clear();

            var dir = new DirectoryInfo(path);
            foreach (var f in dir.GetFiles()) {
                Script s = new Script(f);
                scripts.Add(s);
            }

            foreach (var s in scripts)
            {
                foreach (var p in s.Params)
                {
                    if (!prms.ContainsKey(p.Key))
                    {
                        prms.Add(p.Key, p.Value);
                    }
                }
            }

            cmbKeys.DataSource = new BindingSource(prms, null);
            cmbKeys.DisplayMember = "Key";

            return scripts;
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            String saveDirPath = Path.Combine(txtDir.Text, "out");
            var saveDir = Directory.CreateDirectory(saveDirPath);
            
            Connect();
            StringBuilder result = new StringBuilder();
            foreach (var s in scripts)
            {
                result.Clear();
                String path = Path.Combine(saveDirPath, s.Title);
                String sqlText = s.Text(prms);
                SqlCommand sql = new SqlCommand(sqlText, connection);
                using(var reader = sql.ExecuteReader()) {
                    while (reader.Read())
                    {
                        result.AppendLine(
                            String.Join("\t", 
                                Enumerable.Range(0, reader.FieldCount).Select(i => reader.GetValue(i)))
                            );
                    }
                }
                File.WriteAllText(path, result.ToString(), Encoding.Default);
            }
            Disconnect();

        }

        private void btnSelectDir_Click(object sender, EventArgs e)
        {
            FindScripts(txtDir.Text);
        }

        private KeyValuePair<String, String> currentParam()
        {
            if (cmbKeys.SelectedValue != null)
            {
                return (KeyValuePair<String, String>)cmbKeys.SelectedValue;
            }
            else
            {
                return new KeyValuePair<String, String>();
            }
        } 

        private void cmbKeys_SelectedIndexChanged(object sender, EventArgs e)
        {
            var v = currentParam();
            txtValue.Text = v.Value;
        }

        private void txtValue_TextChanged(object sender, EventArgs e)
        {
            var v = currentParam();
            if (v.Key == null)
                return;
            prms[v.Key] = txtValue.Text;
        }
    }
}
