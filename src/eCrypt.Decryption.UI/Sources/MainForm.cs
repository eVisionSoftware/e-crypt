using System.Diagnostics;
using System.IO;

namespace eVision.Decryption.UI
{
    using eCrypt.Common;
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Linq;

    public partial class MainForm : Form
    {
        private readonly string IconResourceName = "icon.ico";
        private readonly AppService appService;
   
        public MainForm(AppService service)
        {
            InitializeComponent();
            appService = service;
            destinationField.Text = Application.StartupPath;
            Icon = new Icon(service.GetResource(IconResourceName));
            progressBar.Visible = false;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Text += string.Format(" '{0}'", Path.GetFileNameWithoutExtension(Application.ExecutablePath));
        }

        private void openFileDialog_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                destinationField.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private async void decrypt_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(destinationField.Text))
            {
                return;
            }

            string privateKey = File.ReadAllText(privateKeyPathField.Text);  
            if (!PGPValidator.IsPrivateKey(privateKey))
            {
                MessageBox.Show("Provided private key is invalid", "", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            DialogResult result;
            try
            {
                progressBar.Visible = true;
                DisableAllInputs();
                await appService.ExtractAsync(privateKey, destinationField.Text);

                progressBar.Visible = false;
                lblSuccess.Visible = true;
                selectPrivateKey.Enabled = false;
                openDestination.Visible = true;
               
                MessageBox.Show(String.Format("The package is decrypted to {0}", destinationField.Text), "", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                Logger.Error("Failed decrypt package: " + ex);
                result = MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (result == DialogResult.OK)
                {
                    Close();
                }
            }
        }

        private void DisableAllInputs()
        {
            privateKeyPathField.Enabled = selectDestination.Enabled = decrypt.Enabled = false;
        }

        private void privateKey_OpenFileDialog_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                privateKeyPathField.Text = openFileDialog.FileName;
            }
        }

        private void privateKeyField_TextChanged(object sender, EventArgs e)
        {
            string privateKey = File.ReadLines(privateKeyPathField.Text).First();
            if (!PGPValidator.IsPrivateKey(privateKey))
            {
                privateKeyValidityLabel.Text = "Provided private key is invalid";
                decrypt.Enabled = false;
            }
            else
            {
                privateKeyValidityLabel.Text = string.Empty;
                decrypt.Enabled = true;
            }
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            string[] filePaths = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            privateKeyPathField.Text = filePaths[0];
        }

        private void openDestination_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", appService.GetDecryptedDir(destinationField.Text));
            Close();
        }

        private void privateKeyPathField_DragDrop(object sender, DragEventArgs e)
        {
            privateKeyPathField.Text = e.Data.GetData(DataFormats.Text).ToString();
        }

        private void privateKeyPathField_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }
    }
}
