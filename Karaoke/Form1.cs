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
using System.Configuration;
using System.Threading; 
using System.Timers;
using System.Security.Cryptography;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace Karaoke
{
    public partial class Form1 : Form
    { 
        
        FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

        int index = 0, indexlf;
        string[] archivo, ruta, ruta2;

        List<string> miLista = new List<string>();
        List<string> miListareservas = new List<string>();
        
        public IWMPPlaylist playlist, playlist2;
        IWMPMedia media;
        string[] extenciones = { ".mp4", ".avi", ".mp3" };
        public System.Windows.Forms.Timer temporizador = new System.Windows.Forms.Timer();
        public int Tiempo = 0;

        //C4B2E76CE1E2CD9D08643D79A2672877
        //BitProject2020

        public string hash = BitConverter.ToString(MD5.Create().ComputeHash(Encoding.ASCII.GetBytes("BitProject2020"))).Replace("-","");
        
        public Form1()
        {
            InitializeComponent();

            pictureBox2.Controls.Add(MediaPlayer1);
            pictureBox2.Location = new Point(0, 0);
            pictureBox2.BackColor = Color.Transparent;

            //pictureBox1.BackColor = Color.Transparent;
            //pictureBox1.Parent = pictureBox2;

            playlist = MediaPlayer1.playlistCollection.newPlaylist("myplaylist");
            playlist2 = MediaPlayer1.playlistCollection.newPlaylist("myplaylist2"); 
            
            playlistpv();
            WindowState = FormWindowState.Maximized;
            listBox1.Visible = false;
            listBox2.Visible = false;
            label1.Visible = false;
            this.ActiveControl = txt_codigo;
            
            var macAddr =
                (
                    from nic in NetworkInterface.GetAllNetworkInterfaces()
                    where nic.OperationalStatus == OperationalStatus.Up
                    select nic.GetPhysicalAddress().ToString()
                ).FirstOrDefault();
            string chash = ConfigurationManager.AppSettings["hash"].ToString();
            string chash2 = ConfigurationManager.AppSettings["hash2"].ToString();

            if (chash == hash && chash2 == macAddr)
            {
                
            }
            else
            {
                temporizador.Interval = 1000;
                temporizador.Start();
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            temporizador.Tick += new EventHandler(Cuenta);
        }
        void Cuenta(object sender, EventArgs e)
        {
            Tiempo += 1;
            label1.Text = Tiempo.ToString();
            if (Tiempo == 1800)
            {
                Application.Exit();
            }
        }
        public void playlistpv()
        {
            Random rnd = new Random();
            List<string> miLis = new List<string>();
            listBox1.Items.Clear();
            if (ConfigurationManager.AppSettings["urlvideo"] != "" && Directory.Exists(ConfigurationManager.AppSettings["urlvideo"]))
            {
                archivo = new DirectoryInfo(ConfigurationManager.AppSettings["urlvideo"].ToString()).GetFiles("*.*", SearchOption.AllDirectories).Select(o => o.Name)
                    .Where(s => s.EndsWith(".mp4") || s.EndsWith(".avi") || s.EndsWith(".mp3")).ToArray<string>();

                ruta = Directory.GetFiles(ConfigurationManager.AppSettings["urlvideo"].ToString(), "*.*", SearchOption.AllDirectories)
                    .Where(s => s.EndsWith(".mp4") || s.EndsWith(".avi") || s.EndsWith(".mp3")).ToArray<string>() ;

                if (ConfigurationManager.AppSettings["videos_aleatorios"].ToString() == "true")
                {
                    Shuffle(ruta);
                }
            
                for (int i = 0; i < archivo.Length; i++)
                {
                    listBox1.Items.Add((i + 1) + " - " + (i + 1));
                    media = MediaPlayer1.newMedia(ruta[i]);
                    playlist.appendItem(media);
                }
            
                MediaPlayer1.currentPlaylist = playlist;
                MediaPlayer1.Ctlcontrols.playItem(playlist.Item[0]);
                listBox1.SelectedIndex = 0;
            }
        }

        public static void Shuffle<T>(IList<T> values)
        {
            var n = values.Count;
            var rnd = new Random();
            for (int i = n - 1; i > 0; i--)
            {
                var j = rnd.Next(0, i);
                var temp = values[i];
                values[i] = values[j];
                values[j] = temp;
            }
        }
        
        private void txt_codigo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '-'))
            {
                e.Handled = true;
            }
        }
        
        private void MediaPlayer1_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            string name = MediaPlayer1.currentPlaylist.name;
            
            if (e.newState == 8 && name == "myplaylist")
            {
                nextcanciones();
            }
            if (e.newState == 8 && name == "myplaylist2" && playlist2.count > 0)
            {
                removercanciones();
            }

            if(e.newState == 3)
            {
                if (MediaPlayer1.currentPlaylist.name == "myplaylist2" && playlist2.count > 0)
                {
                    lbl_actual.Text = miListareservas[0];
                }
            }
        }
        public void nextcanciones()
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
            if (MediaPlayer1.currentMedia.isIdentical[playlist.Item[playlist.count - 1]])
            {
                MediaPlayer1.Ctlcontrols.play();
            }
        }
        public void removercanciones()
        {
            string reserva = "";
            miListareservas.RemoveAt(0);
            index--;
            int n = playlist2.count;
            if (index == 0)
            {
                MediaPlayer1.currentPlaylist = playlist;
                if (indexlf == 0)
                {
                    listBox1.SelectedIndex = 0;
                }
                else
                {
                    MediaPlayer1.Ctlcontrols.playItem(playlist.Item[(indexlf - 1)]);
                }

                lbl_actual.Text = "";
                playlist2.clear();
                miLista.Clear();
                listBox2.Items.Clear();
            }
            else
            {
                miLista.RemoveAt(0);
                listBox2.Items.RemoveAt(0);
            }

            for (int i = 1; i < miListareservas.Count; i++)
            {
                reserva = reserva + " " + miListareservas[i];
            }
            
            lbl_reserva.Text = reserva;
        }
        
        private void txt_codigo_KeyDown(object sender, KeyEventArgs e)
        {
            Configuraciones config = new Configuraciones();
            string[] lineaTexto;
            if (e.KeyCode == Keys.C)
            {
                config.ShowDialog();
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
                    txt_codigo.Clear();
                }
                else
                {
                    listBox1.Visible = true;
                }
            }

            if (e.KeyCode == Keys.A)
            {
                if (listBox2.Visible)
                {
                    listBox2.Visible = false;
                    txt_codigo.Clear();
                }
                else
                {
                    listBox2.Visible = true;
                }
            }

            if (e.KeyCode == Keys.B)
            {
                for (int i = 0; i < playlist.count - 1; i++)
                {
                    if (MediaPlayer1.currentMedia.isIdentical[playlist.Item[i]])
                    {
                        MediaPlayer1.Ctlcontrols.playItem(playlist.Item[i]);
                        break;
                    }
                }
            }

            if (e.KeyCode == Keys.S)
            {
                MediaPlayer1.Ctlcontrols.next();
                if (MediaPlayer1.currentPlaylist.name == "myplaylist2" && playlist2.count > 0)
                {
                    removercanciones();
                }
                if (MediaPlayer1.currentPlaylist.name == "myplaylist")
                {
                    nextcanciones();
                }
            }

            if (e.KeyCode == Keys.P)
            {
                if ( MediaPlayer1.playState == WMPPlayState.wmppsPlaying)
                {
                    MediaPlayer1.Ctlcontrols.pause();
                }
                else
                {
                    MediaPlayer1.Ctlcontrols.play();
                }
            }

            if (e.KeyCode == Keys.Down)
            {
                if (listBox1.Visible)
                {
                    if (listBox1.SelectedIndex < listBox1.Items.Count - 1)
                    {
                        listBox1.SelectedIndex = listBox1.SelectedIndex + 1;
                        lineaTexto = listBox1.SelectedItem.ToString().Split(Convert.ToChar(@"-"));
                        txt_codigo.Text = "-" + lineaTexto[1];
                    }
                }
            }

            if (e.KeyCode == Keys.Up)
            {
                if (listBox1.Visible)
                {
                    if (listBox1.SelectedIndex > 0)
                    {
                        listBox1.SelectedIndex = listBox1.SelectedIndex - 1;
                        lineaTexto = listBox1.SelectedItem.ToString().Split(Convert.ToChar(@"-"));
                        txt_codigo.Text = "-" + lineaTexto[1];
                    }
                }
            }

            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                if (listBox1.Visible)
                {
                    if (MediaPlayer1.currentPlaylist.name == "myplaylist2")
                    {
                        MediaPlayer1.currentPlaylist = playlist;
                        playlist2.clear();
                        miListareservas.Clear();
                        limpiar();
                    }
                    MediaPlayer1.Ctlcontrols.playItem(playlist.Item[listBox1.SelectedIndex]);
                    listBox1.Visible = false;
                    txt_codigo.Clear();
                }
                else
                {
                    try
                    {
                        if (lbl_actual.Text != null)
                        {
                            if (ConfigurationManager.AppSettings["url"].ToString() != "" && Directory.Exists(ConfigurationManager.AppSettings["url"].ToString()))
                            {
                                string rutav = ConfigurationManager.AppSettings["url"].ToString();
                                
                                int id=0;

                                lineaTexto = txt_codigo.Text.Split(Convert.ToChar(@"-"));

                                for (int i = 0; i < lineaTexto.Length; i++)
                                {
                                    rutav += "\\" + lineaTexto[i];
                                }

                                for (int i = 0; i < extenciones.Length; i++)
                                {
                                    if (File.Exists(rutav + extenciones[i]))
                                    {
                                        string exits = miLista.Find(x => x == rutav + extenciones[i]);
                                        if (ConfigurationManager.AppSettings["reservas_mp"].ToString() == "true" || exits == null)
                                        {
                                            miLista.Add(rutav + extenciones[i]);
                                            listBox2.Items.Add(txt_codigo.Text);
                                            agregarreservacancion(rutav);
                                        }
                                        else
                                        {
                                            MessageBox.Show("Ya esta reservado", "Error");
                                        }
                                    }
                                    else
                                    {
                                        id++;
                                    }
                                }
                                if (id == extenciones.Length)
                                {
                                    MessageBox.Show("no existe", "Error");
                                }
                            }
                            else
                            {
                                MessageBox.Show("Error : Tiene agregar un Directorio de pistas de karaoke", "Error");
                                config.ShowDialog();
                            }
                            txt_codigo.Clear();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error : \n" + ex.Message, "Error");
                    }
                }
            }
        }

        public void agregarreservacancion(string rutav)
        {
            ruta2 = miLista.ToArray();

            media = MediaPlayer1.newMedia(ruta2[ruta2.Length - 1]);
            playlist2.appendItem(media);

            if (index == 0)
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
                    if (ConfigurationManager.AppSettings["reservas_mp"].ToString() == "true" && i>0)
                    {
                        reserva = reserva + " " + miListareservas[i];
                    }
                }
                else
                {
                    reserva = reserva + " " + miListareservas[i];
                }
            }
            lbl_reserva.Text = reserva;
        }

        public void limpiar()
        {
            lbl_actual.Text = "";
            lbl_reserva.Text = "";
            indexlf = 0;
            index = 0;
            txt_codigo.Clear();
            miLista.Clear();
            playlist2.clear();
            listBox2.DataSource = null;
            listBox2.Items.Clear();
        }
        public void sonido(int valor)
        {
            MediaPlayer1.settings.volume = valor;
        }
        
    }
}
