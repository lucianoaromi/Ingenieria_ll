using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapaPresentacion.Modales;
using SpreadsheetColor = DocumentFormat.OpenXml.Spreadsheet.Color; //------
using DrawingColor = System.Drawing.Color;//---------- para cambiar color el fondo

namespace CapaPresentacion.Modales
{
    public partial class ProductoDetalles : Form
    {
        public ProductoDetalles()
        {
            InitializeComponent();
            groupBox2.Paint += groupBox1_Paint; // Sobrescribe el evento Paint del GroupBox //------
        }

        public void FormDetalles_Load(object sender, EventArgs e)
        {
            txtId.ReadOnly = true;
            txtNombre.ReadOnly = true;
            txtDescripcion.ReadOnly = true;
            txtCategoria.ReadOnly = true;
            txtStock.ReadOnly = true;
            txtPrecio.ReadOnly = true;

            txtId.TabStop = false; // Evitar que sea accesible por tabulación
            txtNombre.TabStop = false; // Evitar que sea accesible por tabulación
            txtDescripcion.TabStop = false; // Evitar que sea accesible por tabulación
            txtCategoria.TabStop = false; // Evitar que sea accesible por tabulación
            txtStock.TabStop = false; // Evitar que sea accesible por tabulación
            txtPrecio.TabStop = false; // Evitar que sea accesible por tabulación
            this.ActiveControl = null; // Ningún control tiene el foco al inicio

            this.BackColor = System.Drawing.Color.FromArgb(42, 47, 58); //---------- para cambiar color el fondo
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
    }
}
