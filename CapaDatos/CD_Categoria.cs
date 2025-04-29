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
    public class CD_Categoria
    {
        // Metodo que permite listar todos los usuarios desde la base de datos
        public List<Categoria> Listar()
        {
            //Genera la variable de tipo Lista que contiene Usuarios
            List<Categoria> lista = new List<Categoria>();

            //Conecta a la base por medio de la "cadena de conexion" -> (Conexiones.(clase en la capa de datos))
            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {

                //Capturador de errores por si falla la conexion a la BD
                try
                {
                    StringBuilder query = new StringBuilder();

                    //Consulta a la BD la tabla Usuario
                    query.AppendLine("select c.IdCategoria,c.Descripcion,c.Estado from categoria c");

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
                            lista.Add(new Categoria()
                            {
                                IdCategoria = Convert.ToInt32(dr["IdCategoria"]),
                                Descripcion = dr["Descripcion"].ToString(),
                                Estado = Convert.ToBoolean(dr["Estado"]),

                            });
                        }
                    }

                }
                //si se produce una excepción se crea una nueva lista de usuarios, se garantiza que lista esté en un estado válido y vacío
                catch (Exception ex)
                {

                    lista = new List<Categoria>();
                }

            }

            return lista;


        }

        //Parametros de entrada y salida - "obj" objeto declaro de tipo usuario
        public int Registrar(Categoria obj, out string Mensaje)
        {
            int idclientegenerado = 0;
            Mensaje = string.Empty;

            try
            {
                //Realiza la conexion a la base de datos con la cadena de conexion
                using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
                {
                    //Recibe como parametro el nombre del procedimiento almacenado 
                    SqlCommand cmd = new SqlCommand("SP_REGISTRARCATEGORIA".ToString(), oconexion);

                    //Parametros de entrada
                    cmd.Parameters.AddWithValue("Descripcion", obj.Descripcion);
                    cmd.Parameters.AddWithValue("Estado", obj.Estado);

                    //Parametros de salida
                    cmd.Parameters.Add("IdCategoriaResultado", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;

                    cmd.CommandType = CommandType.StoredProcedure;
                    oconexion.Open();

                    cmd.ExecuteNonQuery();

                    idclientegenerado = Convert.ToInt32(cmd.Parameters["IdCategoriaResultado"].Value);
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
        public bool Editar(Categoria obj, out string Mensaje)
        {
            bool respuesta = false;
            Mensaje = string.Empty;

            try
            {
                //Realiza la conexion a la base de datos con la cadena de conexion
                using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
                {
                    //Recibe como parametro el nombre del procedimiento almacenado 
                    SqlCommand cmd = new SqlCommand("SP_EDITARCATEGORIA".ToString(), oconexion);

                    //Parametros de entrada
                    cmd.Parameters.AddWithValue("IdCategoria", obj.IdCategoria);
                    cmd.Parameters.AddWithValue("Descripcion", obj.Descripcion);
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
        public bool Eliminar(Categoria obj, out string Mensaje)
        {
            bool respuesta = false;
            Mensaje = string.Empty;

            try
            {
                //Realiza la conexion a la base de datos con la cadena de conexion
                using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
                {
                    //Recibe como parametro el nombre del procedimiento almacenado 
                    SqlCommand cmd = new SqlCommand("SP_ELIMINARCATEGORIA", oconexion);

                    //Parametros de entrada
                    cmd.Parameters.AddWithValue("IdCategoria", obj.IdCategoria);

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
