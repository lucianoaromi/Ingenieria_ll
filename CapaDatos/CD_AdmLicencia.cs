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
    public class CD_AdmLicencia
    {
        // Metodo que permite listar todos los clientes desde la base de datos
        public List<CE_Licencia> Listar()
        {
            //Genera la variable de tipo Lista que contiene Clientes
            List<CE_Licencia> lista = new List<CE_Licencia>();

            //Conecta a la base por medio de la "cadena de conexion" -> (Conexiones.(clase en la capa de datos))
            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {

                //Capturador de errores por si falla la conexion a la BD
                try
                {
                    StringBuilder query = new StringBuilder();

                    //Consulta a la BD la tabla Cliente
                    query.AppendLine(
                        "select c.ID, " +
                        "c.CodigoActivacion, " +
                        "c.Activado, " +
                        "c.FechaInicio, " +
                        "c.FechaActivacion, " +                        
                        "c.DiasPermitidos, " +
                        "c.UltimaVerificacion " +

                        "from softwareState c");

                    SqlCommand cmd = new SqlCommand(query.ToString(), oconexion);
                    cmd.CommandType = CommandType.Text;

                    oconexion.Open();

                    //Lee el resultado del comando anterior
                    //Crea un objeto SqlDataReader llamado "dr" que se utiliza para leer, procesar y guardar los resultados de la consulta.
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            // crea un nuevo objeto de la clase Cliente, y agrega a la lista "lista".
                            lista.Add(new CE_Licencia()
                            {
                                ID = Convert.ToInt32(dr["ID"]),
                                CodigoActivacion = dr["CodigoActivacion"].ToString(),
                                Activado = Convert.ToBoolean(dr["Activado"]),
                                FechaInicio = Convert.ToDateTime(dr["FechaInicio"]), // Asigna la fecha de registro
                                FechaActivacion = Convert.ToDateTime(dr["FechaActivacion"]), // Asigna la hora de registro
                                DiasPermitidos = Convert.ToInt32(dr["DiasPermitidos"]),
                                UltimaVerificacion = dr["UltimaVerificacion"] == DBNull.Value
                                                         ? (DateTime?)null
                                                         : Convert.ToDateTime(dr["UltimaVerificacion"]),

                            });
                        }
                    }

                }
                //si se produce una excepción se crea una nueva lista de clientes, se garantiza que lista esté en un estado válido y vacío
                catch (Exception ex)
                {
                    lista = new List<CE_Licencia>();
                }

            }

            return lista;
        }


        //Parametros de entrada y salida - "obj" objeto declarado de tipo cliente
        public int Registrar(CE_Licencia obj, out string Mensaje)
        {
            int idclientegenerado = 0;
            Mensaje = string.Empty;

            try
            {
                //Realiza la conexión a la base de datos con la cadena de conexión
                using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
                {
                    //Recibe como parámetro el nombre del procedimiento almacenado
                    SqlCommand cmd = new SqlCommand("SP_REGISTRARLICENCIA", oconexion);

                    //Parametros de entrada
                    cmd.Parameters.AddWithValue("CodigoActivacion", obj.CodigoActivacion);
                    cmd.Parameters.AddWithValue("Activado", obj.Activado);
                    cmd.Parameters.AddWithValue("FechaInicio", obj.FechaInicio);
                    cmd.Parameters.AddWithValue("FechaActivacion", obj.FechaActivacion); // Registrar solo la hora
                    cmd.Parameters.AddWithValue("DiasPermitidos", obj.DiasPermitidos);

                    //Parametros de salida
                    cmd.Parameters.Add("IdLicenciaResultado", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;

                    cmd.CommandType = CommandType.StoredProcedure;
                    oconexion.Open();

                    cmd.ExecuteNonQuery();

                    idclientegenerado = Convert.ToInt32(cmd.Parameters["IdLicenciaResultado"].Value);
                    Mensaje = cmd.Parameters["Mensaje"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                idclientegenerado = 0;
                Mensaje = ex.Message;
            }

            return idclientegenerado;
        }


        //Parametros de entrada y salida - "obj" objeto declarado de tipo cliente
        public bool Editar(CE_Licencia obj, out string Mensaje)
        {
            bool respuesta = false;
            Mensaje = string.Empty;

            try
            {
                //Realiza la conexión a la base de datos con la cadena de conexión
                using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
                {
                    //Recibe como parámetro el nombre del procedimiento almacenado
                    SqlCommand cmd = new SqlCommand("SP_EDITARLICENCIA", oconexion);

                    //Parametros de entrada
                    cmd.Parameters.AddWithValue("ID", obj.ID);
                    cmd.Parameters.AddWithValue("CodigoActivacion", obj.CodigoActivacion);
                    cmd.Parameters.AddWithValue("Activado", obj.Activado);
                    cmd.Parameters.AddWithValue("FechaInicio", obj.FechaInicio);
                    cmd.Parameters.AddWithValue("FechaActivacion", obj.FechaActivacion); // Registrar solo la hora

                    cmd.Parameters.AddWithValue("DiasPermitidos", obj.DiasPermitidos);

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
        public bool Eliminar(CE_Licencia obj, out string Mensaje)
        {
            bool respuesta = false;
            Mensaje = string.Empty;

            try
            {
                //Realiza la conexion a la base de datos con la cadena de conexion
                using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
                {
                    //Recibe como parametro el nombre del procedimiento almacenado 
                    SqlCommand cmd = new SqlCommand("SP_ELIMINARLICENCIA", oconexion);

                    //Parametros de entrada
                    cmd.Parameters.AddWithValue("ID", obj.ID);

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
