using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapaDatos;
using CapaNegocio;
using System.Data.SqlClient;

namespace CapaPresentacion
{
    public partial class frmBackup : Form
    {
        //SqlConnection con=new SqlConnection("server=.(local); database = RESTAURANT_DB; integrated security = true");
        SqlConnection oconexion = new SqlConnection(Conexion.cadena);


        public frmBackup()
        {
            InitializeComponent();
        }

        private void limpiarDatos()
        {
            txt_backup.Clear();
            txt_backup.Text = " Seleccione un destino...";
            txt_restore.Clear();
            txt_restore.Text = " Seleccione el archivo .bak";
            btn_backup.Enabled = false;
            btn_restore.Enabled = false;
        }


        private void btn_examinar_backup_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txt_backup.Text = dlg.SelectedPath;
                btn_backup.Enabled = true;
                //pictureBox4.Enabled = true;
            }
        }

        private void btn_backup_Click(object sender, EventArgs e)
        {
            string database = oconexion.Database.ToString();
            if (txt_backup.Text == string.Empty)
            {
                MessageBox.Show("Por favor ingrese la ubicación del archivo de respaldo");
            }
            else
            {
                string cmd = "BACKUP DATABASE ["+ database + "] TO DISK= '"+ txt_backup.Text + "\\" + "database" + "-" + DateTime.Now.ToString("yyyy-MM-dd--HH-mm-ss") + ".bak'";
                oconexion.Open();
                SqlCommand command = new SqlCommand(cmd, oconexion);
                command.ExecuteNonQuery();
                MessageBox.Show("Copia de seguridad de la base de datos realizada con éxito");
                oconexion.Close();
                btn_backup.Enabled = false;
            }
        }

        private void btn_examinar_restore_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "SQL SERVER database backup files|*.bak";
            dlg.Title = "Database restore";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txt_restore.Text = dlg.FileName;
                btn_restore.Enabled = true;
                //pictureBox3.Enabled = true;
            }
        }


        private void btn_restore_Click(object sender, EventArgs e)
        {
            string database = oconexion.Database.ToString();
            oconexion.Open();
            try
            {
                string str1 = string.Format("ALTER DATABASE ["+ database +"] SET SINGLE_USER WITH ROLLBACK IMMEDIATE");
                SqlCommand cmd1 = new SqlCommand(str1, oconexion);
                cmd1.ExecuteNonQuery();

                string str2 = "USE MASTER RESTORE DATABASE [" + database + "] FROM DISK='" + txt_restore.Text + "' WITH REPLACE;";
                SqlCommand cmd2 = new SqlCommand(str2, oconexion);
                cmd2.ExecuteNonQuery();

                string str3 = string.Format("ALTER DATABASE [" + database + "]  SET MULTI_USER");
                SqlCommand cmd3 = new SqlCommand(str3, oconexion);
                cmd3.ExecuteNonQuery();

                MessageBox.Show("Restauración de la base de datos realizada con éxito");
                oconexion.Close();
            }
            catch
            {

            }
        }

        private void frmBackup_Load(object sender, EventArgs e)
        {

        }
    }
}
