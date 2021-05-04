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
            this.BackColor = Color.DarkCyan;
        }



        IFirebaseConfig config = new FirebaseConfig
        {
            
            //"ubZ1tomtK1BFgdotVQoCJiDHE4zhaEvFdxoF4TiO"    "https://tncsito-135d4-default-rtdb.firebaseio.com/"
            AuthSecret = ConfigurationManager.AppSettings["AUTH"],
            BasePath = ConfigurationManager.AppSettings["BASE_PATH"]
        };

        IFirebaseClient client;
        EventStreamResponse listener;
        List<string> datos = new List<string>();

        private async void Form1_Load(object sender, EventArgs e)
        {
            client = new FireSharp.FirebaseClient(config);
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
                //MessageBox.Show("Conección establecida\n"+client.ToString());
            }
        }


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
        public void notificar()
        {
            int tipo= int.Parse(datos[2]);
            string mensaje=datos[0];
            string remitente = datos[1];
            //notificacion.Icon = new System.Drawing.Icon(Path.GetFullPath(@"../../imagen/ayay.ico"));
            notificacion.Visible = true;
            switch (tipo)
            {
                case 1:
                    notificacion.Text = "Atiende tus notificaciones...";
                    notificacion.BalloonTipTitle = "Te estan llamando :/";//este importa
                    notificacion.BalloonTipText = "Alguien necesita que vayas por algún motivo desconocido u.u";
                    notificacion.ShowBalloonTip(3000);
                    break;
                case 2:
                    notificacion.Text = "Atiende tus notificaciones...";
                    notificacion.Icon = new System.Drawing.Icon(Path.GetFullPath(@"../../imagen/ayay.ico"));
                    notificacion.Text = "Atiende tus notificaciones...";
                    notificacion.BalloonTipTitle = "Quieren que lleves algo...";//este importa
                    notificacion.BalloonTipText = mensaje;//este importa
                    notificacion.ShowBalloonTip(3000);
                    break;
                case 3:
                    notificacion.Text = "Atiende tus notificaciones...";
                    notificacion.BalloonTipTitle = "Alguien pregunta si...";//este importa
                    notificacion.BalloonTipText = mensaje;//este importa
                    notificacion.ShowBalloonTip(3000);
                    break;
                case 4:
                    notificacion.Text = "Atiende tus notificaciones...";
                    notificacion.BalloonTipTitle = "EMERGENCIA!!";//este importa
                    notificacion.BalloonTipText = "Te están llamando, ve lo más rápido que puedas.";//este importa
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

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            listBox.Items.Clear();
        }

    }
    public class Pedido
    {
        public string mensaje { get; set; }
        public string remitente { get; set; }
        public int tipo { get; set; }
    }
}
