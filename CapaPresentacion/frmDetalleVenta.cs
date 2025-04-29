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

using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System.IO;

using SpreadsheetColor = DocumentFormat.OpenXml.Spreadsheet.Color; //------ para los cuadros
using System.Globalization; //------
using DrawingColor = System.Drawing.Color; //---------- para cambiar color el fondo


namespace CapaPresentacion
{
    public partial class frmDetalleVenta : Form
    {

        public string TextBoxValue
        {
            get { return txtbusqueda.Text; }
            set { txtbusqueda.Text = value; }
        }


        public frmDetalleVenta()
        {
            InitializeComponent();

            groupBox1.Paint += groupBox1_Paint; // Sobrescribe el evento Paint del GroupBox //------
            groupBox2.Paint += groupBox1_Paint;
            groupBox4.Paint += groupBox1_Paint;
        }

        //--------------------------------------------------------------------------------------------------------------------------------

        private void frmDetalleVenta_Load(object sender, EventArgs e)
        {
            txtfecha.ReadOnly = true;
            txttipodocumento.ReadOnly = true;
            txtusuarioapellido.ReadOnly = true;
            txtusuarionombre.ReadOnly = true;
            txtdoccliente.ReadOnly = true;
            txtapellidocliente.ReadOnly = true;
            txtnombrecliente.ReadOnly = true;
            txtmontototal.ReadOnly = true;
            txtmontopago.ReadOnly = true;
            txtmontocambio.ReadOnly = true;
            txtmetodopago.ReadOnly = true;

            txtbusqueda.Select();

            Venta oVenta = new CN_Venta().ObtenerVenta(txtbusqueda.Text);

            if (oVenta.IdVenta != 0)
            {
                txtnumerodocumento.Text = oVenta.NumeroDocumento;

                // Informacion del Vendedor
                txtfecha.Text = oVenta.FechaRegistro;
                txttipodocumento.Text = oVenta.TipoDocumento;
                txtusuarioapellido.Text = oVenta.oUsuario.Apellido;
                txtusuarionombre.Text = oVenta.oUsuario2.Nombre;

                // Informacion del Cliente
                txtdoccliente.Text = oVenta.DocumentoCliente;
                txtapellidocliente.Text = oVenta.ApellidoCliente;
                txtnombrecliente.Text = oVenta.NombreCliente;

                // Lista de productos vendidos
                dgvdata.Rows.Clear();
                foreach (Detalle_Venta dv in oVenta.oDetalle_Venta)
                {
                    dgvdata.Rows.Add(new object[] {
                        dv.oProducto.Nombre,
                        dv.Precio.ToString("N2"),
                        dv.Cantidad,
                        dv.SubTotal.ToString("N2")
                    });
                }

                // Informacion de los distintos montos
                txtmontototal.Text = oVenta.MontoTotal.ToString("#,0.00"); // Formato con separador de miles
                txtmontopago.Text = oVenta.MontoPago.ToString("#,0.00"); // Formato con separador de miles
                txtmontocambio.Text = oVenta.MontoCambio.ToString("#,0.00"); // Formato con separador de miles
                txtmetodopago.Text = oVenta.DesMetPago;

                // Desmarca cualquier fila seleccionada
                dgvdata.ClearSelection();

                this.BackColor = System.Drawing.Color.FromArgb(42, 47, 58); //---------- para cambiar color el fondo
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------

        private void btnpdf_Click_1(object sender, EventArgs e)
        {
            SaveFileDialog savefile = new SaveFileDialog();

            //savefile.FileName = string.Format("{0}.pdf", DateTime.Now.ToString("ddMMyyyyHHmmss"));
            savefile.FileName = string.Format("{0}.pdf", DateTime.Now.ToString("dd-MM-yyyy_(HHmmss)"));

            string PaginaHTML_Texto = Properties.Resources.Plantilla.ToString();
            PaginaHTML_Texto = PaginaHTML_Texto.Replace("@NRODOCUMENTO", txtnumerodocumento.Text);
            PaginaHTML_Texto = PaginaHTML_Texto.Replace("@CLIENTEA", txtapellidocliente.Text);
            PaginaHTML_Texto = PaginaHTML_Texto.Replace("@CLIENTEN", txtnombrecliente.Text);
            PaginaHTML_Texto = PaginaHTML_Texto.Replace("@DOCUMENTO", txtdoccliente.Text);
            PaginaHTML_Texto = PaginaHTML_Texto.Replace("@FECHA", DateTime.Now.ToString("dd/MM/yyyy"));
            PaginaHTML_Texto = PaginaHTML_Texto.Replace("@METPAGO", txtmetodopago.Text);

            string filas = string.Empty;
            decimal total = 0;
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                filas += "<tr>";
                filas += "<td>" + row.Cells["Producto"].Value.ToString() + "</td>";

                // Formateo de Precio y Subtotal
                string precioFormatted = Convert.ToDecimal(row.Cells["Precio"].Value).ToString("#,0.00").Replace(",", ".");
                filas += "<td>" + precioFormatted + "</td>";

                filas += "<td>" + row.Cells["Cantidad"].Value.ToString() + "</td>";

                // Formateo de SubTotal
                string subTotalFormatted = Convert.ToDecimal(row.Cells["SubTotal"].Value).ToString("#,0.00").Replace(",", ".");
                filas += "<td>" + subTotalFormatted + "</td>";

                filas += "</tr>";
                total += Convert.ToDecimal(row.Cells["SubTotal"].Value);
            }


            // Formato de miles y decimales para el total
            string totalFormatted = total.ToString("#,0.00", CultureInfo.InvariantCulture);  // 1,000.50

            // Reemplazar la coma por el punto decimal (en caso de querer usar coma como separador de decimales)
            totalFormatted = totalFormatted.Replace(",", ".");
            //totalFormatted = totalFormatted.Replace(".", ","); // Esto cambiará el punto por la coma en el formato

            PaginaHTML_Texto = PaginaHTML_Texto.Replace("@TOTAL", totalFormatted);



            PaginaHTML_Texto = PaginaHTML_Texto.Replace("@FILAS", filas);
            //PaginaHTML_Texto = PaginaHTML_Texto.Replace("@TOTAL", total.ToString());

            if (savefile.ShowDialog() == DialogResult.OK)
            {
                using (FileStream stream = new FileStream(savefile.FileName, FileMode.Create))
                {
                    //Creamos un nuevo documento y lo definimos como PDF
                    Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 25);

                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                    pdfDoc.Open();
                    pdfDoc.Add(new Phrase(""));

                    //Agregamos la imagen del banner al documento
                    iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(Properties.Resources.ArduinoLogo, System.Drawing.Imaging.ImageFormat.Png);
                    img.ScaleToFit(60, 60);
                    img.Alignment = iTextSharp.text.Image.UNDERLYING;

                    img.SetAbsolutePosition(10, 100);
                    img.SetAbsolutePosition(pdfDoc.LeftMargin, pdfDoc.Top - 60);
                    pdfDoc.Add(img);

                    //pdfDoc.Add(new Phrase("Hola Mundo"));
                    using (StringReader sr = new StringReader(PaginaHTML_Texto))
                    {
                        XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                    }

                    pdfDoc.Close();
                    stream.Close();
                }

            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            Venta oVenta = new CN_Venta().ObtenerVenta(txtbusqueda.Text);

            if (oVenta.IdVenta != 0)
            {
                txtnumerodocumento.Text = oVenta.NumeroDocumento;

                // Información del Vendedor
                txtfecha.Text = oVenta.FechaRegistro;
                txttipodocumento.Text = oVenta.TipoDocumento;
                txtusuarioapellido.Text = oVenta.oUsuario.Apellido;
                txtusuarionombre.Text = oVenta.oUsuario2.Nombre;

                // Información del Cliente
                txtdoccliente.Text = oVenta.DocumentoCliente;
                txtapellidocliente.Text = oVenta.ApellidoCliente;
                txtnombrecliente.Text = oVenta.NombreCliente;

                // Lista de productos vendidos
                dgvdata.Rows.Clear();
                foreach (Detalle_Venta dv in oVenta.oDetalle_Venta)
                {
                    dgvdata.Rows.Add(new object[] { dv.oProducto.Nombre, dv.Precio, dv.Cantidad, dv.SubTotal });
                }

                // Restablecer colores de todas las filas
                foreach (DataGridViewRow row in dgvdata.Rows)
                {
                    row.DefaultCellStyle.BackColor = dgvdata.DefaultCellStyle.BackColor;
                    row.DefaultCellStyle.ForeColor = dgvdata.DefaultCellStyle.ForeColor;
                }

                // Deseleccionar la primera fila
                if (dgvdata.Rows.Count > 0)
                {
                    dgvdata.ClearSelection();
                }

                // Información de los distintos montos
                txtmontototal.Text = oVenta.MontoTotal.ToString("#,0.00");
                txtmontopago.Text = oVenta.MontoPago.ToString("#,0.00");
                txtmontocambio.Text = oVenta.MontoCambio.ToString("#,0.00");
                txtmetodopago.Text = oVenta.DesMetPago;
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            txtbusqueda.Text = "";
            txtnumerodocumento.Text = "";
            txtfecha.Text = "";
            txttipodocumento.Text = "";
            txtusuarioapellido.Text = "";

            txtdoccliente.Text = "";
            txtapellidocliente.Text = "";
            dgvdata.Rows.Clear();
            txtmontototal.Text = "0.00";
            txtmontopago.Text = "0.00";
            txtmontocambio.Text = "0.00";
            txtmetodopago.Text = "";
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
