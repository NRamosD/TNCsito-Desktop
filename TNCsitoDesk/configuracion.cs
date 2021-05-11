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
        public configuracion()
        {
            InitializeComponent();
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
                Properties.Settings.Default["nameroom"] = tbNombre.Text;
                Properties.Settings.Default["passroom"] = tbContra.Text;
                Properties.Settings.Default["activeroom"] = false;
                Properties.Settings.Default.Save();
            }
            catch(Exception exc)
            {
                MessageBox.Show("Ha ocurrido un problema","Lo sentimos");
            }
            //regreso
            inicio.Show();
            this.Close();
        }

        private void configuracion_Load(object sender, EventArgs e)
        {
            tbNombre.Text = Properties.Settings.Default["nameroom"].ToString();
            tbContra.Text = Properties.Settings.Default["passroom"].ToString();
        }
    }
}
