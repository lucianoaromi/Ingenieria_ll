using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using CapaNegocio;
using System.Drawing;

using System.IO;

namespace CapaPresentacion
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            CN_Licencia softwareStateBLL = new CN_Licencia();

            try
            {
                if (softwareStateBLL.VerificarUsoPermitido(out int diasRestantes))
                {
                    if (diasRestantes == -1)
                    {
                        Application.Run(new Login()); // El software no tiene limitación
                    }
                    else
                    {
                        Application.Run(new Login()); // Período de prueba activo
                    }
                }
                else
                {
                    // Mostrar el formulario de activación
                    FrmActivacion frmActivacion = new FrmActivacion();
                    if (frmActivacion.ShowDialog() == DialogResult.OK)
                    {
                        string codigoActivacion = frmActivacion.CodigoActivacion;
                        if (softwareStateBLL.ActivarSoftware(codigoActivacion))
                        {
                            //SaveActivationDate(); // Guarda la fecha de activación
                            Application.Run(new Login());
                        }
                        else
                        {
                            MessageBox.Show("Código de activación incorrecto. La aplicación se cerrará.", "Error de activación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Application.Exit();
                        }
                    }
                    else
                    {
                        // El usuario cerró el formulario sin ingresar un código
                        Application.Exit();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al verificar el estado del software: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }
    }
}



