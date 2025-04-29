using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.SqlClient;
using CapaEntidad;
using System.Collections;

namespace CapaDatos
{
    public class CD_Cliente
    {
        // Metodo que permite listar todos los usuarios desde la base de datos
        public List<Cliente> Listar()
        {
            //Genera la variable de tipo Lista que contiene Usuarios
            List<Cliente> lista = new List<Cliente>();

            //Conecta a la base por medio de la "cadena de conexion" -> (Conexiones.(clase en la capa de datos))
            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {

                //Capturador de errores por si falla la conexion a la BD
                try
                {
                    StringBuilder query = new StringBuilder();

                    //Consulta a la BD la tabla Usuario
                    query.AppendLine("select c.IdCliente,c.Apellido,c.Nombre,c.Direccion,c.Telefono,c.Estado from cliente c");

                    //r y u son objetos de tipo Rol y Usuario respectivamente
                    //query.AppendLine("inner join rol r on r.IdRol = u.IdRol");

                    SqlCommand cmd = new SqlCommand(query.ToString(), oconexion);
                    cmd.CommandType = CommandType.Text;

                    oconexion.Open();

                    //Lee el resultado del comando anterior
                    //Crea un objeto SqlDataReader llamado "dr" que se utiliza para leer, procesar y guardar los resultados de la consulta.
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            // crea un nuevo objeto de la clase Usuario, y agrega a la lista "lista".
                            lista.Add(new Cliente()
                            {
                                IdCliente = Convert.ToInt32(dr["IdCliente"]),
                                //Documento = dr["Documento"].ToString(),
                                Apellido = dr["Apellido"].ToString(),
                                Nombre = dr["Nombre"].ToString(),
                                Direccion = dr["Direccion"].ToString(),
                                //Correo = dr["Correo"].ToString(),
                                Telefono = dr["Telefono"].ToString(),
                                Estado = Convert.ToBoolean(dr["Estado"]),

                                //Crea una instancia de la clase Rol y establece
                                //sus propiedades IdRol y Descripcion con valores leídos desde un objeto SqlDataReader
                                //oRol = new Rol() { IdRol = Convert.ToInt32(dr["IdRol"]), Descripcion = dr["Descripcion"].ToString() }

                            });
                        }
                    }

                }
                //si se produce una excepción se crea una nueva lista de usuarios, se garantiza que lista esté en un estado válido y vacío
                catch (Exception ex)
                {

                    lista = new List<Cliente>();
                }

            }

            return lista;


        }

        //Parametros de entrada y salida - "obj" objeto declaro de tipo usuario
        public int Registrar(Cliente obj, out string Mensaje)
        {
            int idclientegenerado = 0;
            Mensaje = string.Empty;

            try
            {
                //Realiza la conexion a la base de datos con la cadena de conexion
                using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
                {
                    //Recibe como parametro el nombre del procedimiento almacenado 
                    SqlCommand cmd = new SqlCommand("SP_REGISTRARCLIENTE".ToString(), oconexion);

                    //Parametros de entrada
                    //cmd.Parameters.AddWithValue("Documento", obj.Documento);
                    cmd.Parameters.AddWithValue("Apellido", obj.Apellido);
                    cmd.Parameters.AddWithValue("Nombre", obj.Nombre);
                    cmd.Parameters.AddWithValue("Direccion", obj.Direccion);
                    //cmd.Parameters.AddWithValue("Correo", obj.Correo);
                    cmd.Parameters.AddWithValue("Telefono", obj.Telefono);
                    cmd.Parameters.AddWithValue("Estado", obj.Estado);

                    //Parametros de salida
                    cmd.Parameters.Add("IdClienteResultado", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;

                    cmd.CommandType = CommandType.StoredProcedure;
                    oconexion.Open();

                    cmd.ExecuteNonQuery();

                    idclientegenerado = Convert.ToInt32(cmd.Parameters["IdClienteResultado"].Value);
                    Mensaje = cmd.Parameters["Mensaje"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                idclientegenerado =0;
                Mensaje = ex.Message;


            }

            return idclientegenerado;
        }





        //Parametros de entrada y salida - "obj" objeto declaro de tipo usuario
        public bool Editar(Cliente obj, out string Mensaje)
        {
            bool respuesta = false;
            Mensaje = string.Empty;

            try
            {
                //Realiza la conexion a la base de datos con la cadena de conexion
                using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
                {
                    //Recibe como parametro el nombre del procedimiento almacenado 
                    SqlCommand cmd = new SqlCommand("SP_EDITARCLIENTE".ToString(), oconexion);

                    //Parametros de entrada
                    cmd.Parameters.AddWithValue("IdCliente", obj.IdCliente);
                    //cmd.Parameters.AddWithValue("Documento", obj.Documento);
                    cmd.Parameters.AddWithValue("Apellido", obj.Apellido);
                    cmd.Parameters.AddWithValue("Nombre", obj.Nombre);
                    cmd.Parameters.AddWithValue("Direccion", obj.Direccion);
                    //cmd.Parameters.AddWithValue("Correo", obj.Correo);
                    cmd.Parameters.AddWithValue("Telefono", obj.Telefono);
                    cmd.Parameters.AddWithValue("Estado", obj.Estado);

                    //Parametros de salida
                    cmd.Parameters.Add("Respuesta", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;

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
        public bool Eliminar(Cliente obj, out string Mensaje)
        {
            bool respuesta = false;
            Mensaje = string.Empty;

            try
            {
                //Realiza la conexion a la base de datos con la cadena de conexion
                using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
                {
                    //Recibe como parametro el nombre del procedimiento almacenado 
                    SqlCommand cmd = new SqlCommand("SP_ELIMINARCLIENTE", oconexion);

                    //Parametros de entrada
                    cmd.Parameters.AddWithValue("IdCliente", obj.IdCliente);

                    //Parametros de salida
                    cmd.Parameters.Add("Respuesta", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;

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
