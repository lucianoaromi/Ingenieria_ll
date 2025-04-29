using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//Hace referencia a la carpeta "Utilidades" para el uso de la clase "OpcionCombo"
using CapaPresentacion.Utilidades;

using CapaEntidad;
using CapaNegocio;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Globalization;

namespace CapaPresentacion
{
    public partial class frmUsuarios : Form
    {
        private Usuario objusuario;

        public frmUsuarios()
        {
            InitializeComponent();

        }

        private void frmUsuarios_Load(object sender, EventArgs e)
        {
            cboestado.Items.Add(new OpcionCombo() { Valor = 1, Texto = "Activo" });
            cboestado.Items.Add(new OpcionCombo() { Valor = 0, Texto = "No Activo" });
            //Especifica que solo se debe mostrar el dato de nombre "Texto"
            cboestado.DisplayMember = "Texto";
            //Maneja como informacion interna el dato de nombre "Valor"
            cboestado.ValueMember = "Valor";
            //Se selecciona siempre el indice 0
            cboestado.SelectedIndex = 0;

            txtcorreo.Text = "@gmail.com";

            List<Rol> listaRol = new CN_Rol().Listar();

            foreach (Rol item in listaRol)
            {
                cborol.Items.Add(new OpcionCombo() { Valor = item.IdRol, Texto = item.Descripcion });
            }
            cborol.DisplayMember = "Texto";
            cborol.ValueMember = "Valor";
            cborol.SelectedIndex = 0;

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

            
            //MOSTRAR TODOS LOS USARIOS
            List<Usuario> listaUsuario = new CN_Usuario().Listar();

            foreach (Usuario item in listaUsuario)
            {
               dgvdata.Rows.Add(new object[] {"", item.IdUsuario,item.Documento,item.Apellido,item.Nombre,item.Direccion,item.Correo,item.Clave,
                   item.oRol.IdRol,
                   item.oRol.Descripcion,
                   item.Estado ==  true ? 1 : 0,
                   item.Estado == true ? "Activo" : "No Activo",
               });
            }

            txtdocumento.Focus();

            dgvdata.ForeColor = System.Drawing.Color.Black;

            /*
            //txtdocumento.TabIndex = 0;
            txtapellido.TabIndex = 1;
            txtnombre.TabIndex = 2;
            txtdireccion.TabIndex = 3;
            txtcorreo.TabIndex = 4;
            txtclave.TabIndex = 5;
            txtconfirmarclave.TabIndex = 6;
            cborol.TabIndex = 7;
            cboestado.TabIndex = 8;
            */

        }

        //------------------------------------------------------------------------------------------------------------------------------------------

        private void btnguardar_Click_1(object sender, EventArgs e)
        {
            String hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(txtclave.Text);
            string mensaje = string.Empty;
            Usuario objusuario = new Usuario()
            {
                IdUsuario = Convert.ToInt32(txtid.Text),
                Documento = txtdocumento.Text,
                Apellido = txtapellido.Text,
                Nombre = txtnombre.Text,
                Direccion = txtdireccion.Text,
                Correo = txtcorreo.Text,
                Clave = hashedPassword,
                oRol = new Rol() { IdRol = Convert.ToInt32(((OpcionCombo)cborol.SelectedItem).Valor)},
                Estado = Convert.ToInt32(((OpcionCombo)cboestado.SelectedItem).Valor) == 1 ? true : false         
            };

            
            if(objusuario.IdUsuario == 0)
            {
                //Ejecuta el metodo Registrar de la Clase Usuario en la Cap de Neg con sus respectivos parametros, retornando el idusuario
                int idusuariogenerado = new CN_Usuario().Registrar(objusuario, out mensaje);

                if (idusuariogenerado != 0)
                {
                  dgvdata.Rows.Add(new object[] {"",idusuariogenerado,txtdocumento.Text,txtapellido.Text,txtnombre.Text,txtdireccion.Text,txtcorreo.Text,txtclave.Text,
                  ((OpcionCombo)cborol.SelectedItem).Valor.ToString(),
                  ((OpcionCombo)cborol.SelectedItem).Texto.ToString(),
                  ((OpcionCombo)cboestado.SelectedItem).Valor.ToString(),
                  ((OpcionCombo)cboestado.SelectedItem).Texto.ToString()
                });

                    Limpiar();
                }
                else
                {
                    MessageBox.Show(mensaje);
                }

            }
            //Si el objeto idusuario no es igual a 0 se accede a editar el usario seleccionado del datagrid
            else
            {
                bool resultado = new CN_Usuario().Editar(objusuario, out mensaje);

                if (resultado)
                {   
                    //Se obtiene la fila seleccionada en el datagrid
                    DataGridViewRow row = dgvdata.Rows[Convert.ToInt32(txtindice.Text)];
                    //Se realiza el llamado a las filas del datagrid
                    row.Cells["Id"].Value = txtid.Text;
                    row.Cells["Documento"].Value = txtdocumento.Text;
                    row.Cells["Apellido"].Value = txtapellido.Text;
                    row.Cells["Nombre"].Value = txtnombre.Text;
                    row.Cells["Direccion"].Value = txtdireccion.Text;
                    row.Cells["Correo"].Value = txtcorreo.Text;
                    row.Cells["Clave"].Value = txtclave.Text;
                    row.Cells["IdRol"].Value = ((OpcionCombo)cborol.SelectedItem).Valor.ToString();
                    row.Cells["Rol"].Value = ((OpcionCombo)cborol.SelectedItem).Texto.ToString();
                    row.Cells["EstadoValor"].Value = ((OpcionCombo)cboestado.SelectedItem).Valor.ToString();
                    row.Cells["Estado"].Value = ((OpcionCombo)cboestado.SelectedItem).Texto.ToString();

                    Limpiar();

                }
                else
                {
                    MessageBox.Show(mensaje);
                }
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------

        private void Limpiar()
        {
            // Restablecer colores originales de todas las filas del DataGridView
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                row.DefaultCellStyle.BackColor = dgvdata.DefaultCellStyle.BackColor;
                row.DefaultCellStyle.ForeColor = dgvdata.DefaultCellStyle.ForeColor;
            }

            txtindice.Text = "-1";
            txtid.Text = "0";
            txtdocumento.Text = "";
            txtapellido.Text = "";
            txtnombre.Text = "";
            txtdireccion.Text = "";
            txtcorreo.Text = "@gmail.com";
            txtclave.Text = "";
            txtconfirmarclave.Text = "";
            cborol.SelectedIndex = 0;   
            cboestado.SelectedIndex = 0;
            //El foco se va a el textbox documento
            txtdocumento.Select();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------

        //Muestra la imagen de tilde en el DataGrid
        private void dgvdata_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            if (e.ColumnIndex == 0)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                var w = Properties.Resources.editaricon.Width;
                var h = Properties.Resources.editaricon.Height;
                var x = e.CellBounds.Left + (e.CellBounds.Width - w)/2;
                var y = e.CellBounds.Top + (e.CellBounds.Height - h)/2;

                e.Graphics.DrawImage(Properties.Resources.editaricon, new Rectangle(x, y, w, h));
                e.Handled = true;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------

        //Boton de DataGrid que trae los datos hacia los TextBox
        private void dgvdata_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvdata.Columns[e.ColumnIndex].Name == "btnseleccionar")
            {
                int indice = e.RowIndex;

                if (indice >=0){

                    // Establecer color blanco a todas las filas para restablecer
                    foreach (DataGridViewRow row in dgvdata.Rows)
                    {
                        row.DefaultCellStyle.BackColor = dgvdata.DefaultCellStyle.BackColor;
                    }

                    dgvdata.Rows[indice].DefaultCellStyle.BackColor = System.Drawing.Color.LightCoral;

                    txtindice.Text = indice.ToString();

                    txtid.Text = dgvdata.Rows[indice].Cells["Id"].Value.ToString();
                    txtdocumento.Text = dgvdata.Rows[indice].Cells["Documento"].Value.ToString();
                    txtapellido.Text = dgvdata.Rows[indice].Cells["Apellido"].Value.ToString();
                    txtnombre.Text = dgvdata.Rows[indice].Cells["Nombre"].Value.ToString();
                    txtdireccion.Text = dgvdata.Rows[indice].Cells["Direccion"].Value.ToString();
                    txtcorreo.Text = dgvdata.Rows[indice].Cells["Correo"].Value.ToString();
                    txtclave.Text = dgvdata.Rows[indice].Cells["Clave"].Value.ToString();
                    txtconfirmarclave.Text = dgvdata.Rows[indice].Cells["Clave"].Value.ToString();

                    foreach (OpcionCombo oc in cborol.Items)
                    {
                        if(Convert.ToInt32(oc.Valor) == Convert.ToInt32(dgvdata.Rows[indice].Cells["IdRol"].Value))
                        {
                            int indice_combo = cborol.Items.IndexOf(oc);
                            cborol.SelectedIndex = indice_combo;
                            break;
                        }
                    }


                    foreach (OpcionCombo oc in cboestado.Items)
                    {
                        if (Convert.ToInt32(oc.Valor) == Convert.ToInt32(dgvdata.Rows[indice].Cells["EstadoValor"].Value))
                        {
                            int indice_combo = cboestado.Items.IndexOf(oc);
                            cboestado.SelectedIndex = indice_combo;
                            break;
                        }
                    }
                }
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------

        private void btneliminar_Click_1(object sender, EventArgs e)
        {
            if (Convert.ToInt32(txtid.Text) != 0)
            {
                if (MessageBox.Show("¿Desea eliminar el usuario?","Alerta",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string mensaje = string.Empty;
                    Usuario objusuario = new Usuario()
                    {
                        IdUsuario = Convert.ToInt32(txtid.Text)
                    };
                    
                    
                    bool respuesta = new CN_Usuario().Eliminar(objusuario, out mensaje);

                    if (respuesta)
                    {
                        dgvdata.Rows.RemoveAt(Convert.ToInt32(txtindice.Text));
                    }
                    else
                    {
                        MessageBox.Show(mensaje, "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------

        private void btnlimpiar_Click_1(object sender, EventArgs e)
        {
            Limpiar();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------

        private void txtdocumento_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            // Verifica si la tecla presionada es un número o una tecla de control (por ejemplo, Backspace).
            // Permite solo números y teclas de control como Backspace, Enter, etc.
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                // Si la tecla presionada no es un número ni una tecla de control, se ignora.
                e.Handled = true;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------

        private void txtnombrecompleto_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Verifica si la tecla presionada es una letra y no una tecla de control (por ejemplo, Backspace).
            if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar))
            {
                // Si la tecla presionada no es una letra ni una tecla de control, se ignora.
                e.Handled = true;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------

        private void txtnombre_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Verifica si la tecla presionada es una letra y no una tecla de control (por ejemplo, Backspace).
            if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar))
            {
                // Si la tecla presionada no es una letra ni una tecla de control, se ignora.
                e.Handled = true;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------

        private void btnbuscar_Click_1(object sender, EventArgs e)
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

        //------------------------------------------------------------------------------------------------------------------------------------------

        private void btnlimpiarbusqueda_Click_1(object sender, EventArgs e)
        {
            txtbusqueda.Text = "";
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                row.Visible = true;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            //-----------------------------------------------
            // Verificar si el campo txtstock y txtprecio esten vacíos
            if (string.IsNullOrEmpty(txtclave.Text))
            {
                MessageBox.Show("Es necesario la 'Contraseña' del usuario.", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Detener la ejecución si el campo está vacío
            }



            // Obtener los valores de los TextBox
            string contraseña = txtclave.Text;
            string confirmarContraseña = txtconfirmarclave.Text;

            // Verificar si coinciden
            if (contraseña == confirmarContraseña)
            {
                //MessageBox.Show("Las contraseñas coinciden.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Las contraseñas no coinciden. Por favor, verifica.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Detener la ejecución si el campo está vacío
            }

            //-----------------------------------------------

            String hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(txtclave.Text);
            string mensaje = string.Empty;
            Usuario objusuario = new Usuario()
            {
                IdUsuario = Convert.ToInt32(txtid.Text),
                Documento = txtdocumento.Text,
                Apellido = txtapellido.Text,
                Nombre = txtnombre.Text,
                Direccion = txtdireccion.Text,
                Correo = txtcorreo.Text,
                Clave = hashedPassword,
                oRol = new Rol() { IdRol = Convert.ToInt32(((OpcionCombo)cborol.SelectedItem).Valor) },
                Estado = Convert.ToInt32(((OpcionCombo)cboestado.SelectedItem).Valor) == 1 ? true : false
            };


            if (objusuario.IdUsuario == 0)
            {
                //Ejecuta el metodo Registrar de la Clase Usuario en la Cap de Neg con sus respectivos parametros, retornando el idusuario
                int idusuariogenerado = new CN_Usuario().Registrar(objusuario, out mensaje);

                if (idusuariogenerado != 0)
                {
                    dgvdata.Rows.Add(new object[] {"",idusuariogenerado,txtdocumento.Text,txtapellido.Text,txtnombre.Text,txtdireccion.Text,txtcorreo.Text,txtclave.Text,
                      ((OpcionCombo)cborol.SelectedItem).Valor.ToString(),
                      ((OpcionCombo)cborol.SelectedItem).Texto.ToString(),
                      ((OpcionCombo)cboestado.SelectedItem).Valor.ToString(),
                      ((OpcionCombo)cboestado.SelectedItem).Texto.ToString()
                    });

                    Limpiar();
                }
                else
                {
                    MessageBox.Show(mensaje, "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }
            //Si el objeto idusuario no es igual a 0 se accede a editar el usario seleccionado del datagrid
            else
            {
                bool resultado = new CN_Usuario().Editar(objusuario, out mensaje);

                if (resultado)
                {
                    //Se obtiene la fila seleccionada en el datagrid
                    DataGridViewRow row = dgvdata.Rows[Convert.ToInt32(txtindice.Text)];
                    //Se realiza el llamado a las filas del datagrid
                    row.Cells["Id"].Value = txtid.Text;
                    row.Cells["Documento"].Value = txtdocumento.Text;
                    row.Cells["Apellido"].Value = txtapellido.Text;
                    row.Cells["Nombre"].Value = txtnombre.Text;
                    row.Cells["Direccion"].Value = txtdireccion.Text;
                    row.Cells["Correo"].Value = txtcorreo.Text;
                    row.Cells["Clave"].Value = txtclave.Text;
                    row.Cells["IdRol"].Value = ((OpcionCombo)cborol.SelectedItem).Valor.ToString();
                    row.Cells["Rol"].Value = ((OpcionCombo)cborol.SelectedItem).Texto.ToString();
                    row.Cells["EstadoValor"].Value = ((OpcionCombo)cboestado.SelectedItem).Valor.ToString();
                    row.Cells["Estado"].Value = ((OpcionCombo)cboestado.SelectedItem).Texto.ToString();

                    Limpiar();

                }
                else
                {
                    MessageBox.Show(mensaje, "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Limpiar();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(txtid.Text) != 0)
            {
                if (MessageBox.Show("¿Desea eliminar el usuario?", "Alerta", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string mensaje = string.Empty;
                    Usuario objusuario = new Usuario()
                    {
                        IdUsuario = Convert.ToInt32(txtid.Text)
                    };


                    bool respuesta = new CN_Usuario().Eliminar(objusuario, out mensaje);

                    if (respuesta)
                    {
                        dgvdata.Rows.RemoveAt(Convert.ToInt32(txtindice.Text));
                    }
                    else
                    {
                        MessageBox.Show(mensaje, "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------

        private void pictureBox4_Click(object sender, EventArgs e)
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

        //------------------------------------------------------------------------------------------------------------------------------------------

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            txtbusqueda.Text = "";
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                row.Visible = true;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------
    }
}
