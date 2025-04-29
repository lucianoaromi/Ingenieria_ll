using CapaEntidad;
using CapaNegocio;
using CapaPresentacion.Modales;
using CapaPresentacion.Utilidades;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Org.BouncyCastle.Math.Primes;
using SpreadsheetColor = DocumentFormat.OpenXml.Spreadsheet.Color; //------ para los cuadros 
using DrawingColor = System.Drawing.Color;
using System.Data.SqlClient;
using CapaDatos; //---------- para cambiar color el fondo

namespace CapaPresentacion
{
    public partial class frmVentas : Form
    {
        //Obtiene el usuario
        private Usuario _Usuario;

        public frmVentas(Usuario oUsuario = null)
        {
            _Usuario = oUsuario;
            InitializeComponent();
            dgvdata.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
            //dgvdata.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.Gray; // Cambia el color de fondo de las filas alternas
            txtidproducto.Text = "0";
            txtcantidad.Value = 1;

            groupBox1.Paint += groupBox1_Paint; // Sobrescribe el evento Paint del GroupBox //------
            groupBox2.Paint += groupBox1_Paint;
            groupBox3.Paint += groupBox1_Paint;
            groupBox4.Paint += groupBox1_Paint;

        }

        //-----------------------------------------------------------

        private void limpiarproducto()
        {
            txtidproducto.Text = "0";
            txtcodigoproducto.Text = "";
            txtnombreproducto.Text = "";
            txtpreciounidad.Text = "";
            txtstock.Text = "";
            txtcantidad.Value = 1;
        }

        //-----------------------------------------------------------

        private void frmVentas_Load(object sender, EventArgs e)
        {
            cbotipodocumento.Items.Add(new OpcionCombo() { Valor = "Consum. final", Texto = "Consum. final" });
            cbotipodocumento.Items.Add(new OpcionCombo() { Valor = "Factura B", Texto = "Factura B" });
            cbotipodocumento.DisplayMember = "Texto";
            cbotipodocumento.ValueMember = "Valor";
            cbotipodocumento.SelectedIndex = 0;

            txtfecha.Text = DateTime.Now.ToString("dd/MM/yyyy");


            cbometodopago.Items.Add(new OpcionCombo() { Valor = 0, Texto = "Tarj. Credito" });
            cbometodopago.Items.Add(new OpcionCombo() { Valor = 1, Texto = "Tarj. Debito" });
            cbometodopago.Items.Add(new OpcionCombo() { Valor = 2, Texto = "Efectivo" });
            cbometodopago.Items.Add(new OpcionCombo() { Valor = 3, Texto = "Tensferencia" });
            cbometodopago.Items.Add(new OpcionCombo() { Valor = 4, Texto = "QR" });
            //Especifica que solo se debe mostrar el dato de nombre "Texto"
            cbometodopago.DisplayMember = "Texto";
            //Maneja como informacion interna el dato de nombre "Valor"
            cbometodopago.ValueMember = "Valor";
            //Se selecciona siempre el indice 0
            cbometodopago.SelectedIndex = 0;
            txtidcliente.Focus();

            this.BackColor = System.Drawing.Color.FromArgb(42, 47, 58); //---------- para cambiar color el fondo
            dgvdata.ForeColor = System.Drawing.Color.Black;
            dgvdata.DefaultCellStyle.BackColor = System.Drawing.Color.White;
        }

        //-----------------------------------------------------------

        private void LimpiarCliente()
        {
            // Limpiar los TextBox después de agregar los datos
            txtnombrecliente.Clear();
            txtapellidocliente.Clear();
            txtidcliente.Clear();
            txtidcliente.Focus();
        }

        //-----------------------------------------------------------

        private void dgvdata_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            {
                if (e.RowIndex < 0)
                {
                    return;
                }

                if (e.ColumnIndex == 5)
                {
                    e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                    var w = Properties.Resources.verde.Width;
                    var h = Properties.Resources.verde.Height;
                    var x = e.CellBounds.Left + (e.CellBounds.Width - w)/2;
                    var y = e.CellBounds.Top + (e.CellBounds.Height - h)/2;

                    e.Graphics.DrawImage(Properties.Resources.rojo, new Rectangle(x, y, w, h));
                    e.Handled = true;
                }
            }
        }

        //-----------------------------------------------------------

        private void dgvdata_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvdata.Columns[e.ColumnIndex].Name== "btneliminar")
            {
                int index = e.RowIndex;
                if (index >= 0)
                {
                    bool respuesta = new CN_Venta().SumarStock(
                         Convert.ToInt32(dgvdata.Rows[index].Cells["IdProducto"].Value.ToString()),                        
                         Convert.ToInt32(dgvdata.Rows[index].Cells["Cantidad"].Value.ToString()));

                    if (respuesta)
                    {
                        dgvdata.Rows.RemoveAt(index);
                        calcularTotal();
                    }
                    
                    
                }
            }
        }

        //-----------------------------------------------------------

        private void txtpagacon_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        //-----------------------------------------------------------

        private void txtpagacon_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                calcularcambio();
            }
        }

        //-----------------------------------------------------------

        private void picBuscarCliente_Click(object sender, EventArgs e)
        {
            using (var modal = new mdClientes())
            {
                var result = modal.ShowDialog();

                if (result == DialogResult.OK)
                {
                    txtidcliente.Text = modal._Cliente.IdCliente.ToString();
                    //txtdocumentocliente.Text = modal._Cliente.Documento.ToString();
                    txtnombrecliente.Text = modal._Cliente.Nombre.ToString();
                    txtapellidocliente.Text = modal._Cliente.Apellido;

                }
                else
                {
                    txtcodigoproducto.Select();
                }
            }
        }

        //-----------------------------------------------------------

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            LimpiarCliente();
        }

        //-----------------------------------------------------------

        private void picBuscarProducto_Click(object sender, EventArgs e)
        {
            using (var modal = new mdProducto())
            {
                var result = modal.ShowDialog();

                if (result == DialogResult.OK)
                {
                    txtidproducto.Text = modal._Producto.IdProducto.ToString();
                    txtcodigoproducto.Text = modal._Producto.Codigo.ToString();
                    txtnombreproducto.Text = modal._Producto.Nombre;
                    txtstock.Text = modal._Producto.Stock.ToString();

                    // Formateo del precio con separador de miles y decimales
                    txtpreciounidad.Text = modal._Producto.Precio.ToString("#,0.00");

                    txtcantidad.Select();
                }
                else
                {
                    txtcodigoproducto.Select();
                }
            }

        }

        //-----------------------------------------------------------

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            limpiarproducto();
        }

        //-----------------------------------------------------------

        private void calcularcambio()
        {
            // Mostrar los valores para depurar
            //MessageBox.Show("Total: " + txttotalpagar.Text + " Pago: " + txtpagacon.Text);

            if (txttotalpagar.Text.Trim() == "")
            {
                MessageBox.Show("No existen productos en la venta", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            decimal pagacon;
            decimal total;

            if (!decimal.TryParse(txttotalpagar.Text.Trim(), out total))
            {
                MessageBox.Show("El total a pagar no es un valor válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (txtpagacon.Text.Trim() == "")
            {
                txtpagacon.Text = "0.00";
            }

            if (decimal.TryParse(txtpagacon.Text.Trim(), out pagacon))
            {
                if (pagacon < total)
                {
                    txtcambio.Text = "0.00";
                }
                else
                {
                    decimal cambio = pagacon - total;
                    txtcambio.Text = cambio.ToString("#,##0.00");
                }
            }
            else
            {
                MessageBox.Show("El pago ingresado no es un valor válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }





        //-----------------------------------------------------------

        private void calcularTotal()
        {
            decimal total = 0;
            if (dgvdata.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvdata.Rows)
                    total += Convert.ToDecimal(row.Cells["SubTotal"].Value.ToString());
            }
            txttotalpagar.Text = total.ToString("#,0.00");


            // Intenta convertir el valor del primer TextBox a decimal
            
            if (decimal.TryParse(txttotalpagar.Text, out decimal valor))
            {
                // Formatea el valor con separadores de miles y coma como separador decimal
                txtOutput.Text = "$ " + valor.ToString("#,##0.00", new System.Globalization.CultureInfo("es-ES"));
            }
        }

        //-----------------------------------------------------------

        private void picAgregarProducto_Click(object sender, EventArgs e)
        {
            decimal precio = 0;
            bool producto_existe = false;
            

            // Verifica que se seleccione un producto
            if (int.Parse(txtidproducto.Text) == 0)
            {
                MessageBox.Show("Debe seleccionar un producto", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Para el formado de moneda sea correcto
            if (!decimal.TryParse(txtpreciounidad.Text, out precio))
            {
                MessageBox.Show("Precio - Formato de número incorrecto", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtpreciounidad.Select();
                return;
            }

            // Verifica si la cantidad es mayor al stock disponible o si es 0
            if (Convert.ToInt32(txtcantidad.Value) > Convert.ToInt32(txtstock.Text) || Convert.ToInt32(txtcantidad.Value) == 0)
            {
                MessageBox.Show("La cantidad no es válida", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Analiza si el producto existe
            foreach (DataGridViewRow fila in dgvdata.Rows)
            {
                if (fila.Cells["IdProducto"].Value.ToString() == txtidproducto.Text)
                {
                    producto_existe = true;
                    break;
                }
            }

            if (!producto_existe)
            {
                bool respuesta = new CN_Venta().RestarStock(
                    Convert.ToInt32(txtidproducto.Text),
                    Convert.ToInt32(txtcantidad.Value.ToString())
                );

                if (respuesta)
                {
                    // Agrega la condición para verificar si el stock es mayor a 0 antes de agregar al DataGridView
                    if (Convert.ToInt32(txtstock.Text) > 0)
                    {
                        decimal totalProducto = txtcantidad.Value * precio;

                        // Se añade el producto al DataGridView
                        dgvdata.Rows.Add(new object[]
                        {
                            txtidproducto.Text,
                            txtnombreproducto.Text,
                            precio.ToString("#,0.00"), // Formato de moneda con separadores de miles y decimales
                            txtcantidad.Value.ToString(),
                            totalProducto.ToString("#,0.00") // Formato de moneda para el total del producto
                        });

                        calcularTotal();  // Llamada a tu método para recalcular si es necesario
                        limpiarproducto();
                        txtcodigoproducto.Select();
                    }
                    else
                    {
                        MessageBox.Show("No se puede agregar un producto con stock 0", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }

        }



        private void txtcodigoproducto_KeyDown(object sender, KeyEventArgs e)
        {
            // Verifica si la tecla presionada es 'Enter'
            if (e.KeyCode == Keys.Enter)
            {
                // Llama a la función para obtener el producto con el código ingresado
                string codigoProducto = txtcodigoproducto.Text.Trim();

                // Verifica que el código no esté vacío
                if (string.IsNullOrEmpty(codigoProducto))
                {
                    MessageBox.Show("Debe ingresar un código de producto", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // Busca el producto en la base de datos
                Producto producto = ObtenerProductoPorCodigo(codigoProducto);
                if (producto == null)
                {
                    MessageBox.Show("Producto no encontrado", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // Agregar el producto encontrado al DataGridView
                AgregarProductoAlDataGridView(producto);
            }
        }

        private Producto ObtenerProductoPorCodigo(string codigo)
        {
            string query = "SELECT IdProducto, Codigo, Nombre, Precio, Stock FROM Producto WHERE Codigo = @Codigo";

            using (SqlConnection conn = new SqlConnection(Conexion.cadena))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Codigo", codigo);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Producto
                            {
                                IdProducto = Convert.ToInt32(reader["IdProducto"]),
                                Codigo = reader["Codigo"].ToString(),
                                Nombre = reader["Nombre"].ToString(),
                                Precio = Convert.ToDecimal(reader["Precio"]),
                                Stock = Convert.ToInt32(reader["Stock"])
                            };
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }

        private void AgregarProductoAlDataGridView(Producto producto)
        {
            bool producto_existe = false;
            foreach (DataGridViewRow fila in dgvdata.Rows)
            {
                if (fila.Cells["IdProducto"].Value.ToString() == producto.IdProducto.ToString())
                {
                    producto_existe = true;
                    break;
                }
            }

            if (!producto_existe)
            {
                int cantidad = Convert.ToInt32(txtcantidad.Value);

                if (cantidad > producto.Stock || cantidad == 0)
                {
                    MessageBox.Show("La cantidad no es válida o excede el stock disponible.", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                decimal subtotal = cantidad * producto.Precio;

                // Resta el stock del producto en la base de datos
                bool respuesta = new CN_Venta().RestarStock(producto.IdProducto, cantidad);

                if (respuesta)
                {
                    dgvdata.Rows.Add(new object[]
                    {
                producto.IdProducto,
                producto.Nombre,
                producto.Precio.ToString("#,0.00"),
                cantidad.ToString(),
                subtotal.ToString("#,0.00")
                    });

                    calcularTotal();
                }
                else
                {
                    MessageBox.Show("Error al actualizar el stock.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            limpiarproducto();
        }





        //-----------------------------------------------------------

        private void picComprar_Click(object sender, EventArgs e)
        {
            if (txtapellidocliente.Text == "")
            {
                MessageBox.Show("Debe ingresar Apellido del cliente.", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (txtnombrecliente.Text == "")
            {
                MessageBox.Show("Debe ingresar Nombre del cliente.", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (dgvdata.Rows.Count < 1)
            {
                MessageBox.Show("La lista de compra está vacía.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataTable detalle_venta = new DataTable();
            detalle_venta.Columns.Add("IdProducto", typeof(int));
            detalle_venta.Columns.Add("Precio", typeof(decimal));
            detalle_venta.Columns.Add("Cantidad", typeof(int));
            detalle_venta.Columns.Add("SubTotal", typeof(decimal));

            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                detalle_venta.Rows.Add(new object[]
                {
            row.Cells["IdProducto"].Value.ToString(),
            row.Cells["Precio"].Value.ToString(),
            row.Cells["Cantidad"].Value.ToString(),
            row.Cells["SubTotal"].Value.ToString()
                });
            }

            int idcorrelativo = new CN_Venta().ObtenerCorrelativo();
            string numeroDocumento = string.Format("{0:00000}", idcorrelativo);
            calcularcambio();

            Venta oVenta = new Venta()
            {
                oUsuario = new Usuario() { IdUsuario = _Usuario.IdUsuario },
                TipoDocumento = ((OpcionCombo)cbotipodocumento.SelectedItem).Texto,
                NumeroDocumento = numeroDocumento,
                DocumentoCliente = txtidcliente.Text,
                ApellidoCliente = txtapellidocliente.Text,
                NombreCliente = txtnombrecliente.Text,
                MontoPago = Convert.ToDecimal(txtpagacon.Text),
                MontoCambio = Convert.ToDecimal(txtcambio.Text),
                MontoTotal = Convert.ToDecimal(txttotalpagar.Text),
                DesMetPago = ((OpcionCombo)cbometodopago.SelectedItem).Texto,
            };

            string mensaje = string.Empty;
            bool respuesta = new CN_Venta().Registrar(oVenta, detalle_venta, out mensaje);

            if (respuesta)
            {
                var result = MessageBox.Show(
                    "Número de venta generado: \n" + numeroDocumento + "\n\n¿Desea generar el PDF?",
                    "Mensaje",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information
                );

                if (result == DialogResult.Yes)
                {
                    // Llama al código para generar e imprimir el PDF
                    frmDetalleVenta form2 = new frmDetalleVenta();
                    form2.txtbusqueda.Text = numeroDocumento; // Pasa el número de venta al segundo formulario
                    form2.TopMost = true; // Mantiene la ventana en primer plano
                    form2.Show();
                    form2.Activate(); // Forzar el enfoque en la nueva ventana
                }

                // Limpiar campos y restablecer la interfaz
                txtapellidocliente.Text = "";
                txtnombrecliente.Text = "";
                txtidcliente.Text = "";
                dgvdata.Rows.Clear();
                calcularTotal();
                txtpagacon.Text = "";
                txtcambio.Text = "";
                txtnumventa.Text = numeroDocumento.ToString();
                txtidcliente.Focus();
            }
            else
            {
                MessageBox.Show(mensaje, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }



        //-----------------------------------------------------------

        private void picPDF_Click(object sender, EventArgs e)
        {
            // Verifica si el TextBox está vacío
            if (string.IsNullOrEmpty(txtnumventa.Text))
            {
                MessageBox.Show("No se ha registrado ninguna venta para mostrar", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {

                // Crea una instancia del segundo formulario
                frmDetalleVenta form2 = new frmDetalleVenta();

                // Pasa el contenido del TextBox de Form1 al TextBox de Form2
                form2.txtbusqueda.Text = txtnumventa.Text;

                // Muestra el segundo formulario
                form2.Show();
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------

        private void groupBox1_Paint(object sender, PaintEventArgs e)  //------
        {
            GroupBox box = sender as GroupBox;
            if (box != null)
            {
                int borderWidth = 1;
                System.Drawing.Color borderColor = System.Drawing.Color.Silver; // Personalizar color del borde
                //System.Drawing.Color borderColor = System.Drawing.Color.FromArgb(64, 64, 64);


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

        private void btnpdf_Click(object sender, EventArgs e)
        {
            calcularcambio();
        }

        private void txtpagacon_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            // Permitir solo números, el punto decimal y la tecla de retroceso
            if (!Char.IsControl(e.KeyChar) && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Si se presiona un carácter no permitido, lo ignora
            }

            // Evitar que se ingrese más de un punto decimal
            if (e.KeyChar == '.' && (sender as TextBox).Text.Contains("."))
            {
                e.Handled = true; // Si ya hay un punto, no permite otro
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            frmClientes nuevoFormulario = new frmClientes(); // Crea una instancia del otro formulario
            nuevoFormulario.Show(); // Muestra el formulario en una nueva ventana
        }

        private void btnlimpiarcompra_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                int idProducto = Convert.ToInt32(row.Cells["IdProducto"].Value.ToString());
                int cantidad = Convert.ToInt32(row.Cells["Cantidad"].Value.ToString());

                // Llama a la función para devolver el stock
                new CN_Venta().SumarStock(idProducto, cantidad);
            }

            // Limpia el DataGridView después de reponer el stock
            dgvdata.Rows.Clear();
            // Limpiar campos y restablecer la interfaz
            txtapellidocliente.Text = "";
            txtnombrecliente.Text = "";
            txtidcliente.Text = "";
            dgvdata.Rows.Clear();
            calcularTotal();
            txtpagacon.Text = "";
            txtcambio.Text = "";
            txtidcliente.Focus();
        }

        //--------------------------------------------------------------------------------------------------------------------------------

    }
}
