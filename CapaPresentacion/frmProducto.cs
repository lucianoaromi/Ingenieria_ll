using CapaEntidad;
using CapaNegocio;
using CapaPresentacion.Utilidades;
using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using CapaDatos;

namespace CapaPresentacion
{

    public partial class frmProducto : Form
    {

        public frmProducto()
        {
            InitializeComponent();

        }

        private void frmProducto_Load(object sender, EventArgs e)
        {
            CargarProductos();
        }

        //---------------------------------------------------------------------------------------------------------------

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
            txtcodigo.Text = "";
            txtnombre.Text = "";
            txtdescripcion.Text = "";
            cbocategoria.SelectedIndex = 0;
            cboestado.SelectedIndex = 0;
            txtstock.Text = "0";
            txtprecio.Text = "0";
            //El foco se va a el textbox codigo
            txtcodigo.Select();
        }

        //---------------------------------------------------------------------------------------------------------------

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

        //---------------------------------------------------------------------------------------------------------------

        private void buscar()
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

        //---------------------------------------------------------------------------------------------------------------

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

        //---------------------------------------------------------------------------------------------------------------

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

                    txtcodigo.Text = dgvdata.Rows[indice].Cells["Codigo"].Value.ToString();
                    txtnombre.Text = dgvdata.Rows[indice].Cells["Nombre"].Value.ToString();
                    txtdescripcion.Text = dgvdata.Rows[indice].Cells["Descripcion"].Value.ToString();

                    txtstock.Text = dgvdata.Rows[indice].Cells["Stock"].Value.ToString();
                    txtprecio.Text = dgvdata.Rows[indice].Cells["Precio"].Value.ToString();

                    foreach (OpcionCombo oc in cbocategoria.Items)
                    {
                        if (Convert.ToInt32(oc.Valor) == Convert.ToInt32(dgvdata.Rows[indice].Cells["IdCategoria"].Value))
                        {
                            int indice_combo = cbocategoria.Items.IndexOf(oc);
                            cbocategoria.SelectedIndex = indice_combo;
                            break;
                        }
                    }


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

        //---------------------------------------------------------------------------------------------------------------

        private void txtstock_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Verifica si el carácter ingresado no es un número ni un carácter de control (como Backspace)
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Cancela la entrada del carácter
            }
        }

        //---------------------------------------------------------------------------------------------------------------

        private void txtprecio_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // Permite números, el punto decimal (.) y el signo negativo (-)
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != '-' && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }

            // Asegúrate de que solo haya un punto decimal
            if (e.KeyChar == '.' && textBox.Text.Contains("."))
            {
                e.Handled = true;
            }

            // Asegúrate de que el signo negativo solo esté al inicio
            if (e.KeyChar == '-' && textBox.SelectionStart != 0)
            {
                e.Handled = true;
            }
        }

        //---------------------------------------------------------------------------------------------------------------

        private void txtbusqueda_KeyPress(object sender, KeyPressEventArgs e)
        {
            buscar();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            buscar();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            txtbusqueda.Text = "";
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                row.Visible = true;
            }
        }

        //---------------------------------------------------------------------------------------------------------------

        private void pictureBox2_Click_1(object sender, EventArgs e)
        {
            //-----------------------------------------------
            // Verificar si el campo txtstock y txtprecio esten vacíos
            if (string.IsNullOrEmpty(txtstock.Text))
            {
                MessageBox.Show("Es necesario el 'Stock' del producto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Detener la ejecución si el campo está vacío
            }

            if (string.IsNullOrEmpty(txtprecio.Text))
            {
                MessageBox.Show("Es necesario el 'Precio' del producto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Detener la ejecución si el campo 'Precio' está vacío
            }
            //-----------------------------------------------

            string mensaje = string.Empty;
            Producto obj = new Producto()
            {
                IdProducto = Convert.ToInt32(txtid.Text),
                Codigo = txtcodigo.Text,
                Nombre = txtnombre.Text,
                Descripcion = txtdescripcion.Text,
                Stock = Convert.ToInt32(txtstock.Text),
                Precio = Convert.ToDecimal(txtprecio.Text),
                oCategoria = new Categoria() { IdCategoria = Convert.ToInt32(((OpcionCombo)cbocategoria.SelectedItem).Valor) },
                Estado = Convert.ToInt32(((OpcionCombo)cboestado.SelectedItem).Valor) == 1 ? true : false
            };

            if (obj.IdProducto == 0)
            {
                //Ejecuta el metodo Registrar de la Clase Producto en la Cap de Neg con sus respectivos parametros, retornando el idproducto
                int idgenerado = new CN_Producto().Registrar(obj, out mensaje);

                if (idgenerado != 0)
                {
                    dgvdata.Rows.Add(new object[] {
                        "",
                        idgenerado,
                        txtcodigo.Text,
                        txtnombre.Text,
                        txtdescripcion.Text,
                        ((OpcionCombo)cbocategoria.SelectedItem).Valor.ToString(),
                        ((OpcionCombo)cbocategoria.SelectedItem).Texto.ToString(),
                        txtstock.Text,
                        string.Format("{0:N2}", obj.Precio), // Formatear el precio como pesos

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
            //Si el objeto idproducto no es igual a 0 se accede a editar el producto seleccionado del datagrid
            else
            {
                bool resultado = new CN_Producto().Editar(obj, out mensaje);

                if (resultado)
                {
                    //Se obtiene la fila seleccionada en el datagrid
                    DataGridViewRow row = dgvdata.Rows[Convert.ToInt32(txtindice.Text)];
                    //Se realiza el llamado a las filas del datagrid
                    row.Cells["Id"].Value = txtid.Text;
                    row.Cells["Codigo"].Value = txtcodigo.Text;
                    row.Cells["Nombre"].Value = txtnombre.Text;
                    row.Cells["Descripcion"].Value = txtdescripcion.Text;
                    row.Cells["IdCategoria"].Value = ((OpcionCombo)cbocategoria.SelectedItem).Valor.ToString();
                    row.Cells["Categoria"].Value = ((OpcionCombo)cbocategoria.SelectedItem).Texto.ToString();

                    row.Cells["Stock"].Value = txtstock.Text;
                    row.Cells["Precio"].Value = string.Format("{0:N2}", obj.Precio); // Formatear el precio como pesos

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

        //---------------------------------------------------------------------------------------------------------------

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {
            Limpiar();
        }

        //---------------------------------------------------------------------------------------------------------------

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(txtid.Text) != 0)
            {
                if (MessageBox.Show("¿Desea eliminar el Producto?", "Alerta", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string mensaje = string.Empty;
                    Producto objproducto = new Producto()
                    {
                        IdProducto = Convert.ToInt32(txtid.Text)
                    };


                    bool respuesta = new CN_Producto().Eliminar(objproducto, out mensaje);

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

        //---------------------------------------------------------------------------------------------------------------

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            // Validar que el TextBox tenga un valor numérico válido
            if (!decimal.TryParse(txtPorcentaje.Text, out decimal porcentaje) || porcentaje <= 0)
            {
                MessageBox.Show("Ingrese un porcentaje válido mayor a 0.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Convertir el porcentaje a decimal (Ej: si el usuario ingresa 10, se convierte a 1.10)
            decimal factor = 1 + (porcentaje / 100);

            // Lista para almacenar los ID de los productos visibles en el DataGridView
            List<int> productosFiltrados = new List<int>();

            // Recorrer solo las filas visibles del DataGridView y obtener sus ID
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                if (row.Visible) // Solo toma las filas que están visibles
                {
                    int idProducto = Convert.ToInt32(row.Cells["Id"].Value);
                    productosFiltrados.Add(idProducto);
                }
            }

            // Si hay productos filtrados, ejecutar la actualización
            if (productosFiltrados.Count > 0)
            {
                using (SqlConnection conn = new SqlConnection(Conexion.cadena))
                {
                    conn.Open();

                    // Construir la consulta dinámica para actualizar solo los productos visibles
                    string query = $"UPDATE Producto SET Precio = Precio * @Factor WHERE IdProducto IN ({string.Join(",", productosFiltrados)})";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Factor", factor);
                        cmd.ExecuteNonQuery();
                    }
                }

                // Recargar los productos actualizados en el DataGridView
                CargarProductos();

                // Mostrar mensaje de éxito
                MessageBox.Show($"Los precios se han actualizado en un {porcentaje}% correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("No hay productos visibles para actualizar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        //---------------------------------------------------------------------------------------------------------------

        private void CargarProductos()
        {
            dgvdata.Rows.Clear(); // 🔹 Limpia los datos previos para evitar datos desactualizados

            // Cargar estados en ComboBox
            cboestado.Items.Clear();
            cboestado.Items.Add(new OpcionCombo() { Valor = 1, Texto = "Activo" });
            cboestado.Items.Add(new OpcionCombo() { Valor = 0, Texto = "No Activo" });
            cboestado.DisplayMember = "Texto";
            cboestado.ValueMember = "Valor";
            cboestado.SelectedIndex = 0;

            txtstock.Text = "0";
            txtprecio.Text = "0";

            // Cargar categorías
            cbocategoria.Items.Clear();
            List<Categoria> listaCategoria = new CN_Categoria().Listar();
            foreach (Categoria item in listaCategoria)
            {
                cbocategoria.Items.Add(new OpcionCombo() { Valor = item.IdCategoria, Texto = item.Descripcion });
            }
            cbocategoria.DisplayMember = "Texto";
            cbocategoria.ValueMember = "Valor";
            cbocategoria.SelectedIndex = 0;

            // Cargar opciones de búsqueda
            cbobusqueda.Items.Clear();
            foreach (DataGridViewColumn columna in dgvdata.Columns)
            {
                if (columna.Visible == true && columna.Name != "btnseleccionar")
                {
                    cbobusqueda.Items.Add(new OpcionCombo() { Valor = columna.Name, Texto = columna.HeaderText });
                }
            }
            cbobusqueda.DisplayMember = "Texto";
            cbobusqueda.ValueMember = "Valor";
            cbobusqueda.SelectedIndex = 0;

            // 🔹 Obtener productos actualizados
            List<Producto> lista = new CN_Producto().Listar();
            foreach (Producto item in lista)
            {
                dgvdata.Rows.Add(new object[] {
                    "",
                    item.IdProducto,
                    item.Codigo,
                    item.Nombre,
                    item.Descripcion,
                    item.oCategoria.IdCategoria,
                    item.oCategoria.Descripcion,
                    item.Stock,
                    item.Precio, // 🔹 Aquí ya estará reflejado el nuevo precio
                    item.Estado == true ? 1 : 0,
                    item.Estado == true ? "Activo" : "No Activo",
                });
            }

            dgvdata.ForeColor = System.Drawing.Color.Black;
        }

    }
}
