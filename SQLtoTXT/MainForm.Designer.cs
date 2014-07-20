namespace SQLtoTXT
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnSelectDir = new System.Windows.Forms.Button();
            this.btnGo = new System.Windows.Forms.Button();
            this.txtDir = new System.Windows.Forms.TextBox();
            this.grpParams = new System.Windows.Forms.GroupBox();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.cmbKeys = new System.Windows.Forms.ComboBox();
            this.grpParams.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSelectDir
            // 
            this.btnSelectDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectDir.Location = new System.Drawing.Point(354, 12);
            this.btnSelectDir.Name = "btnSelectDir";
            this.btnSelectDir.Size = new System.Drawing.Size(32, 22);
            this.btnSelectDir.TabIndex = 0;
            this.btnSelectDir.Text = "...";
            this.btnSelectDir.UseVisualStyleBackColor = true;
            this.btnSelectDir.Click += new System.EventHandler(this.btnSelectDir_Click);
            // 
            // btnGo
            // 
            this.btnGo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnGo.Location = new System.Drawing.Point(12, 105);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(75, 28);
            this.btnGo.TabIndex = 1;
            this.btnGo.Text = "GO";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // txtDir
            // 
            this.txtDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDir.Location = new System.Drawing.Point(12, 12);
            this.txtDir.Name = "txtDir";
            this.txtDir.Size = new System.Drawing.Size(336, 22);
            this.txtDir.TabIndex = 2;
            this.txtDir.Text = "d:\\_MMK\\_ROADS\\Скрипты\\";
            // 
            // grpParams
            // 
            this.grpParams.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpParams.Controls.Add(this.txtValue);
            this.grpParams.Controls.Add(this.cmbKeys);
            this.grpParams.Location = new System.Drawing.Point(12, 40);
            this.grpParams.Name = "grpParams";
            this.grpParams.Size = new System.Drawing.Size(374, 55);
            this.grpParams.TabIndex = 3;
            this.grpParams.TabStop = false;
            this.grpParams.Text = "Параметры";
            // 
            // txtValue
            // 
            this.txtValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtValue.Location = new System.Drawing.Point(133, 24);
            this.txtValue.Name = "txtValue";
            this.txtValue.Size = new System.Drawing.Size(235, 22);
            this.txtValue.TabIndex = 1;
            this.txtValue.TextChanged += new System.EventHandler(this.txtValue_TextChanged);
            // 
            // cmbKeys
            // 
            this.cmbKeys.FormattingEnabled = true;
            this.cmbKeys.Location = new System.Drawing.Point(6, 23);
            this.cmbKeys.Name = "cmbKeys";
            this.cmbKeys.Size = new System.Drawing.Size(121, 24);
            this.cmbKeys.TabIndex = 0;
            this.cmbKeys.SelectedIndexChanged += new System.EventHandler(this.cmbKeys_SelectedIndexChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(395, 145);
            this.Controls.Add(this.grpParams);
            this.Controls.Add(this.txtDir);
            this.Controls.Add(this.btnGo);
            this.Controls.Add(this.btnSelectDir);
            this.Name = "MainForm";
            this.Text = "SQLtoTXT";
            this.grpParams.ResumeLayout(false);
            this.grpParams.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSelectDir;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.TextBox txtDir;
        private System.Windows.Forms.GroupBox grpParams;
        private System.Windows.Forms.TextBox txtValue;
        private System.Windows.Forms.ComboBox cmbKeys;
    }
}

