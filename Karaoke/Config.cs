using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace Caraoke
{
    public partial class Config : Form
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
        
        public Config(string url)
        {
            InitializeComponent();
            lbl_url.Text = url;
        }
        
        private void btn_select_directory_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                lbl_url.Text = folderBrowserDialog.SelectedPath;

                StreamWriter file = new StreamWriter(@"D:\1Karaoke\config.txt", false);
                file.WriteLine(folderBrowserDialog.SelectedPath);
                file.Flush();
                file.Close();
                ///Form1 form1 = new Form1(lbl_url.Text);
                
                ///form1.Refresh();

            }
        }

        private void Config_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form1 frm1 = Application.OpenForms.OfType<Form1>().FirstOrDefault();
            
            if (frm1 != null)  //Si encuentra una instancia abierta
            {
                frm1.Refresh();
            }
        }
    }
}
