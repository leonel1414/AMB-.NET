using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Entidade;
using Datos;

namespace Clase11
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Datos.Dal objdal = new Datos.Dal();//instacio capa Datos y clase Dal
            Datosalumnos objAlum = new Datosalumnos();

            DataSet objds = new DataSet(); // instacio clase Dataset
            // la variable de clase "objdal", realiza la conexion.
            //utiliza el metodo "BuscarLegajo" para buscar informacion y almacenarla en la variable de clase
            objds = objdal.BuscarLegajo(Convert.ToInt32(txtLegajo.Text));

            DataTable dtAlumnos = objds.Tables[0]; ; //representa la tabla de un DataSet
            dataGridView1.DataSource = dtAlumnos;

        }
    }
}
