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
    public partial class frmCategoria : Form
    {
        private Categoria objcategoria;

        public frmCategoria()
        {
            InitializeComponent();
        }

        private void frmCategoria_Load(object sender, EventArgs e)
        {
            cboestado.Items.Add(new OpcionCombo() { Valor = 1, Texto = "ACTIVO" });
            cboestado.Items.Add(new OpcionCombo() { Valor = 0, Texto = "NO ACTIVO" });
            //Especifica que solo se debe mostrar el dato de nombre "Texto"
            cboestado.DisplayMember = "Texto";
            //Maneja como informacion interna el dato de nombre "Valor"
            cboestado.ValueMember = "Valor";
            //Se selecciona siempre el indice 0
            cboestado.SelectedIndex = 0;

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
            List<Categoria> listaCategoria = new CN_Categoria().Listar();

            foreach (Categoria item in listaCategoria)
            {
                dgvdata.Rows.Add(new object[] {"", item.IdCategoria,item.Descripcion,
                   item.Estado ==  true ? 1 : 0,
                   item.Estado == true ? "Activo" : "No Activo",
               });
            }

            dgvdata.ForeColor = System.Drawing.Color.Black;
        }


        private void buttonGuardar_Click(object sender, EventArgs e)
        {

            string mensaje = string.Empty;
            Categoria objcategoria = new Categoria()
            {
                IdCategoria = Convert.ToInt32(txtid.Text),
                Descripcion = txtdescripcion.Text,


                Estado = Convert.ToInt32(((OpcionCombo)cboestado.SelectedItem).Valor) == 1 ? true : false
            };


            if (objcategoria.IdCategoria == 0)
            {
                //Ejecuta el metodo Registrar de la Clase Usuario en la Cap de Neg con sus respectivos parametros, retornando el idusuario
                int idcategoriagenerado = new CN_Categoria().Registrar(objcategoria, out mensaje);

                if (idcategoriagenerado != 0)
                {
                    dgvdata.Rows.Add(new object[] {"",idcategoriagenerado,txtdescripcion.Text,

                      ((OpcionCombo)cboestado.SelectedItem).Valor.ToString(),
                      ((OpcionCombo)cboestado.SelectedItem).Texto.ToString()

                     });

                    Limpiar();
                }
                else
                {
                    MessageBox.Show(mensaje);
                }

            }
            //Si el objeto idusuario no es igual a 0 se accede a editar el usario seleccionado del datagrid
            else
            {
                bool resultado = new CN_Categoria().Editar(objcategoria, out mensaje);

                if (resultado)
                {
                    //Se obtiene la fila seleccionada en el datagrid
                    DataGridViewRow row = dgvdata.Rows[Convert.ToInt32(txtindice.Text)];
                    //Se realiza el llamado a las filas del datagrid
                    row.Cells["Id"].Value = txtid.Text;
                    row.Cells["Descripcion"].Value = txtdescripcion.Text;

                    row.Cells["EstadoValor"].Value = ((OpcionCombo)cboestado.SelectedItem).Valor.ToString();
                    row.Cells["Estado"].Value = ((OpcionCombo)cboestado.SelectedItem).Texto.ToString();

                    Limpiar();

                }
                else
                {
                    MessageBox.Show(mensaje);
                }
            }


        }


        private void Limpiar()
        {
            // Restablecer colores originales de todas las filas del DataGridView
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                row.DefaultCellStyle.BackColor = dgvdata.DefaultCellStyle.BackColor;
                row.DefaultCellStyle.ForeColor = dgvdata.DefaultCellStyle.ForeColor;
            }

            txtindice.Text = "-1";
            txtid.Text = "0";
            txtdescripcion.Text = "";
            cboestado.SelectedIndex = 0;

            //El foco se va a el textbox documento
            txtdescripcion.Select();
        }

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

        //Boton de DataGrid que trae los datos hacia los TextBox


        private void btneliminar_Click_1(object sender, EventArgs e)
        {
            if (Convert.ToInt32(txtid.Text) != 0)
            {
                if (MessageBox.Show("¿Desea eliminar la Categoria?", "Alerta", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string mensaje = string.Empty;
                    Categoria objcategoria = new Categoria()
                    {
                        IdCategoria = Convert.ToInt32(txtid.Text)
                    };


                    bool respuesta = new CN_Categoria().Eliminar(objcategoria, out mensaje);

                    if (respuesta)
                    {
                        dgvdata.Rows.RemoveAt(Convert.ToInt32(txtindice.Text));
                    }
                    else
                    {
                        MessageBox.Show(mensaje, "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
        }

        private void btnlimpiar_Click_1(object sender, EventArgs e)
        {
            Limpiar();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------

        //------------------------------------------------------------------------------------------------------------------------------------------

        private void btnbuscar_Click_1(object sender, EventArgs e)
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

        private void dgvdata_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvdata.Columns[e.ColumnIndex].Name == "btnseleccionar")
            {
                int indice = e.RowIndex;

                if (indice >=0)
                {
                    // Establecer color blanco a todas las filas para restablecer
                    foreach (DataGridViewRow row in dgvdata.Rows)
                    {
                        row.DefaultCellStyle.BackColor = dgvdata.DefaultCellStyle.BackColor;
                    }

                    dgvdata.Rows[indice].DefaultCellStyle.BackColor = System.Drawing.Color.LightCoral;

                    txtindice.Text = indice.ToString();

                    txtid.Text = dgvdata.Rows[indice].Cells["Id"].Value.ToString();
                    txtdescripcion.Text = dgvdata.Rows[indice].Cells["Descripcion"].Value.ToString();



                    foreach (OpcionCombo oc in cboestado.Items)
                    {
                        if (Convert.ToInt32(oc.Valor) == Convert.ToInt32(dgvdata.Rows[indice].Cells["EstadoValor"].Value))
                        {
                            int indice_combo = cboestado.Items.IndexOf(oc);
                            cboestado.SelectedIndex = indice_combo;
                            break;
                        }
                    }
                }
            }
        }


        private void btnlimpiarbusqueda_Click_1(object sender, EventArgs e)
        {
            txtbusqueda.Text = "";
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                row.Visible = true;
            }
        }

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

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            txtbusqueda.Text = "";
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                row.Visible = true;
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            string mensaje = string.Empty;
            Categoria objcategoria = new Categoria()
            {
                IdCategoria = Convert.ToInt32(txtid.Text),
                Descripcion = txtdescripcion.Text,


                Estado = Convert.ToInt32(((OpcionCombo)cboestado.SelectedItem).Valor) == 1 ? true : false
            };


            if (objcategoria.IdCategoria == 0)
            {
                //Ejecuta el metodo Registrar de la Clase Usuario en la Cap de Neg con sus respectivos parametros, retornando el idusuario
                int idcategoriagenerado = new CN_Categoria().Registrar(objcategoria, out mensaje);

                if (idcategoriagenerado != 0)
                {
                    dgvdata.Rows.Add(new object[] {"",idcategoriagenerado,txtdescripcion.Text,

                      ((OpcionCombo)cboestado.SelectedItem).Valor.ToString(),
                      ((OpcionCombo)cboestado.SelectedItem).Texto.ToString()

                     });

                    Limpiar();
                }
                else
                {
                    MessageBox.Show(mensaje, "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }
            //Si el objeto idusuario no es igual a 0 se accede a editar el usario seleccionado del datagrid
            else
            {
                bool resultado = new CN_Categoria().Editar(objcategoria, out mensaje);

                if (resultado)
                {
                    //Se obtiene la fila seleccionada en el datagrid
                    DataGridViewRow row = dgvdata.Rows[Convert.ToInt32(txtindice.Text)];
                    //Se realiza el llamado a las filas del datagrid
                    row.Cells["Id"].Value = txtid.Text;
                    row.Cells["Descripcion"].Value = txtdescripcion.Text;

                    row.Cells["EstadoValor"].Value = ((OpcionCombo)cboestado.SelectedItem).Valor.ToString();
                    row.Cells["Estado"].Value = ((OpcionCombo)cboestado.SelectedItem).Texto.ToString();

                    Limpiar();

                }
                else
                {
                    MessageBox.Show(mensaje, "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Limpiar();
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(txtid.Text) != 0)
            {
                if (MessageBox.Show("¿Desea eliminar la Categoria?", "Alerta", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string mensaje = string.Empty;
                    Categoria objcategoria = new Categoria()
                    {
                        IdCategoria = Convert.ToInt32(txtid.Text)
                    };


                    bool respuesta = new CN_Categoria().Eliminar(objcategoria, out mensaje);

                    if (respuesta)
                    {
                        dgvdata.Rows.RemoveAt(Convert.ToInt32(txtindice.Text));
                    }
                    else
                    {
                        MessageBox.Show("No es posible eliminar la categoría porque está asociada a uno o más productos. " +
                                        "Elimina todos los productos asociados a esta categoría antes de intentar eliminarla.",
                                        "Alerta",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);
                    }

                }
            }
        }



    }
}
