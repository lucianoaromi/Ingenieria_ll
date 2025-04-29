using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.SqlClient;

// Hace referencia a la Capa Entidad, para hacer uso de la clase Usuario de la misma
using CapaEntidad;
using System.Collections;

namespace CapaDatos
{
    public class CD_Usuario
    {
        // Metodo que permite listar todos los usuarios desde la base de datos
        public List<Usuario> Listar()
        {
            //Genera la variable de tipo Lista que contiene Usuarios
            List<Usuario> lista = new List<Usuario>();

            //Conecta a la base por medio de la "cadena de conexion" -> (Conexiones.(clase en la capa de datos))
            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                
                //Capturador de errores por si falla la conexion a la BD
                try
                {
                    StringBuilder  query = new StringBuilder();

                    //Consulta a la BD la tabla Usuario y Rol, relacionando su IdRol
                    query.AppendLine("select u.IdUsuario,u.Documento,u.Apellido,u.Nombre,u.Direccion,u.Correo,u.Clave,u.Estado,r.IdRol,r.Descripcion from usuario u");
                    
                    //r y u son objetos de tipo Rol y Usuario respectivamente
                    query.AppendLine("inner join rol r on r.IdRol = u.IdRol");

                    // Crea un objeto SqlCommand con la consulta definida en 'query' y la conexión activa 'oconexion'
                    SqlCommand cmd = new SqlCommand(query.ToString(), oconexion);

                    // Establece el tipo de comando como texto (consulta SQL directa)
                    cmd.CommandType = CommandType.Text;

                    // Abre la conexión con la base de datos antes de ejecutar la consulta
                    oconexion.Open();


                    //Lee el resultado de los comandos anteriores
                    //Crea un objeto SqlDataReader llamado "dr" que se utiliza para leer, procesar y guardar los resultados de la consulta.
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            // crea un nuevo objeto de la clase Usuario, y agrega a la lista "lista".
                            lista.Add(new Usuario()
                            {
                                IdUsuario = Convert.ToInt32(dr["IdUsuario"]),
                                Documento = dr["Documento"].ToString(),
                                Apellido = dr["Apellido"].ToString(),
                                Nombre = dr["Nombre"].ToString(),
                                Direccion = dr["Direccion"].ToString(),
                                Correo = dr["Correo"].ToString(),
                                Clave = dr["Clave"].ToString(),
                                Estado = Convert.ToBoolean(dr["Estado"]),

                                //Crea una instancia de la clase Rol y establece
                                //sus propiedades IdRol y Descripcion con valores leídos desde un objeto SqlDataReader
                                oRol = new Rol() { IdRol = Convert.ToInt32(dr["IdRol"]),Descripcion = dr["Descripcion"].ToString()}

                            });
                        }
                    }

                }
                //si se produce una excepción se crea una nueva lista de usuarios, se garantiza que lista esté en un estado válido y vacío
                catch (Exception ex)
                {

                    lista = new List<Usuario>();
                }

            }

            // Se retorna la Lista
            return lista;

        }

        //Parametros de entrada y salida - "obj" objeto declaro de tipo usuario
        public int Registrar(Usuario obj, out string Mensaje)
        {
            int idusuariogenerado = 0;
            Mensaje = string.Empty;

            try
            {
                //Realiza la conexion a la base de datos con la cadena de conexion
                using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
                {
                    //Recibe como parametro el nombre del procedimiento almacenado 
                    SqlCommand cmd = new SqlCommand("SP_REGISTRARUSUARIO".ToString(), oconexion);
                   
                    //Parametros de entrada
                    cmd.Parameters.AddWithValue("Documento",obj.Documento);
                    cmd.Parameters.AddWithValue("Apellido", obj.Apellido);
                    cmd.Parameters.AddWithValue("Nombre", obj.Nombre);
                    cmd.Parameters.AddWithValue("Direccion", obj.Nombre);
                    cmd.Parameters.AddWithValue("Correo", obj.Correo);
                    cmd.Parameters.AddWithValue("Clave", obj.Clave);
                    cmd.Parameters.AddWithValue("IdRol", obj.oRol.IdRol);
                    cmd.Parameters.AddWithValue("Estado", obj.Estado);

                    //Parametros de salida
                    cmd.Parameters.Add("IdUsuarioResultado",SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar,500).Direction = ParameterDirection.Output;

                    cmd.CommandType = CommandType.StoredProcedure;
                    oconexion.Open();

                    cmd.ExecuteNonQuery();

                    idusuariogenerado = Convert.ToInt32( cmd.Parameters["IdUsuarioResultado"].Value);
                    Mensaje = cmd.Parameters["Mensaje"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                idusuariogenerado =0;
                Mensaje = ex.Message;


            }

            return idusuariogenerado;
        }





        //Parametros de entrada y salida - "obj" objeto declaro de tipo usuario
        public bool Editar(Usuario obj, out string Mensaje)
        {
            bool respuesta = false;
            Mensaje = string.Empty;

            try
            {
                //Realiza la conexion a la base de datos con la cadena de conexion
                using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
                {
                    //Recibe como parametro el nombre del procedimiento almacenado 
                    SqlCommand cmd = new SqlCommand("SP_EDITARUSUARIO".ToString(), oconexion);

                    //Parametros de entrada
                    cmd.Parameters.AddWithValue("IdUsuario", obj.IdUsuario);
                    cmd.Parameters.AddWithValue("Documento", obj.Documento);
                    cmd.Parameters.AddWithValue("Apellido", obj.Apellido);
                    cmd.Parameters.AddWithValue("Nombre", obj.Nombre);
                    cmd.Parameters.AddWithValue("Direccion", obj.Direccion);
                    cmd.Parameters.AddWithValue("Correo", obj.Correo);
                    cmd.Parameters.AddWithValue("Clave", obj.Clave);
                    cmd.Parameters.AddWithValue("IdRol", obj.oRol.IdRol);
                    cmd.Parameters.AddWithValue("Estado", obj.Estado);

                    //Parametros de salida
                    cmd.Parameters.Add("Respuesta", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar,500).Direction = ParameterDirection.Output;

                    cmd.CommandType = CommandType.StoredProcedure;
                    oconexion.Open();

                    cmd.ExecuteNonQuery();

                    respuesta = Convert.ToBoolean(cmd.Parameters["Respuesta"].Value);
                    Mensaje = cmd.Parameters["Mensaje"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                respuesta = false;
                Mensaje = ex.Message;


            }

            return respuesta;
        }




        //Parametros de entrada y salida - "obj" objeto declaro de tipo usuario
        public bool Eliminar(Usuario obj, out string Mensaje)
        {
            bool respuesta = false;
            Mensaje = string.Empty;

            try
            {
                //Realiza la conexion a la base de datos con la cadena de conexion
                using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
                {
                    //Recibe como parametro el nombre del procedimiento almacenado 
                    SqlCommand cmd = new SqlCommand("SP_ELIMINARUSUARIO", oconexion);

                    //Parametros de entrada
                    cmd.Parameters.AddWithValue("IdUsuario", obj.IdUsuario);

                    //Parametros de salida
                    cmd.Parameters.Add("Respuesta", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar,500).Direction = ParameterDirection.Output;

                    cmd.CommandType = CommandType.StoredProcedure;
                    oconexion.Open();

                    cmd.ExecuteNonQuery();

                    respuesta = Convert.ToBoolean(cmd.Parameters["Respuesta"].Value);
                    Mensaje = cmd.Parameters["Mensaje"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                respuesta = false;
                Mensaje = ex.Message;


            }

            return respuesta;
        }

    }
}
