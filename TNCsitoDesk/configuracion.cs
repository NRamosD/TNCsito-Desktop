using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TNCsitoDesk
{
    public partial class configuracion : Form
    {
        string name = Properties.Settings.Default["nameroom"].ToString();
        string pass = Properties.Settings.Default["passroom"].ToString();
        List<string[]> devices;
        public configuracion(List<string[]> devices)
        {
            InitializeComponent();
            this.devices = devices;
        }
        Form1 inicio = new Form1();
        private void btnCancel_Click(object sender, EventArgs e)
        {
            
            inicio.Show();
            this.Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                if (name != tbNombre.Text || pass != tbContra.Text)
                {
                    if (tbNombre.Text!="" && tbContra.Text!="")
                    {
                        Properties.Settings.Default["nameroom"] = tbNombre.Text;
                        Properties.Settings.Default["passroom"] = tbContra.Text;
                        Properties.Settings.Default["activeroom"] = false;
                        Properties.Settings.Default.Save();
                    }
                    else { MessageBox.Show("Llene todos los campos para continuar","Faltan Datos"); }
                    
                }
                
            }
            catch(Exception exc)
            {
                MessageBox.Show("Ha ocurrido un problema.\nCódigo de error "+exc.ToString(),"Lo sentimos");
            }



            //regreso
            inicio.Show();
            this.Close();
        }
        private void showDevices()
        {
            checkedLB.Items.Add("algo",true);
            checkedLB.Items.Add("algo2");
            checkedLB.Items.Add("algo3");
            checkedLB.Items.Add("algo4");
            checkedLB.Items.Add("algo5", true);
            checkedLB.Items.Add("algo6");
            checkedLB.Items.Add("algo7");
            checkedLB.Items.Add("algo8");
            checkedLB.SetItemChecked(2,true);

        }



        private void configuracion_Load(object sender, EventArgs e)
        {
            tbNombre.Text = Properties.Settings.Default["nameroom"].ToString();
            tbContra.Text = Properties.Settings.Default["passroom"].ToString();
            fillChBoxList();
        }

        private void fillChBoxList()
        {
            foreach (string[] device in devices)
            {
                if (device[1] == "1")
                {
                    checkedLB.Items.Add(device[0],true);
                }
                else
                {
                    checkedLB.Items.Add(device[0], false);
                }
                
            }
        }
    }
}
