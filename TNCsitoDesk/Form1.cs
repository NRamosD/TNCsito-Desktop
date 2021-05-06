using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;


namespace TNCsitoDesk
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }




        IFirebaseClient client;
        roomdata rd = new roomdata();
        EventStreamResponse listener;
        List<string> datos = new List<string>();

        #region Inicio
        private async void Form1_Load(object sender, EventArgs e)
        {
            //Limpiar lista deshabilitado
            btnLimpiar.Enabled = false;
            //Verifico que exista la sala
            if (rd.getUser()!="" && rd.getPass()!= "")
            {
                rd.buscarSala();
                btnLimpiar.Enabled = true;
            }

            //start client
            client = new FireSharp.FirebaseClient(rd.config);

            if (client != null)
            {
                try
                {
                    eliminarTodoAlIniciar();
                    await Task.Delay(2000);
                }
                catch(Exception exc)
                {
                    Console.WriteLine(exc);
                }
                setListener();
            }
        }
        #endregion

        #region DB
        //Métodos
        async void setListener()
        {
            
            listener = await client.OnAsync("req", (sender, args, context) => {
                datos.Add(args.Data.ToString());
                if (datos.Count == 3)
                {
                    //MessageBox.Show($"Entro {datos[0]} {datos[1]} {datos[2]}");
                    notificar();
                }              
            });
            
        }
        async void eliminarTodoAlIniciar()
        {
            FirebaseResponse response = await client.DeleteAsync("req");
            //MessageBox.Show(""+response.StatusCode);
        }
        private async void LiveCall()
        {
            ///-MZJvf63pzVNCoAoVd_F
            var res = await client.GetAsync("req/-MZJvf63pzVNCoAoVd_F");
            //var objResponse1 = JsonConvert.DeserializeObject<List<Pedido>>(res.Body);
            Pedido p = res.ResultAs<Pedido>();
            //Pedido p = res.ResultAs<Pedido[]>();
            MessageBox.Show("uhm\n" + p.tipo);
        }

        public async void crearSala()
        {
            try
            {
                //MessageBox.Show("Hubo\n"+ Properties.Settings.Default["nameroom"].ToString() + "---"+ 
                  //  Properties.Settings.Default["passroom"].ToString());
                //Pedido p = new Pedido("","",1);
                await client.SetAsync($"rama/"+Properties.Settings.Default["nameroom"].ToString(),"");
                await client.SetAsync($"rama/" + Properties.Settings.Default["nameroom"]+"/algomas".ToString(), "");
            }
            catch (Exception exc)
            {
                MessageBox.Show("Hubo un problema","Se presentó el siguiente error: "+exc);
            }
        }





        #endregion

        #region Métodos locales
        public void notificar()
        {
            int tipo= int.Parse(datos[2]);
            string mensaje=datos[0];
            string remitente = datos[1];
            notificacion.Text = "Atiende tus notificaciones...";
            notificacion.Visible = true;
            switch (tipo)
            {
                case 1:
                    notificacion.BalloonTipTitle = "Te estan llamando :/";
                    notificacion.BalloonTipText = "Alguien necesita que vayas por algún motivo desconocido u.u";
                    notificacion.ShowBalloonTip(3000);
                    break;
                case 2:
                    
                    notificacion.BalloonTipTitle = "Quieren que lleves algo...";
                    notificacion.BalloonTipText = mensaje;
                    notificacion.ShowBalloonTip(3000);
                    break;
                case 3:
                    notificacion.BalloonTipTitle = "Alguien pregunta si...";
                    notificacion.BalloonTipText = mensaje;
                    notificacion.ShowBalloonTip(3000);
                    break;
                case 4:
                    notificacion.BalloonTipTitle = "EMERGENCIA!!";
                    notificacion.BalloonTipText = "Te están llamando, ve lo más rápido que puedas.";
                    notificacion.ShowBalloonTip(3000);
                    break;
                default:
                    break;

            }
            addListBox(mensaje, remitente,tipo);
            datos = new List<string>();
        }

        private void addListBox(string men, string remi, int tipo)
        {
            string hora = DateTime.Now.ToString("hhmmss");
            Thread TypingThread = new Thread(delegate () {

                //heavyBackgroundTask();

                // Cambiar el estado de los botones dentro del hilo TypingThread
                // Esto no generará excepciones de nuevo !
                if (listBox.InvokeRequired)
                {
                    listBox.Invoke(new MethodInvoker(delegate
                    {
                        switch (tipo)
                        {
                            case 1:
                                listBox.Items.Add($"Quieren que vayas  ---  De: {remi} - {hora[0]}{hora[1]}:{hora[2]}{hora[3]}:{hora[4]}{hora[5]}");
                                break;
                            case 2:
                                listBox.Items.Add($"Llevar {men}  ---  De: {remi} - {hora[0]}{hora[1]}:{hora[2]}{hora[3]}:{hora[4]}{hora[5]}");
                                break;
                            case 3:
                                listBox.Items.Add($"¿{men}?  ---  De: {remi} - {hora[0]}{hora[1]}:{hora[2]}{hora[3]}:{hora[4]}{hora[5]}");
                                break;
                            case 4:
                                listBox.Items.Add($"EMERGENCIA  ---  De: {remi} - {hora[0]}{hora[1]}:{hora[2]}{hora[3]}:{hora[4]}{hora[5]}");
                                break;
                            default:
                                break;
                        }
                        
                    }));
                }
            });
            TypingThread.Start();
        }
        #endregion


        #region Eventos
        private void btnSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            listBox.Items.Clear();
        }

        private void btnMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }


        private void btnBegin_Click(object sender, EventArgs e)
        {
            this.Hide();
            configuracion fconfig = new configuracion();
            fconfig.Show();
        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            crearSala();
        }
    }

    public class Pedido
    {
        public Pedido(string r, string m, int t)
        {
            this.remitente = r;
            this.mensaje = m;
            this.tipo = t;
        }
        public string mensaje { get; set; }
        public string remitente { get; set; }
        public int tipo { get; set; }
    }
}
