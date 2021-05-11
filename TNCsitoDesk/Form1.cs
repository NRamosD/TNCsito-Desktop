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
            //Verifico que exista la sala
            string user = Properties.Settings.Default.nameroom.ToString();
            string pass = Properties.Settings.Default.passroom.ToString();
            try
            {
                if (user != "" && pass != "")
                {
                    //start client
                    client = new FireSharp.FirebaseClient(rd.config);

                    if (client != null)
                    {
                        try
                        {
                            //deletePreviousOrders();
                            await Task.Delay(2000);
                        }
                        catch (Exception exc)
                        {
                            Console.WriteLine(exc);
                        }
                        setListener();
                        chBConect.BackColor = Color.Red;
                    }
                }
                else
                {
                    lbMensaje.Text = "Crea una sala";
                }
            }
            catch(Exception exc)
            {
                MessageBox.Show("Algo salió mal", "Ocurrió un error durante la ejecución.\nCódigo de error: " + exc);
            }
        }
        #endregion

        #region DB
        //Métodos
        async void setListener()
        {
            
            listener = await client.OnAsync($"bdtncsito/rooms/{Properties.Settings.Default.nameroom}/orders", (sender, args, context) => {
                datos.Add(args.Data.ToString());
                if (datos.Count == 3)
                {
                    //MessageBox.Show($"Entro {datos[0]} {datos[1]} {datos[2]}");
                    notificar();
                }              
            });
            
        }
        /*async void deletePreviousOrders()
        {
            //Eliminar los datos de pedidos previos
            FirebaseResponse response = await client.DeleteAsync($"bdtncsito/rooms/{Properties.Settings.Default["nameroom"]}/orders");
            if (response.StatusCode.ToString()!="OK")
            {
                MessageBox.Show("Algo salió mal","No se pudo borrar la sala.\nCódigo de error: "+ response.StatusCode);
            }
        }*/

        public async void createRoom()
        {
            try
            {
                if (searchRoom())
                {
                    Properties.Settings.Default["activeroom"] = true;
                    credentials cr = new credentials()
                    {
                        name = Properties.Settings.Default["nameroom"].ToString(),
                        pass = Properties.Settings.Default["passroom"].ToString(),
                        active = (bool)Properties.Settings.Default["activeroom"]
                    };
                    await client.SetAsync("bdtncsito/rooms/" + Properties.Settings.Default["nameroom"].ToString() + "/credentials", cr);
                    //cambio el estado en la interfaz
                    chBConect.BackColor = Color.GreenYellow;
                }
                else
                {
                    MessageBox.Show("El nombre de sala ingresado no se encuentra disponible, escriba otro nombre.","Nombre de no disponible");
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Se presentó el siguiente error: " + exc, "Hubo un problema");
            }
        }

        public async void updateRoom()
        {
            credentials cr = new credentials()
            {
                name = Properties.Settings.Default["nameroom"].ToString(),
                pass = Properties.Settings.Default["passroom"].ToString(),
                active = (bool)Properties.Settings.Default["activeroom"]
            };
            await client.UpdateAsync("bdtncsito/rooms/"+Properties.Settings.Default["nameroom"].ToString()+"/credentials", cr);
        }

        public bool searchRoom()
        {
            bool flag = false;
            var data = client.Get("bdtncsito/rooms/" + Properties.Settings.Default["nameroom"].ToString());
            if (data.Body != null)
            {
                flag = true;
                return flag;
            }
            return flag;
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
        private async void btnSalir_Click(object sender, EventArgs e)
        {
            //Dejo inactiva la sala por defecto
            Properties.Settings.Default["activeroom"] = false;
            //elimino la sala para proteger la info
            FirebaseResponse response = await client.DeleteAsync($"bdtncsito/rooms/{Properties.Settings.Default["nameroom"]}");
            await Task.Delay(2000);
            if (response.StatusCode.ToString() != "OK")
            {
                MessageBox.Show("Algo salió mal", "No se pudo borrar la sala.\nCódigo de error: " + response.StatusCode);
            }
            else
            {
                Application.Exit();
            }
            
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
        private void btnIniciar_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default["activeroom"] = true;
            createRoom();
        }

        #endregion

        private void chBConect_MouseHover(object sender, EventArgs e)
        {
            if (chBConect.BackColor == Color.GreenYellow)
            {
                toolTip.ShowAlways = true;
                toolTip.SetToolTip(chBConect, "Activo");
            }
            else
            {
                toolTip.ShowAlways = true;
                toolTip.SetToolTip(chBConect, "Desconectado");
            }
            
        }

        private void chBConect_MouseLeave(object sender, EventArgs e)
        {

        }
    }
}
