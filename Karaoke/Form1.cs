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
using WMPLib;
using System.Text.RegularExpressions;

namespace Caraoke
{
    public partial class Form1 : Form
    {
        StreamWriter tuberia;
        OpenFileDialog openFileDialog = new OpenFileDialog();
        FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

        int index = 0, indexlf;
        string[] archivo, ruta, ruta2;

        List<string> miLista = new List<string>();
        List<string> miListareservas = new List<string>();
        List<string> miListaconfig = new List<string>();


        public string fileconfig, url;
        public IWMPPlaylist playlist, playlist2;
        IWMPMedia media;
        
        public Form1()
        {
            InitializeComponent();
            playlist = MediaPlayer1.playlistCollection.newPlaylist("myplaylist");
            playlist2 = MediaPlayer1.playlistCollection.newPlaylist("myplaylist2");
            string path = Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().Location).FullName;
            fileconfig = Path.Combine(path, "config.txt");

            if (!File.Exists(fileconfig))
            {
                // Create the file.
                using (FileStream fs = File.Create(fileconfig))
                {
                    Byte[] info =
                        new UTF8Encoding(true).GetBytes(@"D:\1Karaoke\1");
                    // Add some information to the file.
                    fs.Write(info, 0, info.Length);
                }
            }

            leertxt();
            playlistpv();
            WindowState = FormWindowState.Maximized;
            listBox1.Visible = false;
            this.ActiveControl = txt_codigo;
        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog.Multiselect = true;
            openFileDialog.InitialDirectory = label1.Text;
            openFileDialog.Filter = " archivo MP4 |*.mp4| archivo MP3 |*.mp3| archivo AVI |*.avi";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string[] data = openFileDialog.SafeFileNames;

                for (int i = 0; i < data.Length; i++)
                {
                    listBox1.Items.Add(data[i]);
                }
                //archivo = openFileDialog.SafeFileNames;
                archivo = archivo.Concat(data).ToArray();
                //ruta = openFileDialog.FileNames;
                ruta = ruta.Concat(openFileDialog.FileNames).ToArray();
            }
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            bonton();
        }

        public void bonton()
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                label1.Text = folderBrowserDialog.SelectedPath;

                StreamWriter file = new StreamWriter(fileconfig, false);
                file.WriteLine(folderBrowserDialog.SelectedPath);
                file.Flush();
                file.Close();
                leertxt();
                playlistpv();
            }
        }
        
        public void leertxt()
        {
            StreamReader tuberia;
            tuberia = File.OpenText(fileconfig);
            string[] listaconfig;

            var text = @"D:\1Karaoke\1";
            string[] lineaTexto;
            try
            {
                foreach (string item in File.ReadAllLines(fileconfig, Encoding.Default))
                {
                    lineaTexto = item.Split(Convert.ToChar(@"-"));
                    switch (lineaTexto[0])
                    {
                        case "url":
                            miListaconfig.Add(lineaTexto[1]);
                            return;
                        case "url2":
                            miListaconfig.Add(lineaTexto[1]);
                            return;
                        default:
                            break;
                    }
                }

                url = miListaconfig[0];
                label1.Text = url;
                tuberia.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("error: " + ex.Message);
            }
        }

        public void playlistpv()
        {
            listBox1.Items.Clear();
            archivo = new DirectoryInfo(miListaconfig[0]).GetFiles("*.*", SearchOption.AllDirectories).Select(o => o.Name).Where(s => s.EndsWith(".mp4") || s.EndsWith(".avi")).ToArray<string>();

            ruta = Directory.GetFiles(miListaconfig[0], "*.*", SearchOption.AllDirectories)
            .Where(s => s.EndsWith(".mp4") || s.EndsWith(".avi")).ToArray<string>();

            //playlist = MediaPlayer1.playlistCollection.newPlaylist("myplaylist");

            for (int i = 0; i < archivo.Length; i++)
            {
                listBox1.Items.Add(archivo[i]);
                media = MediaPlayer1.newMedia(ruta[i]);
                playlist.appendItem(media);
            }
            //
            MediaPlayer1.currentPlaylist = playlist;
            MediaPlayer1.Ctlcontrols.playItem(playlist.Item[0]);
            listBox1.SelectedIndex = 0;
        }

        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                if (MediaPlayer1.currentPlaylist.name == "myplaylist2")
                {
                    MediaPlayer1.currentPlaylist = playlist;
                    lbl_actual.Text = "";
                    lbl_reserva.Text = "";
                    playlist2.clear();
                    miListareservas.Clear();
                    indexlf = 0;
                    index = 0;
                    txt_codigo.Clear();
                    //MediaPlayer1.URL = ruta[listBox1.SelectedIndex];
                }
                MediaPlayer1.Ctlcontrols.playItem(playlist.Item[listBox1.SelectedIndex]);
                
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MediaPlayer1.settings.setMode("loop", true);
        }
        
        private void MediaPlayer1_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            string name = MediaPlayer1.currentPlaylist.name;
            string reserva = "";
            if (e.newState == 8 &&name == "myplaylist")
            {
                if (listBox1.SelectedIndex == listBox1.Items.Count - 1)
                {
                    listBox1.SelectedIndex = 0;
                    lbl_actual.Text = "";
                }
                else
                {
                    listBox1.SelectedIndex = listBox1.SelectedIndex + 1;
                }
            }
            if (e.newState == 8 && name == "myplaylist2" && playlist2.count > 0)
            {
                string[] lineaTexto = miListareservas[0].Split(Convert.ToChar(@"-"));
                string nom = lineaTexto[lineaTexto.Count() - 1];
                miListareservas.RemoveAt(0);
                index--;
                int n = playlist2.count;
                if (index == 0)
                {
                    MediaPlayer1.currentPlaylist = playlist;
                    if (indexlf == 0)
                    {
                        //MediaPlayer1.Ctlcontrols.playItem(playlist.Item[0]);
                        //MediaPlayer1.Ctlcontrols.playItem(playlist.Item[listBox1.SelectedIndex]);
                        listBox1.SelectedIndex = 0;
                    }
                    else
                    {
                        MediaPlayer1.Ctlcontrols.playItem(playlist.Item[(indexlf - 1)]);
                    }

                    lbl_actual.Text = "";
                    playlist2.clear();
                }
                n = playlist2.count;
                for (int i = 1; i < miListareservas.Count; i++)
                {
                    reserva = reserva + " " + miListareservas[i];
                }
                lbl_reserva.Text = reserva;
            }

            if(e.newState == 3)
            {
                if (MediaPlayer1.currentPlaylist.name == "myplaylist2" && playlist2.count > 0)
                {
                    lbl_actual.Text = miListareservas[0];
                }
                //MediaPlayer1.Ctlcontrols.playItem(playlist2.Item[0]);
            }
        }
        
        private void txt_codigo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.C)
            {
                Config config = new Config(url);
                config.ShowDialog();
                //bonton();
            }
            if (e.KeyCode == Keys.X)
            {
                Application.Exit();
            }
            if (e.KeyCode == Keys.L)
            {
                if (listBox1.Visible)
                {
                    listBox1.Visible = false;
                }
                else
                {
                    listBox1.Visible = true;
                }
            }
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                try
                {
                    if (lbl_actual.Text != null)
                    {
                        string[] lineaTexto;
                        string texto = txt_codigo.Text;
                        
                        lineaTexto = texto.Split(Convert.ToChar(@"-"));

                        string  rutav = url;

                        for (int i = 0; i < lineaTexto.Length; i++)
                        {
                            rutav += "\\" + lineaTexto[i];
                        }
                       
                        if (File.Exists(rutav+".mp4"))
                        {
                            miLista.Add(rutav + ".mp4");
                            listBox2.Items.Add(rutav + ".mp4");

                        }
                        if (File.Exists(rutav + ".avi"))
                        {
                            miLista.Add(rutav + ".avi");
                            listBox2.Items.Add(rutav + ".avi");
                        }

                        ruta2 = miLista.ToArray();
                        
                        media = MediaPlayer1.newMedia(ruta2[ruta2.Length - 1]);
                        playlist2.appendItem(media);
                        
                        if (index==0)
                        {
                            for (int i = 0; i < playlist.count - 1; i++)
                            {
                                if (MediaPlayer1.currentMedia.isIdentical[playlist.Item[i]])
                                {
                                    indexlf = i;
                                    break;
                                }
                            }

                            MediaPlayer1.currentPlaylist = playlist2;
                            MediaPlayer1.Ctlcontrols.play();
                            lbl_actual.Text = txt_codigo.Text;
                        }

                        index++;
                        miListareservas.Add(txt_codigo.Text);
                        string reserva = "";
                        for (int i = 0; i < miListareservas.Count; i++)
                        {
                            if (lbl_actual.Text == miListareservas[i])
                            {
                                reserva = "";
                            }
                            else
                            {
                                reserva = reserva + " " + miListareservas[i];
                            }
                        }
                        lbl_reserva.Text = reserva;
                        txt_codigo.Clear();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error : \n" + ex.Message, "Error");
                }
                
            }
            //txt_codigo.Clear();
        }

        public void play()
        {
            if (listBox1.SelectedIndex < listBox1.Items.Count - 1)
            {
                listBox1.SelectedIndex = listBox1.SelectedIndex + 1;
            }
        }
    }
}

//El tiempo de 30 minutos de 
//prueba a termiando.
//Si desea tener el programa
//sin limite de tiempo, deberá
//registrarlo.

