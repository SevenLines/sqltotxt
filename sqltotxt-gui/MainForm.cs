using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using sqltotxt;


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

        

        String getOutputDir()
        {
            outputFolderDialog.SelectedPath = SQLtoTXT.Properties.Settings.Default.LastDir;

            if (outputFolderDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return null;

            Properties.Settings.Default.LastDir = outputFolderDialog.SelectedPath;
            Properties.Settings.Default.Save();

            return outputFolderDialog.SelectedPath;
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            String saveDirPath = getOutputDir();
            if (String.IsNullOrEmpty(saveDirPath))
                return;

            if (!Directory.Exists(saveDirPath))
            {
                Directory.CreateDirectory(saveDirPath);
            }
            
            Connect();
            StringBuilder result = new StringBuilder();
            progressBar.Maximum = scripts.Count;
            progressBar.Minimum = 0;
            progressBar.Value = 0;
            foreach (var s in scripts)
            {
                result.Clear();
                String path = Path.Combine(saveDirPath, s.Title.Replace(".sql", ".txt"));
                String sqlText = s.Text(prms);
                if (String.IsNullOrEmpty(sqlText))
                    continue;

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
                if (chkAppend.Checked)
                {
                    File.AppendAllText(path, result.ToString(), Encoding.Default);
                } else {
                    File.WriteAllText(path, result.ToString(), Encoding.Default);
                }
                progressBar.PerformStep();
            }
            progressBar.Value = progressBar.Maximum;
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
