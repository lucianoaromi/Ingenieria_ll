using CapaDatos;
using CapaEntidad;
using CapaNegocio;
using CapaPresentacion.Utilidades;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using FontAwesome.Sharp;
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
using DrawingColor = System.Drawing.Color;//---------- para cambiar color el fondo


using SpreadsheetColor = DocumentFormat.OpenXml.Spreadsheet.Color; //------  

namespace CapaPresentacion
{
    public partial class frmReportes : Form
    {
        private Usuario _Usuario;
        private int lastSelectedRow = -1;
        private CN_Reporte ventaNegocio;
        private object cadena;


        public frmReportes(Usuario oUsuario = null)
        {
            _Usuario = oUsuario;
            InitializeComponent();
            dgvdata.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
            dgvdata.RowTemplate.Height = 27;

            groupBox1.Paint += groupBox1_Paint; // Sobrescribe el evento Paint del GroupBox //------
            groupBox2.Paint += groupBox1_Paint;
            groupBox3.Paint += groupBox1_Paint;

            // Centramos las columnas
            dgvdata.Columns["verDetalle"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvdata.Columns["EstadoEntrega"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvdata.Columns["EstadoPago"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            ventaNegocio = new CN_Reporte(cadena);

            dgvdata.CellFormatting += dgvdata_CellFormatting;
        }

        //--------------------------------------------------------------------------------------------------------------------------------

        private void frmReportes_Load(object sender, EventArgs e)
        {
            foreach (DataGridViewColumn columna in dgvdata.Columns)
            {
                if (!(columna is DataGridViewButtonColumn))
                {
                    cbobusqueda.Items.Add(new OpcionCombo() { Valor = columna.Name, Texto = columna.HeaderText });
                }
            }

            cbobusqueda.DisplayMember = "Texto";
            cbobusqueda.ValueMember = "Valor";
            cbobusqueda.SelectedIndex = 0;
            lblidusuario.Text = Convert.ToString(_Usuario.IdUsuario);
            lblidrol.Text = Convert.ToString(_Usuario.oRol.IdRol);
            lblapeusuario.Text = $"{Convert.ToString(_Usuario.Apellido)}, {Convert.ToString(_Usuario.Nombre)}";

            this.BackColor = System.Drawing.Color.FromArgb(42, 47, 58);

            dgvdata.ForeColor = System.Drawing.Color.Black;

            CargarDatosVentas();
        }

        //--------------------------------------------------------------------------------------------------------------------------------

        private void CargarColoresFilas()
        {
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                string estadoEntrega = row.Cells["EstadoEntrega"].Value.ToString();
                string estadoPago = row.Cells["EstadoPago"].Value.ToString();

                // Combinaciones de estados
                if (estadoEntrega == "SI" && estadoPago == "SI")
                {
                    row.DefaultCellStyle.BackColor = System.Drawing.Color.DarkGreen;  // Verde
                    row.DefaultCellStyle.ForeColor = System.Drawing.Color.White;
                }
                else if (estadoEntrega == "NO" && estadoPago == "SI")
                {
                    row.DefaultCellStyle.BackColor = System.Drawing.Color.Orange;  // Naranja
                    row.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
                }
                else if (estadoEntrega == "NO" && estadoPago == "NO")
                {
                    row.DefaultCellStyle.BackColor = System.Drawing.Color.DarkRed;  // Rojo
                    row.DefaultCellStyle.ForeColor = System.Drawing.Color.White;
                }
                else if (estadoEntrega == "SI" && estadoPago == "NO")
                {
                    row.DefaultCellStyle.BackColor = System.Drawing.Color.LightCoral;  // Color por defecto
                    row.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
                }
            }

            // Estilos de selección
            dgvdata.DefaultCellStyle.SelectionBackColor = dgvdata.DefaultCellStyle.BackColor;
            dgvdata.DefaultCellStyle.SelectionForeColor = dgvdata.DefaultCellStyle.ForeColor;
            dgvdata.AlternatingRowsDefaultCellStyle.SelectionBackColor = dgvdata.AlternatingRowsDefaultCellStyle.BackColor;
            dgvdata.AlternatingRowsDefaultCellStyle.SelectionForeColor = dgvdata.AlternatingRowsDefaultCellStyle.ForeColor;
        }

        //--------------------------------------------------------------------------------------------------------------------------------

        private void dgvdata_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            // Cambiar el estado de pago (cuando el usuario hace clic en la columna de EstadoPago)
            if (e.ColumnIndex == dgvdata.Columns["EstadoPago"].Index && e.RowIndex >= 0)
            {
                int idVenta = Convert.ToInt32(dgvdata.Rows[e.RowIndex].Cells["IdVenta"].Value);
                string estadoActual = dgvdata.Rows[e.RowIndex].Cells["EstadoPago"].Value.ToString();
                bool nuevoEstado = (estadoActual == "SI") ? false : true;

                string mensaje;
                bool resultado = ventaNegocio.ActualizarEstadoPago(idVenta, nuevoEstado, out mensaje);

                if (resultado)
                {
                    dgvdata.Rows[e.RowIndex].Cells["EstadoPago"].Value = nuevoEstado ? "SI" : "NO";
                    if (nuevoEstado)
                    {
                        //dgvdata.Rows[e.RowIndex].DefaultCellStyle.BackColor = System.Drawing.Color.Green;
                        dgvdata.Rows[e.RowIndex].DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
                    }
                    else
                    {
                        //dgvdata.Rows[e.RowIndex].DefaultCellStyle.BackColor = System.Drawing.Color.LightCoral;
                        dgvdata.Rows[e.RowIndex].DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
                    }
                    //MessageBox.Show("Estado de pago actualizado correctamente.");
                }
                else
                {
                    MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            // Cambiar el estado de entrega (cuando el usuario hace clic en la columna de EstadoEntrega)
            if (e.ColumnIndex == dgvdata.Columns["EstadoEntrega"].Index && e.RowIndex >= 0)
            {
                int idVenta = Convert.ToInt32(dgvdata.Rows[e.RowIndex].Cells["IdVenta"].Value);
                string estadoActual = dgvdata.Rows[e.RowIndex].Cells["EstadoEntrega"].Value.ToString();
                bool nuevoEstado = (estadoActual == "SI") ? false : true;

                string mensaje;
                bool resultado = ventaNegocio.ActualizarEstadoVenta(idVenta, nuevoEstado, out mensaje);

                if (resultado)
                {
                    dgvdata.Rows[e.RowIndex].Cells["EstadoEntrega"].Value = nuevoEstado ? "SI" : "NO";
                    if (nuevoEstado)
                    {
                        //dgvdata.Rows[e.RowIndex].DefaultCellStyle.BackColor = System.Drawing.Color.Green;
                        dgvdata.Rows[e.RowIndex].DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
                    }
                    else
                    {
                        //dgvdata.Rows[e.RowIndex].DefaultCellStyle.BackColor = System.Drawing.Color.LightCoral;
                        dgvdata.Rows[e.RowIndex].DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
                    }
                    //MessageBox.Show("Estado de entrega actualizado correctamente.");
                }
                else
                {
                    MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            CargarColoresFilas();

            // Verifica si se hizo clic en una celda válida y si corresponde a la columna del botón
            if (e.RowIndex >= 0 && e.ColumnIndex == dgvdata.Columns["verDetalle"].Index)
            {
                if (lastSelectedRow >= 0)
                {
                    // Restablece el color de la última fila seleccionada
                    dgvdata.Rows[lastSelectedRow].DefaultCellStyle.BackColor = dgvdata.DefaultCellStyle.BackColor;
                    dgvdata.Rows[lastSelectedRow].DefaultCellStyle.ForeColor = dgvdata.DefaultCellStyle.ForeColor;
                }

                // Obtén el valor de la tercera columna de la fila seleccionada
                string valorColumna = dgvdata.Rows[e.RowIndex].Cells[3].Value.ToString();

                // Crea una instancia del formulario Form2
                frmDetalleVenta form2 = new frmDetalleVenta();

                // Pasa el valor al TextBox del segundo formulario
                form2.txtbusqueda.Text = valorColumna;

                // Opcional: Si necesitas actualizar colores al cargar filas
                CargarColoresFilas();

                // Resalta la fila seleccionada
                dgvdata.Rows[e.RowIndex].DefaultCellStyle.BackColor = System.Drawing.Color.Cyan;
                dgvdata.Rows[e.RowIndex].DefaultCellStyle.ForeColor = System.Drawing.Color.Black;

                // Actualiza el índice de la última fila seleccionada
                lastSelectedRow = e.RowIndex;

                // Muestra el segundo formulario
                form2.Show();
            }

            //CargarDatosVentas();
        }

        //--------------------------------------------------------------------------------------------------------------------------------

        private void CargarDatosVentas()
        {
            List<ReporteVenta> lista = new List<ReporteVenta>();
            int idusuario = Convert.ToInt32(lblidusuario.Text);
            string fechaInicio = txtfechainicio.Value.ToString("dd/MM/yyyy");
            string fechaFin = txtfechafin.Value.ToString("dd/MM/yyyy");

            lista = new CN_Reporte().Venta(fechaInicio, fechaFin, idusuario);

            lista = lista
                .OrderBy(rv =>
                    rv.EstadoEntrega == "NO" && rv.EstadoPago == "NO" ? 1 :
                    rv.EstadoEntrega == "SI" && rv.EstadoPago == "NO" ? 2 :
                    rv.EstadoEntrega == "NO" && rv.EstadoPago == "SI" ? 3 :
                    4) // Para "EstadoEntrega" == "SI" && "EstadoPago" == "SI"
                .ThenBy(rv => rv.FechaRegistro) // Orden adicional por fecha si hay empates
                .ToList();


            dgvdata.Rows.Clear();

            foreach (ReporteVenta rv in lista)
            {
                if (lblidrol.Text == "2" || lblapeusuario.Text == rv.UsuarioRegistro)
                {
                    _=dgvdata.Rows.Add(new object[]
                    {
                        null, // Placeholder
                        rv.FechaRegistro,
                        rv.TipoDocumento,
                        rv.NumeroDocumento,
                        Convert.ToDecimal(rv.MontoTotal).ToString("N2", System.Globalization.CultureInfo.GetCultureInfo("es-ES")), // Formato correcto
                        rv.UsuarioRegistro,
                        rv.ApellidoCliente,
                        rv.DesMetPago,
                        rv.EstadoPago,
                        rv.EstadoEntrega,
                        rv.IdVenta,
                    });
                }
            }

            CargarColoresFilas();

            SumarColumnaFilasVisibles(dgvdata, "MontoTotal", txtTotal);
            ContarFilasVisibles(dgvdata, txtResultado);

        }

        //--------------------------------------------------------------------------------------------------------------------------------

        private void button2_Click(object sender, EventArgs e)
        {
            CargarDatosVentas();
        }

        //--------------------------------------------------------------------------------------------------------------------------------

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

        //--------------------------------------------------------------------------------------------------------------------------------

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

        //--------------------------------------------------------------------------------------------------------------------------------

        private void picFiltrar_Click(object sender, EventArgs e)
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

            SumarColumnaFilasVisibles(dgvdata, "MontoTotal", txtTotal);
            ContarFilasVisibles(dgvdata, txtResultado);

        }

        //--------------------------------------------------------------------------------------------------------------------------------

        private void picBuscarFecha_Click(object sender, EventArgs e)
        {
            CargarDatosVentas();
        }

        private void picOrdenarColor_Click(object sender, EventArgs e)
        {
            CargarDatosVentas();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            CargarDatosVentas();
        }

        //--------------------------------------------------------------------------------------------------------------------------------

        // Generar EXCEL
        private void button3_Click(object sender, EventArgs e)
        {
            if (dgvdata.Rows.Count < 1)
            {
                MessageBox.Show("No hay registros para exportar", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            DataTable dt = new DataTable();

            // Excluir la primera y las dos últimas columnas
            for (int i = 1; i < dgvdata.Columns.Count - 3; i++)
            {
                dt.Columns.Add(dgvdata.Columns[i].HeaderText, typeof(string));
            }

            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                if (row.Visible)
                {
                    List<object> valores = new List<object>();

                    // Excluir valores de la primera y las dos últimas columnas
                    for (int i = 1; i < dgvdata.Columns.Count - 3; i++)
                    {
                        valores.Add(row.Cells[i].Value?.ToString() ?? "");
                    }

                    dt.Rows.Add(valores.ToArray());
                }
            }

            SaveFileDialog savefile = new SaveFileDialog();
            savefile.FileName = string.Format("ReporteVentas_{0}.xlsx", DateTime.Now.ToString("ddMMyyyyHHmmss"));
            savefile.Filter = "Excel Files | *.xlsx";

            if (savefile.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    XLWorkbook wb = new XLWorkbook();
                    var hoja = wb.Worksheets.Add(dt, "Informe");
                    wb.SaveAs(savefile.FileName);
                    MessageBox.Show("Reporte Generado", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch
                {
                    MessageBox.Show("Error al generar reporte", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }

        }

        //--------------------------------------------------------------------------------------------------------------------------------

        private void SumarColumnaFilasVisibles(DataGridView dgvdata, string MontoTotal, TextBox txtResultado)
        {
            try
            {
                // Inicializar la suma en 0
                decimal suma = 0;

                // Iterar por todas las filas visibles del DataGridView
                foreach (DataGridViewRow row in dgvdata.Rows)
                {
                    // Verificar si la fila es visible y si la celda tiene un valor válido
                    if (row.Visible && row.Cells[MontoTotal].Value != null)
                    {
                        string valorCelda = row.Cells[MontoTotal].Value.ToString().Replace(".", "").Replace(",", ".");

                        // Intentar convertir el valor de la celda a decimal
                        if (decimal.TryParse(valorCelda, out decimal valor))
                        {
                            suma += valor;
                        }
                    }
                }

                // Mostrar el resultado en el TextBox con formato de moneda
                txtResultado.Text = "$ " + suma.ToString("#,##0.00", System.Globalization.CultureInfo.GetCultureInfo("es-ES"));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al sumar la columna: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------

        private void ContarFilasVisibles(DataGridView dgvdata, TextBox txtResultado)
        {
            try
            {
                // Contar las filas visibles
                int contador = dgvdata.Rows.Cast<DataGridViewRow>().Count(row => row.Visible);

                // Mostrar el resultado en el TextBox
                txtResultado.Text = contador.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al contar las filas visibles: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------

        private void dgvdata_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvdata.Columns[e.ColumnIndex] is DataGridViewButtonColumn)
            {
                e.CellStyle.BackColor = System.Drawing.Color.White; // No pintar la columna de botones
            }
            else
            {
                //e.CellStyle.BackColor = System.Drawing.Color.LightGray; // Pintar el resto
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------

        private void groupBox1_Paint(object sender, PaintEventArgs e)  //------
        {
            GroupBox box = sender as GroupBox;
            if (box != null)
            {
                int borderWidth = 1;
                System.Drawing.Color borderColor = System.Drawing.Color.White; // Personalizar color del borde

                // Dibujar el fondo
                e.Graphics.Clear(this.BackColor);

                // Dibujar el borde
                using (Pen pen = new Pen(borderColor, borderWidth))
                {
                    e.Graphics.DrawRectangle(pen, box.ClientRectangle.X, box.ClientRectangle.Y + 6,
                                              box.ClientRectangle.Width -1, box.ClientRectangle.Height - 8);
                }

                // Dibujar el texto
                TextRenderer.DrawText(
                    e.Graphics,
                    box.Text,
                    box.Font,
                    new Point(box.ClientRectangle.X + 10, box.ClientRectangle.Y),
                    box.ForeColor,
                    TextFormatFlags.Default);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------
    }
}


