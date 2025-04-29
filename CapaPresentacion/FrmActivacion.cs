using CapaEntidad;
using CapaNegocio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace CapaPresentacion
{
    public partial class FrmActivacion : Form
    {
        public string CodigoActivacion { get; private set; }

        public FrmActivacion()
        {
            InitializeComponent();

        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtCodigo.Text))
            {
                CodigoActivacion = txtCodigo.Text.Trim();
                DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Por favor, ingrese un código de activación.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btncancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmActivacion_Load(object sender, EventArgs e)
        {
            richTextBox1.Text = " !El período de prueba finalizo!\n\nIngrese el codigo de licencia para seguir usando o comuníquese con el administrador.";

            richTextBox1.SelectionFont = new Font("Arial", 12, FontStyle.Bold);
            richTextBox1.SelectionColor = Color.Red;
            richTextBox1.SelectionAlignment = HorizontalAlignment.Center;





            //-------------------------------------------------------------------------------

            // Obtener la lista de licencias
            List<CE_Licencia> listaLicencia = new CN_AdmLicencia().Listar();

            // Verificar que la lista tenga al menos un elemento
            if (listaLicencia.Count > 0)
            {
                lblFechaInicio.Text = listaLicencia[0].FechaInicio.ToString("dd/MM/yyyy"); // Formato de fecha
            }
            else
            {
                lblFechaInicio.Text = "No hay datos";
            }


        }


    }
}

