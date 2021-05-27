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
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;
//using Microsoft.Toolkit.Uwp.Notifications;

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
                        setListener(sender,e);
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
        async void setListener(object sndr, EventArgs e)
        {
            
            listener = await client.OnAsync($"bdtncsito/rooms/{Properties.Settings.Default.nameroom}/orders", (sender, args, context) => {
                datos.Add(args.Data.ToString());
                if (datos.Count == 3)
                {
                    //MessageBox.Show($"Entro {datos[0]} {datos[1]} {datos[2]}");
                    notificar(sndr, e);
                    //notificar(sender, args);
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
        public async void notificar(object sender, EventArgs e)
        {
            int tipo= int.Parse(datos[2]);
            string mensaje=datos[0];
            string emisor = datos[1];
            await Task.Delay(1000);
            switch (tipo)
            {
                case 1:
                    OnSendNormalNotification( $"{emisor} necesita que vayas por algún motivo desconocido...", "Te estan llamando 😓");
                    break;
                case 2:
                    OnSendNormalNotification($"{mensaje}", $"{emisor} necesita algo... 🙄");
                    break;
                case 3:
                    OnSendInteractiveNotification(sender,e,mensaje, $"{emisor} pregunta...");
                    break;
                case 4:
                    OnSendNormalNotification($"{emisor} te esta llamando, ve lo más rápido que puedas.", "EMERGENCIA!! ⚠");
                    break;
                default:
                    break;

            }
            addListBox(mensaje, emisor, tipo);
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
            OnSendInteractiveNotification(sender,e,"alguna vaina","ulo");



            /*
            new ToastContentBuilder()
            .AddArgument("action", "viewConversation")
            .AddArgument("conversationId", 9813)
            .AddText("Andrew sent you a picture")
            .AddText("Check this out, The Enchantments in Washington!")
            
            .AddButton(new ToastButton()
            .SetContent("Like")
            .AddArgument("action", "like")
            .SetBackgroundActivation())

            .Show();
            
            new Toas

            const string taskName = "ToastBackgroundTask";

            // If background task is already registered, do nothing
            if (BackgroundTaskRegistration.AllTasks.Any(i => i.Value.Name.Equals(taskName)))
                return;

            // Otherwise request access
            BackgroundAccessStatus status = await BackgroundExecutionManager.RequestAccessAsync();

            // Create the background task
            BackgroundTaskBuilder builder = new BackgroundTaskBuilder()
            {
                Name = taskName
            };

            // Assign the toast action trigger
            builder.SetTrigger(new ToastNotificationActionTrigger());

            // And register the task
            BackgroundTaskRegistration registration = builder.Register();

            builder.
            //listBox.Items.Clear();*/
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



        #region Notifications

        private void OnSendInteractiveNotification(object sender, EventArgs e, string msg, string title)
        {
            ToastNotificationManagerCompat.OnActivated += (args) =>
            {
                string outcome = string.Empty;

                switch (args.Argument)
                {
                    case "yes":
                        outcome = "You have clicked yes";
                        break;
                    case "no":
                        outcome = "You have clicked no";
                        break;
                };

            };

            new ToastContentBuilder()
                .AddHeader("d",title,"w")
                .AddText(msg)
                .AddButton(new ToastButton()
                    .SetContent("Si")
                    .AddArgument("yes")
                    .SetBackgroundActivation())
                .AddButton(new ToastButton()
                    .SetContent("No")
                    .AddArgument("no")
                    .SetBackgroundActivation())
                .Show(toast =>
                {
                    toast.ExpirationTime = DateTime.Now.AddSeconds(30);
                });
        }


        private void OnSendNormalNotification(string msg, string title)
        {
            new ToastContentBuilder()
                .AddHeader("", title, "")
                .AddText(msg)
                .AddButton(new ToastButton()
                    .SetContent("Yes")
                    .AddArgument("yes")
                    .SetBackgroundActivation())
                .AddButton(new ToastButton()
                    .SetContent("No")
                    .AddArgument("no")
                    .SetBackgroundActivation())
                .Show(toast =>
                {
                    toast.ExpirationTime = DateTime.Now.AddSeconds(30);
                });
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
