using FontAwesome.Sharp;
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

using DrawingColor = System.Drawing.Color; //---------- para cambiar color el fondo


//Llamo a las Capas relacionadas
using CapaEntidad;
using CapaNegocio;
using System.Runtime.InteropServices;

namespace CapaPresentacion
{
    public partial class Inicio : Form
    {
        // Importa la función de usuario para mover la ventana
        [DllImport("user32.dll", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private extern static void SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;


        //Se crea una variable de tipo Usuario, global para todos los metodos de esta clase
        private static Usuario usuarioActual;
        //Almacena el menu activo
        private static IconMenuItem MenuActivo = null;
        //Indica el formulario activo en el panel
        private static Form FormularioActivo = null;
      
        //Constructor que recibe como parametro un objeto de tipo Usuario 
        public Inicio(Usuario objusuario = null)
        {
            if (objusuario == null)
                usuarioActual = new Usuario() { Apellido = "ADMIN PREDEFINIDO", IdUsuario = 1 };
            else
                //Se almacena el usuario que se logueo actualmente
                usuarioActual = objusuario;

            InitializeComponent();
        }
        
        //--------------------------------------------------------------------------------------------------------------

        private void Inicio_Load(object sender, EventArgs e)
        {
            //Esta línea de código crea una lista de objetos de tipo Permiso llamada ListaPermisos
            //al llamar al método Listar de la clase CN_Permiso. La variable usuarioActual.
            //IdUsuario se utiliza como argumento para el método Listar. de la clase CN_Permiso
            //ListaPermisos contendrá la lista de permisos asociados al usuario actual
            List<Permiso> ListaPermisos = new CN_Permiso().Listar(usuarioActual.IdUsuario);

            //Recorre todo el menu icono por icono y lo almacena en "iconmenu"
            foreach (IconMenuItem iconmenu in menu.Items)
            {
                //Compara de la ListaPermiso los "nombres" que se encuentren relacionados
                //devuelve true en caso de que asi sea
                //"Any" determina si una secuencia contiene elementos
                //"m" es cada uno de los elementos que contiene la lista
                bool encontrado = ListaPermisos.Any(m => m.NombreMenu == iconmenu.Name);

                //si no se encuentra coincidencia no se muestar el icono de menu
                if (encontrado == false)
                {
                    iconmenu.Visible = false;
                }

                //--------------------------------------------------------------------- 
                //Caragar imagenes a iconos de Iicio latelar
                menuventas.Image = Properties.Resources.compras40;
                menudetalleventa.Image = Properties.Resources.compras40;
                menuverproductos.Image = Properties.Resources.PRODUCTOcAJA;
                menuclientes.Image = Properties.Resources.clasificacion40;
                menuestadisticas.Image = Properties.Resources.estadistica40;
                menumantenedor.Image = Properties.Resources.herramientas40;
                menuusuarios.Image = Properties.Resources.vendedorBorde;
                menubackup.Image = Properties.Resources.guardar_datos;


            }

            //Muestra el nombre de usuario que se encuentra "logueado"
            lblusuario.Text = $"{usuarioActual.Apellido}, {usuarioActual.Nombre}";
            lblusuarioinicio.Text = $"{usuarioActual.Apellido}, {usuarioActual.Nombre}";

            //lblusuarioinicio.Text = usuarioActual.Apellido;
            txtidusuario.Text = Convert.ToString( usuarioActual.IdUsuario);
            timerReloj.Interval = 1000;
            timerReloj.Start();

            this.BackColor = Color.FromArgb(42, 47, 58); //---------- para cambiar color el fondo

            //ººººººººººººººººººººººººººººººººººººººººº
            // Cambia el color de fondo del MenuStrip
            menu.BackColor = Color.FromArgb(42, 47, 58);
            menu.ForeColor = Color.White;

            //ººººººººººººººººººººººººººººººººººººººººº

        }


        //--------------------------------------------------------------------------------------------------------------

        //Metodo para mostrar la hora
        private void timerReloj_Tick_1(object sender, EventArgs e)
        {
            lblreloj.Text = DateTime.Now.ToString("hh:mm tt");
            lblfecha.Text = DateTime.Now.ToLongDateString();
            relojesquina.Text = DateTime.Now.ToString("hh:mm tt");
        }

        //--------------------------------------------------------------------------------------------------------------

        //Recibe el boton del menu seleccionado y el formulario a mostrar
        private void AbrirFormulario(IconMenuItem menu, Form formulario)
        {
            //Pone en color "" el boton anteriormente seleccionado
            
            if (MenuActivo != null)
            {
                MenuActivo.BackColor = Color.FromArgb(42, 47, 58);
            }

            //Cambia de color el boton de menu seleccionado
            menu.BackColor = Color.FromArgb(90, 100, 120); //ººººººººººººººººººººººººººººººººººººººººººº

            //Almacena el menu recibido como parametro en MenuActivo
            MenuActivo = menu;

            //Cierra el formulario no activo que se abrio previamente
            if (FormularioActivo != null)
            {
                FormularioActivo.Close();
                panelvista.Visible = false;
               
            }

            FormularioActivo = formulario;
            //TopLevel = false no permite que el tamaño del formulario sea superior al externo
            formulario.TopLevel = false;
            //FormBorderStyle.None quita el estilo de bordes de la venta
            formulario.FormBorderStyle = FormBorderStyle.None;
            //DockStyle.Fill copa todo el espacio del contenedor disponible
            formulario.Dock = DockStyle.Fill;
            //Muestra del color "DarkSlateGray" como fondo del formulario ---------------------------------
            formulario.BackColor = Color.FromArgb(42, 47, 58); //ººººººººººººººººººººººººººººººººººººººººººº

            //Agrega el formulario seleccionado en el "contenedor" recibido como parametro
            contenedor.Controls.Add(formulario);
            
            //Permite que el formulario sea visible
            formulario.Show();

        }

        //--------------------------------------------------------------------------------------------------------------

        private void menuusuarios_Click(object sender, EventArgs e)
        {
            //Envia el menu seleccionado y el formulario a mostrar como parametros. "Es necesario castearlo previamente"
            AbrirFormulario((IconMenuItem)sender, new frmUsuarios());
            panelclousing();

        }

        //--------------------------------------------------------------------------------------------------------------

        private void submenuproducto_Click(object sender, EventArgs e)
        {
            //Envia el menu seleccionado y el formulario a mostrar como parametros
            AbrirFormulario(menumantenedor, new frmProducto());
            panelclousing();
        }


        private void submenuregistrarventa_Click(object sender, EventArgs e)
        {
            AbrirFormulario(menuventas, new frmVentas(usuarioActual));
            panelclousing();
        }

        private void submenuverdetalleventa_Click(object sender, EventArgs e)
        {
            AbrirFormulario(menuventas, new frmDetalleVenta()); 
            panelclousing();
        }


        private void menuclientes_Click(object sender, EventArgs e)
        {
            AbrirFormulario((IconMenuItem)sender, new frmClientes());
            panelclousing();
        }



        private void menuverdetalleventa_Click(object sender, EventArgs e)
        {
            AbrirFormulario((IconMenuItem)sender, new frmReportes());
            panelclousing();
        }


        private void menubackup_Click(object sender, EventArgs e)
        {
            AbrirFormulario((IconMenuItem)sender, new frmBackup());
            panelclousing();
        }

        private void menudetalleventa_Click(object sender, EventArgs e)
        {
            AbrirFormulario(menudetalleventa, new frmReportes(usuarioActual)); 
            panelclousing();
        }


        private void menuestadisticas_Click_1(object sender, EventArgs e)
        {
            AbrirFormulario((IconMenuItem)sender, new frmGraficos());
            panelclousing();
        }

        
        private void submenucategoria_Click(object sender, EventArgs e)
        {
            AbrirFormulario(menumantenedor, new frmCategoria()); 
            panelclousing();
        }

        
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AbrirFormulario(menuventas, new frmReportes(usuarioActual));
            panelclousing();
        }


        private void menuverproductos_Click(object sender, EventArgs e)
        {
            AbrirFormulario((IconMenuItem)sender, new mdVerProductos());
            panelclousing();
        }

        private void panelclousing()
        {
            panelvista.Visible = false;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }


    }
}
