using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//Hace referencia a la carpeta "Utilidades" para el uso de la clase "OpcionCombo"
using CapaPresentacion.Utilidades;

using CapaEntidad;
using CapaNegocio;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Globalization;

namespace CapaPresentacion
{
    public partial class frmClientes : Form
    {
        private Cliente objcliente;
        internal object _Cliente;

        public frmClientes()
        {
            InitializeComponent();
        }

        private void frmClientes_Load(object sender, EventArgs e)
        {
            cboestado1.Items.Add(new OpcionCombo() { Valor = 1, Texto = "Activo" });
            cboestado1.Items.Add(new OpcionCombo() { Valor = 0, Texto = "No Activo" });
            //Especifica que solo se debe mostrar el dato de nombre "Texto"
            cboestado1.DisplayMember = "Texto";
            //Maneja como informacion interna el dato de nombre "Valor"
            cboestado1.ValueMember = "Valor";
            //Se selecciona siempre el indice 0
            cboestado1.SelectedIndex = 0;


            foreach (DataGridViewColumn columna in dgvdata.Columns)
            {
                if (columna.Visible == true && columna.Name != "btnseleccionar")
                {
                    cbobusqueda.Items.Add(new OpcionCombo() { Valor = columna.Name, Texto = columna.HeaderText });
                }
            }

            // Configurar las propiedades después del bucle
            cbobusqueda.DisplayMember = "Texto";
            cbobusqueda.ValueMember = "Valor";
            cbobusqueda.SelectedIndex = 0;


            //MOSTRAR TODOS LOS USARIOS
            List<Cliente> listaCliente = new CN_Cliente().Listar();

            foreach (Cliente item in listaCliente)
            {
                dgvdata.Rows.Add(new object[] {"", item.IdCliente,item.Apellido,item.Nombre,item.Direccion,item.Telefono,
                   item.Estado ==  true ? 1 : 0,
                   item.Estado == true ? "Activo" : "No Activo",
               });
            }
            
            txtapellido1.Focus();
            txtapellido1.TabIndex = 0;
            txtnombre1.TabIndex = 1;
            txtdireccion1.TabIndex = 2;
            txttelefono1.TabIndex = 3;
            cboestado1.TabIndex = 4;

            dgvdata.ForeColor = System.Drawing.Color.Black;

        }

        //------------------------------------------------------------------------------------------------------------------------------------------

        private void Limpiar()
        {
            // Restablecer colores originales de todas las filas del DataGridView
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                row.DefaultCellStyle.BackColor = dgvdata.DefaultCellStyle.BackColor;
                row.DefaultCellStyle.ForeColor = dgvdata.DefaultCellStyle.ForeColor;
            }

            // Limpiar los campos de texto y combo box
            txtindicecliente.Text = "-1";
            txtidcliente.Text = "0";
            //txtdocumento1.Text = "";
            txtapellido1.Text = "";
            txtnombre1.Text = "";
            txtdireccion1.Text = "";
            //txtcorreo1.Text = "";
            txttelefono1.Text = "";
            cboestado1.SelectedIndex = 0;

            // Establecer el foco en el TextBox documento
            txtapellido1.Select();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------

        //Muestra la imagen de tilde en el DataGrid
        private void dgvdata_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            if (e.ColumnIndex == 0)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                var w = Properties.Resources.editaricon.Width;
                var h = Properties.Resources.editaricon.Height;
                var x = e.CellBounds.Left + (e.CellBounds.Width - w)/2;
                var y = e.CellBounds.Top + (e.CellBounds.Height - h)/2;

                e.Graphics.DrawImage(Properties.Resources.editaricon, new Rectangle(x, y, w, h));
                e.Handled = true;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------

        // Método para normalizar y eliminar los acentos
        private string NormalizarTexto(string texto)
        {
            string normalizedString = texto.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            foreach (char c in normalizedString)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------

        private void dgvdata_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvdata.Columns[e.ColumnIndex].Name == "btnseleccionar")
            {
                int indice = e.RowIndex;

                if (indice >= 0)
                {
                    // Establecer color blanco a todas las filas para restablecer
                    foreach (DataGridViewRow row in dgvdata.Rows)
                    {
                        row.DefaultCellStyle.BackColor = dgvdata.DefaultCellStyle.BackColor;
                    }

                    // Marcar la fila seleccionada con color rojo
                    dgvdata.Rows[indice].DefaultCellStyle.BackColor = System.Drawing.Color.LightCoral;

                    // Asignar valores a los controles de texto
                    txtindicecliente.Text = indice.ToString();
                    txtidcliente.Text = dgvdata.Rows[indice].Cells["Id"].Value.ToString();
                    //txtdocumento1.Text = dgvdata.Rows[indice].Cells["Documento"].Value.ToString();
                    txtapellido1.Text = dgvdata.Rows[indice].Cells["Apellido"].Value.ToString();
                    txtnombre1.Text = dgvdata.Rows[indice].Cells["Nombre"].Value.ToString();
                    txtdireccion1.Text = dgvdata.Rows[indice].Cells["Direccion"].Value.ToString();
                    //txtcorreo1.Text = dgvdata.Rows[indice].Cells["Correo"].Value.ToString();
                    txttelefono1.Text = dgvdata.Rows[indice].Cells["Telefono"].Value.ToString();

                    // Seleccionar el estado correspondiente en el combo box
                    foreach (OpcionCombo oc in cboestado1.Items)
                    {
                        if (Convert.ToInt32(oc.Valor) == Convert.ToInt32(dgvdata.Rows[indice].Cells["EstadoValor"].Value))
                        {
                            int indice_combo = cboestado1.Items.IndexOf(oc);
                            cboestado1.SelectedIndex = indice_combo;
                            break;
                        }
                    }
                }
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------

        private void dgvdata_CellPainting_1(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            if (e.ColumnIndex == 0)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                var w = Properties.Resources.editaricon.Width;
                var h = Properties.Resources.editaricon.Height;
                var x = e.CellBounds.Left + (e.CellBounds.Width - w)/2;
                var y = e.CellBounds.Top + (e.CellBounds.Height - h)/2;

                e.Graphics.DrawImage(Properties.Resources.editaricon, new Rectangle(x, y, w, h));
                e.Handled = true;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            string columnaFiltro = ((OpcionCombo)cbobusqueda.SelectedItem).Valor.ToString();
            string busquedaNormalizada = NormalizarTexto(txtbusqueda.Text.Trim().ToUpper());

            if (dgvdata.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvdata.Rows)
                {
                    if (row.Cells[columnaFiltro].Value != null)
                    {
                        string cellValue = row.Cells[columnaFiltro].Value.ToString().Trim().ToUpper();
                        string cellValueNormalizada = NormalizarTexto(cellValue);

                        if (cellValueNormalizada.Contains(busquedaNormalizada))
                        {
                            row.Visible = true;
                        }
                        else
                        {
                            row.Visible = false;
                        }
                    }
                }
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            txtbusqueda.Text = "";
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                row.Visible = true;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            string mensaje = string.Empty;
            Cliente objcliente = new Cliente()
            {
                IdCliente = Convert.ToInt32(txtidcliente.Text),
                //Documento = txtdocumento1.Text,
                Apellido = txtapellido1.Text,
                Nombre = txtnombre1.Text,
                Direccion = txtdireccion1.Text,
                //Correo = txtcorreo1.Text,
                Telefono = txttelefono1.Text,


                Estado = Convert.ToInt32(((OpcionCombo)cboestado1.SelectedItem).Valor) == 1 ? true : false
            };

            if (objcliente.IdCliente == 0)
            {
                //Ejecuta el metodo Registrar de la Clase Usuario en la Cap de Neg con sus respectivos parametros, retornando el idusuario
                int idclientegenerado = new CN_Cliente().Registrar(objcliente, out mensaje);

                if (idclientegenerado != 0)
                {
                    dgvdata.Rows.Add(new object[] {"",idclientegenerado,txtapellido1.Text,txtnombre1.Text,txtdireccion1.Text,txttelefono1.Text,

                       ((OpcionCombo)cboestado1.SelectedItem).Valor.ToString(),
                       ((OpcionCombo)cboestado1.SelectedItem).Texto.ToString()
                    });

                    Limpiar();
                }
                else
                {
                    MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }

            }
            //Si el objeto idusuario no es igual a 0 se accede a editar el usario seleccionado del datagrid
            else
            {
                bool resultado = new CN_Cliente().Editar(objcliente, out mensaje);

                if (resultado)
                {
                    //Se obtiene la fila seleccionada en el datagrid
                    DataGridViewRow row = dgvdata.Rows[Convert.ToInt32(txtindicecliente.Text)];
                    //Se realiza el llamado a las filas del datagrid
                    row.Cells["Id"].Value = txtidcliente.Text;
                    //row.Cells["Documento"].Value = txtdocumento1.Text;
                    row.Cells["Apellido"].Value = txtapellido1.Text;
                    row.Cells["Nombre"].Value = txtnombre1.Text;
                    row.Cells["Direccion"].Value = txtdireccion1.Text;
                    row.Cells["Telefono"].Value = txttelefono1.Text;
                    //row.Cells["Correo"].Value = txtcorreo1.Text;

                    row.Cells["EstadoValor"].Value = ((OpcionCombo)cboestado1.SelectedItem).Valor.ToString();
                    row.Cells["Estado"].Value = ((OpcionCombo)cboestado1.SelectedItem).Texto.ToString();

                    Limpiar();

                }
                else
                {
                    MessageBox.Show(mensaje, "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Limpiar();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(txtidcliente.Text) != 0)
            {
                if (MessageBox.Show("¿Desea eliminar el usuario?", "Alerta", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string mensaje = string.Empty;
                    Cliente objcliente = new Cliente()
                    {
                        IdCliente = Convert.ToInt32(txtidcliente.Text)
                    };


                    bool respuesta = new CN_Cliente().Eliminar(objcliente, out mensaje);

                    if (respuesta)
                    {
                        dgvdata.Rows.RemoveAt(Convert.ToInt32(txtindicecliente.Text));
                    }
                    else
                    {
                        MessageBox.Show(mensaje, "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------
    }
}
