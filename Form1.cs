using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Datos;
using Entidade;


namespace Clase11
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cboEstadocivil.DataSource = Dal.ListarEcivil();
            cboEstadocivil.DisplayMember = "Desceciv";
            cboEstadocivil.ValueMember = "Codeciv";
            cboEstadocivil.SelectedIndex = -1; //muestra el 1 elemento del combo vacio
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            try
            {
                Datos.Dal objdal = new Datos.Dal();
                if (radioButton1.Checked == true)
                {
                    if (string.IsNullOrEmpty(txtBusqueda.Text) == true)
                    {
                        //nada..
                    }
                    else
                    {
                        string s = txtBusqueda.Text;
                        Int32 result = 0;
                        Int32.TryParse(s, out result); //convierte la cadena de un numero en entero
                        if (result == 0)
                        {
                            MessageBox.Show("Valor invalido: " + "' " + txtBusqueda.Text + "' ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            DataSet dsl = new DataSet();
                            dsl = objdal.BuscarLegajo(Convert.ToInt32(txtBusqueda.Text));
                            DataTable dtAlumnos = dsl.Tables[0]; // representa la tabla de un DataSet
                            dataGridView1.DataSource = dtAlumnos;
                        }
                    }
                }
                else
                {
                    DataSet ds2 = new DataSet();
                    ds2 = objdal.BuscarApeNom(txtBusqueda.Text);
                    DataTable dtAlumnos = ds2.Tables[0];
                    dataGridView1.DataSource = dtAlumnos;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: AL BUSCAR DATOS." + ex.Message, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            txtBusqueda.Text = null;
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtLegajo.Text) || string.IsNullOrEmpty(txtDni.Text) || string.IsNullOrEmpty(txtNombreApellido.Text))
            {
                DialogResult resultado = MessageBox.Show("Debe ingresar datos obligatorios(*)", "Aviso", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (resultado == DialogResult.OK)
                {
                    txtLegajo.Select(0, 0);
                    txtLegajo.Focus();
                }
            }
            else
            {
                string VarConcatena;
                VarConcatena = "" + "\n\r" + "" +
                    "\n\r" + "Legajo: " + txtLegajo.Text +
                    "\n\r" + "Apellido y Nombre: " + txtNombreApellido.Text +
                    "\n\r" + "Nro. Documento: " + txtDni.Text;

                DialogResult resultado = MessageBox.Show("¿Confirma el ingreso de los siguientes datos? " + VarConcatena, "Aviso", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (resultado == DialogResult.OK)
                {
                    string varFoto = txtDni.Text + ".jpg"; //arma el contenido del campo FOTO_JPG
                    string varLegajopdf = txtDni.Text + ".pdf"; //arma el contenido LEGAJO_PDF

                    Dal odjdal = new Dal();

                    odjdal.AgregarAlumno(Convert.ToInt32(txtLegajo.Text), txtNombreApellido.Text, Convert.ToDateTime(dtpFecha.Text), Convert.ToInt32(txtDni.Text), Convert.ToInt32(cboEstadocivil.SelectedValue), txtTelefono.Text, txtCorreo.Text, varFoto, varLegajopdf, txtObservaciones.Text);
                }
                else
                {
                    return;
                }
                Datos.Dal objdal = new Datos.Dal();
                Datosalumnos objAlum = new Datosalumnos();
                DataSet objds = new DataSet();
                objds = objdal.BuscarAlumnos();
                DataTable dtAlumnos = objds.Tables[0];
                dataGridView1.DataSource = dtAlumnos;
            }
        }
        public string VarGuardarNombrePDF;

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //Vincula un DataGridiew con controles del tipo TextBox , Combo y mostrar los datos
            if (e.RowIndex < 0)
            {
                return;
            }

            //Se utiliza una lista para traspasar datos desde el datagridView a los controles
            List<string> objDatos = new List<string>();
            for (int i = 0; i < dataGridView1.Rows[e.RowIndex].Cells.Count; i++)
            {
                objDatos.Add(dataGridView1.Rows[e.RowIndex].Cells[i].Value.ToString());
            }
            //observar la posicion de los campos en la grilla
            //objDatos[0] representa el campo ID de la tabla DATOSALUMNOS 
            txtLegajo.Text = objDatos[1];
            txtNombreApellido.Text = objDatos[2];
            dtpFecha.Text = Convert.ToString(objDatos[3]);
            txtDni.Text = objDatos[4];
            cboEstadocivil.Text = objDatos[5];
            txtTelefono.Text = objDatos[6];
            txtCorreo.Text = objDatos[7];

            //muestra foto del alumno, las imagenes se encuentra en la carpeta C:/FOTOSALUMNOS/
            //el archivo de imagen que contiene la foto tiene el formato DNI.png
            string varFoto = objDatos[8];
            pbxFoto.Image = pbxFoto.InitialImage;
            //Clase Path: muestra la ruta
            //Esta clase hereda de la clase System.Io por tal motivo debe estar referenciado en los using
            string path = Path.Combine(@"C:\FOTOSALUMNOS\", varFoto);
            if (File.Exists(path))
            {
                pbxFoto.Image = System.Drawing.Image.FromFile(path);//muestra imagen enel control pbxFoto

            }
            else
            {
                pbxFoto.Image = Image.FromFile(@"C:\FOTOSALUMNOS\Alumno_SinFoto.jpg");
            }
            VarGuardarNombrePDF = objDatos[9];//guardar info para ser utilizada en el evento click del control
            txtObservaciones.Text = objDatos[10];

        }

        private void pbxPDF_Click(object sender, EventArgs e)
        {
            string varPdf = VarGuardarNombrePDF;
            string v_filename = @"C:\LEGAJOSALUMNOS\" + varPdf;
            if (File.Exists(v_filename))
            {
                System.Diagnostics.Process v_process = System.Diagnostics.Process.Start(v_filename);

            }
            else
            {
                MessageBox.Show("NO Dispone legajo en formato digital", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtLegajo.Text) || string.IsNullOrEmpty(txtNombreApellido.Text) || string.IsNullOrEmpty(txtDni.Text))
            {
                DialogResult resultado = MessageBox.Show("Debe ingresar datos Obligatorios (*)", "Aviso", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

                if (resultado == DialogResult.OK)
                {
                    txtLegajo.Select(0, 0);
                    txtLegajo.Focus();
                }
            }
            else
            {
                string VarConcatena;
                VarConcatena = "" + "\n\r" + "" +
                    "\n\r" + "Legajo: " + txtLegajo.Text +
                    "\n\r" + "Apellido y Nombre: " + txtNombreApellido.Text +
                    "\n\r" + "Nro. Documento: " + txtDni.Text;

                DialogResult resultado = MessageBox.Show("¿Confirma los cambios? " + VarConcatena, "Aviso", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (resultado == DialogResult.OK)
                {
                    string varFoto = txtDni.Text + ".jpg";
                    string varLegajopdf = txtDni.Text + ".pdf";

                    Dal odjdal = new Dal(); // instancio la clase datos
                    odjdal.ModificarAlumno(Convert.ToInt32(txtLegajo.Text), txtNombreApellido.Text, Convert.ToDateTime(dtpFecha.Text), Convert.ToInt32(txtDni.Text), Convert.ToInt32(cboEstadocivil.SelectedValue), txtTelefono.Text, txtCorreo.Text, varFoto, varLegajopdf, txtObservaciones.Text);
                }
                else
                {
                    return;

                }
                Datos.Dal objdal = new Datos.Dal();
                Datosalumnos objAlum = new Datosalumnos();
                DataSet objds = new DataSet();

                objds = objdal.BuscarAlumnos();

                DataTable dtAlumnos = objds.Tables[0];
                dataGridView1.DataSource = dtAlumnos;

            }
        }
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            Datos.Dal objdal = new Datos.Dal();

            DataSet ds1 = new DataSet();
            ds1 = objdal.EliminarAlumno(Convert.ToInt32(txtLegajo.Text));
            MessageBox.Show("Se elimino registro", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);

            Datos.Dal objdatos = new Datos.Dal();
            // Datosalumnos objAlum = new Datosalumnos();
            DataSet objds = new DataSet();

            objds = objdal.BuscarAlumnos();


            DataTable dtAlumnos = objds.Tables[0];
            dataGridView1.DataSource = dtAlumnos;
        }
    }
}
