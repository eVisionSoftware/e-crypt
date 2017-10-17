namespace eVision.Decryption.UI
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.destinationField = new System.Windows.Forms.TextBox();
            this.selectDestination = new System.Windows.Forms.Button();
            this.decrypt = new System.Windows.Forms.Button();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.selectPrivateKey = new System.Windows.Forms.Button();
            this.privateKeyPathField = new System.Windows.Forms.TextBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.privateKeyValidityLabel = new System.Windows.Forms.Label();
            this.lblSuccess = new System.Windows.Forms.Label();
            this.openDestination = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Private key";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 3;
            this.label2.Tag = "";
            this.label2.Text = "Destination";
            // 
            // destinationField
            // 
            this.destinationField.Location = new System.Drawing.Point(79, 43);
            this.destinationField.Name = "destinationField";
            this.destinationField.ReadOnly = true;
            this.destinationField.Size = new System.Drawing.Size(271, 20);
            this.destinationField.TabIndex = 4;
            // 
            // selectDestination
            // 
            this.selectDestination.Location = new System.Drawing.Point(356, 41);
            this.selectDestination.Name = "selectDestination";
            this.selectDestination.Size = new System.Drawing.Size(30, 23);
            this.selectDestination.TabIndex = 5;
            this.selectDestination.Text = "...";
            this.selectDestination.UseVisualStyleBackColor = true;
            this.selectDestination.Click += new System.EventHandler(this.openFileDialog_Click);
            // 
            // decrypt
            // 
            this.decrypt.Enabled = false;
            this.decrypt.Location = new System.Drawing.Point(311, 70);
            this.decrypt.Name = "decrypt";
            this.decrypt.Size = new System.Drawing.Size(75, 23);
            this.decrypt.TabIndex = 6;
            this.decrypt.Text = "Decrypt";
            this.decrypt.UseVisualStyleBackColor = true;
            this.decrypt.Click += new System.EventHandler(this.decrypt_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(15, 70);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(290, 23);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar.TabIndex = 8;
            // 
            // selectPrivateKey
            // 
            this.selectPrivateKey.Location = new System.Drawing.Point(356, 13);
            this.selectPrivateKey.Name = "selectPrivateKey";
            this.selectPrivateKey.Size = new System.Drawing.Size(30, 23);
            this.selectPrivateKey.TabIndex = 1;
            this.selectPrivateKey.Text = "...";
            this.selectPrivateKey.UseVisualStyleBackColor = true;
            this.selectPrivateKey.Click += new System.EventHandler(this.privateKey_OpenFileDialog_Click);
            // 
            // privateKeyPathField
            // 
            this.privateKeyPathField.AllowDrop = true;
            this.privateKeyPathField.Location = new System.Drawing.Point(79, 15);
            this.privateKeyPathField.Name = "privateKeyPathField";
            this.privateKeyPathField.ReadOnly = true;
            this.privateKeyPathField.Size = new System.Drawing.Size(271, 20);
            this.privateKeyPathField.TabIndex = 9;
            this.privateKeyPathField.TextChanged += new System.EventHandler(this.privateKeyField_TextChanged);
            this.privateKeyPathField.DragDrop += new System.Windows.Forms.DragEventHandler(this.privateKeyPathField_DragDrop);
            this.privateKeyPathField.DragEnter += new System.Windows.Forms.DragEventHandler(this.privateKeyPathField_DragEnter);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "PGP files|*.asc;*.pgp|All files|*.*";
            // 
            // privateKeyValidityLabel
            // 
            this.privateKeyValidityLabel.AutoSize = true;
            this.privateKeyValidityLabel.ForeColor = System.Drawing.Color.Red;
            this.privateKeyValidityLabel.Location = new System.Drawing.Point(76, 76);
            this.privateKeyValidityLabel.Name = "privateKeyValidityLabel";
            this.privateKeyValidityLabel.Size = new System.Drawing.Size(0, 13);
            this.privateKeyValidityLabel.TabIndex = 11;
            // 
            // lblSuccess
            // 
            this.lblSuccess.AutoSize = true;
            this.lblSuccess.ForeColor = System.Drawing.Color.Green;
            this.lblSuccess.Location = new System.Drawing.Point(12, 78);
            this.lblSuccess.Name = "lblSuccess";
            this.lblSuccess.Size = new System.Drawing.Size(132, 13);
            this.lblSuccess.TabIndex = 12;
            this.lblSuccess.Text = "Decryption was successful";
            this.lblSuccess.Visible = false;
            // 
            // openDestination
            // 
            this.openDestination.Location = new System.Drawing.Point(267, 70);
            this.openDestination.Name = "openDestination";
            this.openDestination.Size = new System.Drawing.Size(119, 23);
            this.openDestination.TabIndex = 13;
            this.openDestination.Text = "Open destination";
            this.openDestination.UseVisualStyleBackColor = true;
            this.openDestination.Visible = false;
            this.openDestination.Click += new System.EventHandler(this.openDestination_Click);
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(390, 100);
            this.Controls.Add(this.openDestination);
            this.Controls.Add(this.lblSuccess);
            this.Controls.Add(this.privateKeyValidityLabel);
            this.Controls.Add(this.selectPrivateKey);
            this.Controls.Add(this.privateKeyPathField);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.decrypt);
            this.Controls.Add(this.selectDestination);
            this.Controls.Add(this.destinationField);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Decrypt";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox destinationField;
        private System.Windows.Forms.Button selectDestination;
        private System.Windows.Forms.Button decrypt;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button selectPrivateKey;
        private System.Windows.Forms.TextBox privateKeyPathField;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Label privateKeyValidityLabel;
        private System.Windows.Forms.Label lblSuccess;
        private System.Windows.Forms.Button openDestination;
    }
}

