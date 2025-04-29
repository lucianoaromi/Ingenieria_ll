using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FontAwesome.Sharp;

namespace CapaPresentacion
{
    public class RoundedButton : Button
    {
        private Color originalBackColor;

        private void OnMouseEnter(object sender, EventArgs e)
        {
            // Cambia el color de fondo cuando el cursor está sobre el botón
            this.BackColor = Color.Gray; // Puedes ajustar el color según tus preferencias
        }

        private void OnMouseLeave(object sender, EventArgs e)
        {
            // Restaura el color de fondo al valor original cuando el cursor sale del botón
            this.BackColor = originalBackColor;
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);

            GraphicsPath path = new GraphicsPath();
            int borderRadius = 16;  // Puedes ajustar el valor según tus preferencias

            // Crear un rectángulo redondeado
            path.AddArc(0, 0, borderRadius * 2, borderRadius * 2, 180, 90);
            path.AddArc(this.Width - borderRadius * 2, 0, borderRadius * 2, borderRadius * 2, 270, 90);
            path.AddArc(this.Width - borderRadius * 2, this.Height - borderRadius * 2, borderRadius * 2, borderRadius * 2, 0, 90);
            path.AddArc(0, this.Height - borderRadius * 2, borderRadius * 2, borderRadius * 2, 90, 90);
            path.CloseFigure();

            this.Region = new Region(path);
        }
    }
}
