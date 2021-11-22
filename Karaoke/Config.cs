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
using System.Configuration;
using System.Security.Cryptography;
using System.Net.NetworkInformation;

namespace Karaoke
{
    public partial class Configuraciones : Form
    {
        FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
        Form1 frm1 = Application.OpenForms.OfType<Form1>().FirstOrDefault();
        bool estado=false;
       
        public Configuraciones()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            lbl_url.Text = ConfigurationManager.AppSettings["url"];
            lbl_urlvideo.Text = ConfigurationManager.AppSettings["urlvideo"];
            lbl_urlmp3.Text = ConfigurationManager.AppSettings["urlmp3"];
            trackBar2.Value = Convert.ToInt32(ConfigurationManager.AppSettings["volumen"]);
            if (ConfigurationManager.AppSettings["videos_aleatorios"].ToString() == "true")
            {
                chbx_videos_aleatorios.Checked=true;
            }
            if (ConfigurationManager.AppSettings["reservas_mp"].ToString() == "true")
            {
                chbx_reservas_mp.Checked = true;
            }
            
            string chash = ConfigurationManager.AppSettings["hash"].ToString();
            string chash2 = ConfigurationManager.AppSettings["hash2"].ToString();
            var macAddr =
                    (
                        from nic in NetworkInterface.GetAllNetworkInterfaces()
                        where nic.OperationalStatus == OperationalStatus.Up
                        select nic.GetPhysicalAddress().ToString()
                    ).FirstOrDefault();
            if (chash == frm1.hash && chash2 == macAddr)
            {
                txt_licencia.Enabled = false;
                btn_registrar.Enabled = false;
            }
        }
        
        private void btn_select_directory_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    lbl_url.Text = folderBrowserDialog.SelectedPath;
                    estado = true;
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    KeyValueConfigurationCollection settings = config.AppSettings.Settings;
                    
                    settings["url"].Value = folderBrowserDialog.SelectedPath;
                    
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("error: " + ex.Message);
                }
            }
        }
        
        private void btn_aplicar_Click(object sender, EventArgs e)
        {
            if (estado)
            {
                if (ConfigurationManager.AppSettings["urlvideo"].ToString() != "")
                {
                    frm1.playlist.clear();
                    frm1.limpiar();
                }
                frm1.playlistpv();
            }

            frm1.sonido(trackBar2.Value);
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                KeyValueConfigurationCollection settings = config.AppSettings.Settings;
                
                settings["volumen"].Value = trackBar2.Value.ToString();

                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("error: " + ex.Message);
            }
            this.Close();
        }
        
        private void btn_select_directory_video_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    lbl_urlvideo.Text = folderBrowserDialog.SelectedPath;
                    estado = true;
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    KeyValueConfigurationCollection settings = config.AppSettings.Settings;

                    settings["urlvideo"].Value = folderBrowserDialog.SelectedPath;

                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("error: " + ex.Message);
                }
            }
        }

        private void Config_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                if (trackBar2.Value < 100)
                {
                    trackBar2.Value = trackBar2.Value + 1;
                }
            }

            if (e.KeyCode == Keys.Down)
            {
                if (trackBar2.Value > 0)
                {
                    trackBar2.Value = trackBar2.Value - 1;
                }
            }

            if (e.KeyCode == Keys.C)
            {
                this.Close();
            }
        }

        private void btn_select_directory_mp3_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    lbl_urlmp3.Text = folderBrowserDialog.SelectedPath;
                    estado = true;
                    
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    KeyValueConfigurationCollection settings = config.AppSettings.Settings;

                    settings["urlmp3"].Value = folderBrowserDialog.SelectedPath;

                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("error: " + ex.Message);
                }
            }
        }

        private void chbx_videos_aleatorios_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                KeyValueConfigurationCollection settings = config.AppSettings.Settings;

                if (chbx_videos_aleatorios.Checked == true)
                {
                    settings["videos_aleatorios"].Value = "true";
                }
                else
                {
                    settings["videos_aleatorios"].Value = "false";
                }
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);

                estado = true;
            }
                catch (Exception ex)
            {
                MessageBox.Show("error: " + ex.Message);
            }
        }

        private void chbx_reservas_mp_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                KeyValueConfigurationCollection settings = config.AppSettings.Settings;

                if (chbx_reservas_mp.Checked == true)
                {
                    settings["reservas_mp"].Value = "true";
                }
                else
                {
                    settings["reservas_mp"].Value = "false";
                }

                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
                estado = false;

            }
            catch (Exception ex)
            {
                MessageBox.Show("error: " + ex.Message);
            }
        }

        private void btn_registrar_Click(object sender, EventArgs e)
        {
            string hash = BitConverter.ToString(MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(txt_licencia.Text))).Replace("-", "");
            
            if (hash == frm1.hash)
            {
                var macAddr =
                    (
                        from nic in NetworkInterface.GetAllNetworkInterfaces()
                        where nic.OperationalStatus == OperationalStatus.Up
                        select nic.GetPhysicalAddress().ToString()
                    ).FirstOrDefault();

                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                KeyValueConfigurationCollection settings = config.AppSettings.Settings;

                settings["hash"].Value = hash;
                //MAC
                settings["hash2"].Value = macAddr;

                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
                MessageBox.Show("Mensaje : \n La licencia es correcta", "Info");

                frm1.temporizador.Stop();
                frm1.Tiempo = 0;
                txt_licencia.Clear();
            }
            else
            {
                MessageBox.Show("Error : \n La licencia es incorrecta", "Error");
                txt_licencia.Clear();
            }
            
        }
        
    }
}
