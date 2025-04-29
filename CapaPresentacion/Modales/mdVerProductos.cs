using CapaEntidad;
using CapaNegocio;
using CapaPresentacion.Utilidades;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapaPresentacion.Modales;

namespace CapaPresentacion.Modales
{
    public partial class mdVerProductos : Form
    {
        public Producto _Producto { get; set; }
        public mdVerProductos()
        {
            InitializeComponent();
        }

        private void mdVerProductos_Load(object sender, EventArgs e)
        {
            List<Categoria> listaCategoria = new CN_Categoria().Listar();

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

            // MOSTRAR SOLO PRODUCTOS ACTIVOS
            List<Producto> listaProducto = new CN_Producto().Listar().Where(p => p.Estado).ToList();

            foreach (Producto item in listaProducto)
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
                    string.Format("{0:N2}", item.Precio), // Formato 1.000,00
                    item.Estado ==  true ? 1 : 0,
                    item.Estado == true ? "Activo" : "No Activo",
                 });
            }

            dgvdata.ForeColor = System.Drawing.Color.Black;
            //dgvdata.BackgroundColor = System.Drawing.Color.FromArgb(240, 240, 240); // Gris claro
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

        private void btnbuscar_Click(object sender, EventArgs e)
        {
            buscar();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------

        private void btnlimpiarbusqueda_Click(object sender, EventArgs e)
        {
            txtbusqueda.Text = "";
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                row.Visible = true;
            }
        }

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

        private void dgvdata_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Verifica si el índice de la columna corresponde al botón
            if (e.RowIndex >= 0 && dgvdata.Columns[e.ColumnIndex] is DataGridViewButtonColumn)
            {
                // Obtiene la fila seleccionada
                DataGridViewRow fila = dgvdata.Rows[e.RowIndex];

                // Crea una instancia del nuevo formulario
                ProductoDetalles detalles = new ProductoDetalles();

                // Pasa los valores de la fila a los TextBox del nuevo formulario
                detalles.txtId.Text = fila.Cells["Codigo"].Value.ToString();
                detalles.txtNombre.Text = fila.Cells["Nombre"].Value.ToString();
                detalles.txtDescripcion.Text = fila.Cells["Descripcion"].Value.ToString();
                detalles.txtCategoria.Text = fila.Cells["Categoria"].Value.ToString();
                detalles.txtStock.Text = fila.Cells["Stock"].Value.ToString();
                detalles.txtPrecio.Text = "$ " + fila.Cells["Precio"].Value.ToString();

                // Muestra el formulario de detalles
                detalles.ShowDialog();
            }
        }

        private void dgvdata_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            if (e.ColumnIndex == 0)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                var w = Properties.Resources.verlistaColornotexto.Width;
                var h = Properties.Resources.verlistaColornotexto.Height;
                var x = e.CellBounds.Left + (e.CellBounds.Width - w)/2;
                var y = e.CellBounds.Top + (e.CellBounds.Height - h)/2;

                e.Graphics.DrawImage(Properties.Resources.verlistaColornotexto, new Rectangle(x, y, w, h));
                e.Handled = true;
            }
        }
    }
}
