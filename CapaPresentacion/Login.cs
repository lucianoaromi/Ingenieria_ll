using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using CapaNegocio;
using CapaEntidad;


namespace CapaPresentacion
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void btncancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btningresar_Click(object sender, EventArgs e)
        {
            // Obteniendo una lista de usuarios y lo almacena en una lista de tipo Usuario llamada TEST
            List<Usuario> TEST = new CN_Usuario().Listar();

            try
            {
                // Verifica si los valores en los TextBox son "ADMIN"
                if (txtdocumento.Text == "ADMINLICENCIA123" && txtclave.Text == "ADMINLICENCIA123")
                {
                    // Si son "ADMIN", abre el formulario nuevo
                    frmAdmLicencia formAdmin = new frmAdmLicencia();
                    formAdmin.Show();
                    // Ocultar el formulario actual (Login).
                    this.Hide();
                    // Permite cerrar el formulario actual cuando se cierra el formulario de administración
                    formAdmin.FormClosing += Frm_closing;
                }
                else
                {
                    // Devuelve solo una clase de la lista de clases, los cuales tienen documento y clave iguales a los que se encuentran en los TextBoxs
                    Usuario ousuario = TEST.Where(l => l.Documento == txtdocumento.Text).FirstOrDefault();

                    // Si se encuentra resultados
                    if (ousuario != null)
                    {
                        // Verifica la contraseña hashada
                        bool passwordMatches = BCrypt.Net.BCrypt.EnhancedVerify(txtclave.Text, ousuario.Clave);

                        if (passwordMatches)
                        {
                            // Llama al formulario Inicio y se le envía el parámetro objeto "ousuario"
                            Inicio form = new Inicio(ousuario);
                            form.Show();
                            // Ocultar el formulario actual (Login).
                            this.Hide();
                            // Permite cerrar el formulario actual cuando se cierra el formulario de inicio
                            form.FormClosing += Frm_closing;
                        }
                        else
                        {
                            MessageBox.Show("Usuario o contraseña incorrecta", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    }
                    else
                    {
                        MessageBox.Show("No se encontró el usuario", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error inesperado: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Frm_closing(object sender, FormClosingEventArgs e)
        {
            //Al volver al formulario de "Login" borra los datos de los textbox
            txtdocumento.Text = "";
            txtclave.Text = "";
            //Mostrar el propio formulario
            this.Show();
        }

        private void Login_Load(object sender, EventArgs e)
        {

        }
    }
}
