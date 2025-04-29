using CapaPresentacion.Models;
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
    public partial class frmGraficos : Form
    {
        //Fields
        private Dashboard model;

        //Constructor
        public frmGraficos()
        {
            InitializeComponent();
            //Default - Last 7 days
            dtpStartDate.Value = DateTime.Today.AddDays(-7);
            dtpEndDate.Value = DateTime.Now;
            //btnLast7Days.Select();
            dgvUnderstock.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
            model = new Dashboard();
            LoadData();

            // Cambia el color de las etiquetas de los ejes
            chartGrossRevenue.ChartAreas[0].AxisX.LabelStyle.ForeColor = System.Drawing.Color.Turquoise;
            chartGrossRevenue.ChartAreas[0].AxisY.LabelStyle.ForeColor = System.Drawing.Color.Turquoise;
            txtPorcentaje.Focus();
        }

        //-------------------------------------------------------------------------

        //Private methods
        private void LoadData()
        {
            
            var refreshData = model.LoadData(dtpStartDate.Value, dtpEndDate.Value);
            if (refreshData == true)
            {
                lblNumOrders.Text = model.NumOrders.ToString();
                lblTotalRevenue2.Text = model.TotalRevenue.ToString();
                lblTotalRevenue.Text = "$" + model.TotalRevenue.ToString("#,##0.00", new System.Globalization.CultureInfo("es-ES"));
                lblTotalProfit.Text = "$" + model.TotalProfit.ToString("#,##0.00", new System.Globalization.CultureInfo("es-ES"));

                lblNumCustomers.Text = model.NumCustomers.ToString();
                //lblNumSuppliers.Text = model.NumSuppliers.ToString();
                lblNumProducts.Text = model.NumProducts.ToString();

                // Asignar los datos al gráfico como lo estás haciendo
                chartGrossRevenue.DataSource = model.GrossRevenueList;
                chartGrossRevenue.Series[0].XValueMember = "Date";
                chartGrossRevenue.Series[0].YValueMembers = "TotalAmount";
                chartGrossRevenue.DataBind();

                // Formatear los valores del eje Y con el formato deseado
                chartGrossRevenue.ChartAreas[0].AxisY.LabelStyle.Format = "#,##0.00"; // Formato con separadores de miles y decimales
                chartGrossRevenue.ChartAreas[0].AxisY.LabelStyle.Format = "#,##0.00"; // Esto usará los separadores correctos según la cultura regional (es-ES)

                chartTopProducts.DataSource = model.TopProductsList;
                chartTopProducts.Series[0].XValueMember = "Key";
                chartTopProducts.Series[0].YValueMembers = "Value";
                chartTopProducts.DataBind();

                dgvUnderstock.DataSource = model.UnderstockList;
                dgvUnderstock.Columns[0].HeaderText = "Producto";
                dgvUnderstock.Columns[1].HeaderText = "Unidades";
                dgvUnderstock.Columns[0].Width = 250;
                dgvUnderstock.Columns[1].Width = 60;
                Console.WriteLine("Loaded view :)");
                calcularPorcentaje();
                txtPorcentaje.Focus();


                // Cambia el color de fondo del área de plot
                chartTopProducts.ChartAreas[0].BackColor = Color.FromArgb(42, 47, 58); // El índice [0] hace referencia al primer área de gráfico
                // Cambia el color de fondo del área de plot
                chartTopProducts.ChartAreas[0].BackColor = Color.FromArgb(42, 47, 58); // El índice [0] hace referencia al primer área de gráfico

                //ººººººººººººººººººººººººººººººººººººººº
                // Fondo general del Chart
                chartTopProducts.BackColor = Color.FromArgb(42, 47, 58);

                // Fondo del área de gráficos
                chartTopProducts.ChartAreas[0].BackColor = Color.FromArgb(42, 47, 58);

                // Cambiar el color del texto de los ejes
                chartTopProducts.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.White;
                chartTopProducts.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.White;

                // Cambiar el color de los títulos de los ejes
                chartTopProducts.ChartAreas[0].AxisX.TitleForeColor = Color.White;
                chartTopProducts.ChartAreas[0].AxisY.TitleForeColor = Color.White;

                // Cambiar el color del título general del gráfico
                chartTopProducts.Titles[0].ForeColor = Color.White;

                // Cambiar el color de la leyenda (si existe)
                if (chartTopProducts.Legends.Count > 0)
                {
                    chartTopProducts.Legends[0].ForeColor = Color.White;
                    chartTopProducts.Legends[0].BackColor = Color.FromArgb(42, 47, 58); // Fondo de la leyenda
                }

                // Cambiar el color de la leyenda (si existe)
                if (chartTopProducts.Legends.Count > 0)
                {
                    //chartGrossRevenue.Legends[0].ForeColor = Color.White;
                    chartGrossRevenue.Legends[0].BackColor = Color.FromArgb(42, 47, 58); // Fondo de la leyenda
                }

                //ººººººººººººººººººººººººººººººººººººººº

            }
            else Console.WriteLine("Vista no cargada, misma consulta");
        }

        //-------------------------------------------------------------------------

        private void DisableCustomDates()
        {
            dtpStartDate.Enabled = false;
            dtpEndDate.Enabled = false;
            btnOkCustomDate.Visible = false;
        }

        //-------------------------------------------------------------------------

        private void btnOkCustomDate_Click_1(object sender, EventArgs e)
        {
            LoadData();
            calcularPorcentaje();
        }

        //-------------------------------------------------------------------------

        private void btnCustomDate_Click_1(object sender, EventArgs e)
        {
            dtpStartDate.Enabled = true;
            dtpEndDate.Enabled = true;
            btnOkCustomDate.Visible = true;
            calcularPorcentaje();
        }

        //-------------------------------------------------------------------------

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            dtpStartDate.Value = DateTime.Today;
            dtpEndDate.Value = DateTime.Now;
            LoadData();
            DisableCustomDates();
            calcularPorcentaje();
        }

        //-------------------------------------------------------------------------

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            dtpStartDate.Value = DateTime.Today.AddDays(-7);
            dtpEndDate.Value = DateTime.Now;
            LoadData();
            DisableCustomDates();
            calcularPorcentaje();
        }

        //-------------------------------------------------------------------------

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            dtpStartDate.Value = DateTime.Today.AddDays(-30);
            dtpEndDate.Value = DateTime.Now;
            LoadData();
            DisableCustomDates();
            calcularPorcentaje();
        }

        //-------------------------------------------------------------------------

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            dtpStartDate.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            dtpEndDate.Value = DateTime.Now;
            LoadData();
            DisableCustomDates();
            calcularPorcentaje();
        }

        private void calcularPorcentaje()
        {
            // Si necesitas mostrar el valor como un número decimal con formato específico
            decimal valor;
            if (decimal.TryParse(lblTotalRevenue2.Text, out valor))
            {
                txtValor1.Text = valor.ToString("N2"); // Muestra el valor con dos decimales
            }
            else
            {
                MessageBox.Show("El valor en el Label no es un número válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


            // Obtener el valor del primer TextBox (como decimal)
            decimal valor1;
            if (decimal.TryParse(txtValor1.Text, out valor1))
            {
                // Verificar si el segundo TextBox está vacío o no tiene un valor válido
                int porcentaje = 0; // Asumir 0 como valor por defecto
                if (!string.IsNullOrWhiteSpace(txtPorcentaje.Text) && int.TryParse(txtPorcentaje.Text, out int parsedPorcentaje))
                {
                    porcentaje = parsedPorcentaje;
                }

                // Realizar la operación: valor1 * (porcentaje / 100)
                decimal resultado = valor1 * (porcentaje / 100m);

                // Mostrar el resultado en el Label
                label1.Text = "$" + resultado.ToString("#,##0.00", new System.Globalization.CultureInfo("es-ES"));
            }
            else
            {
                MessageBox.Show("El valor en el primer TextBox no es válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            txtPorcentaje.Focus();

        }

        private void txtPorcentaje_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Verificar si la tecla presionada no es un dígito, no es backspace y no es coma o punto decimal
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
            {
                // Cancelar el evento para evitar que se registre la tecla
                e.Handled = true;
            }
        }
    }
}
