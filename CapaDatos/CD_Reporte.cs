using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos
{
    public class CD_Reporte
    {

        // Metodo que permite listar todos los reportes de ventas desde la base de datos
        public List<ReporteVenta> Venta(string fechainicio, string fechafin, int idusuario)
        {
            //Genera la variable de tipo Lista que contiene Reportes
            List<ReporteVenta> lista = new List<ReporteVenta>();

            //Conecta a la base de datos
            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {

                //Capturador de errores por si falla la conexion a la BD
                try
                {
                    StringBuilder query = new StringBuilder();
                    SqlCommand cmd = new SqlCommand("sp_ReporteVentas", oconexion);
                    cmd.Parameters.AddWithValue("FechaInicio", fechainicio);
                    cmd.Parameters.AddWithValue("FechaFin", fechafin);
                    cmd.Parameters.AddWithValue("IdUsuario", idusuario);
                    cmd.CommandType = CommandType.StoredProcedure;

                    oconexion.Open();

                   
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            
                            lista.Add(new ReporteVenta()
                            {
                                
                                FechaRegistro = dr["FechaRegistro"].ToString(),
                                TipoDocumento = dr["TipoDocumento"].ToString(),
                                NumeroDocumento = dr["NumeroDocumento"].ToString(),
                                MontoTotal = dr["MontoTotal"].ToString(),
                                UsuarioRegistro = dr["usuarioregistro"].ToString(),
                                ApellidoCliente = dr["nombrecompletocliente"].ToString(),
                                DesMetPago = dr["DesMetPago"].ToString(),
                                EstadoEntrega = dr["EstadoEntrega"].ToString(), // Agregar el nuevo campo aquí
                                EstadoPago = dr["EstadoPago"].ToString(), // Agregar el nuevo campo aquí
                                IdVenta = dr["IdVenta"].ToString(),
                            });
                        }
                    }

                }
                
                catch (Exception ex)
                {

                    lista = new List<ReporteVenta>();
                }

            }

            return lista;


        }

        public bool ActualizarEstadoEntrega(int idVenta, bool estadoEntrega, out string mensaje)
        {
            try
            {
                //Realiza la conexion a la base de datos 
                using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
                {
                    //Recibe como parametro el nombre del procedimiento almacenado 
                    SqlCommand cmd = new SqlCommand("usp_ActualizarEstadoEntrega", oconexion);

                    //Parametros de entrada
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdVenta", idVenta);
                    cmd.Parameters.AddWithValue("@EstadoEntrega", estadoEntrega);

                    SqlParameter resultadoParam = new SqlParameter("@Resultado", SqlDbType.Bit)
                    {
                        Direction = ParameterDirection.Output
                    };
                    SqlParameter mensajeParam = new SqlParameter("@Mensaje", SqlDbType.VarChar, 500)
                    {
                        Direction = ParameterDirection.Output
                    };

                    //Parametros de salida
                    cmd.Parameters.Add(resultadoParam);
                    cmd.Parameters.Add(mensajeParam);

                    oconexion.Open();
                    cmd.ExecuteNonQuery();

                    bool resultado = Convert.ToBoolean(resultadoParam.Value);
                    mensaje = mensajeParam.Value.ToString();

                    return resultado;
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al actualizar el estado: " + ex.Message;
                return false;
            }
        }


        public bool ActualizarEstadoPago(int idVenta, bool estadoPago, out string mensaje)
        {
            try
            {
                //Realiza la conexion a la base de datos 
                using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
                {
                    //Recibe como parametro el nombre del procedimiento almacenado 
                    SqlCommand cmd = new SqlCommand("usp_ActualizarEstadoPago", oconexion);
       
                    cmd.CommandType = CommandType.StoredProcedure;
                    //Parametros de entrada
                    cmd.Parameters.AddWithValue("@IdVenta", idVenta);
                    cmd.Parameters.AddWithValue("@EstadoPago", estadoPago);

                    SqlParameter resultadoParam = new SqlParameter("@Resultado", SqlDbType.Bit)
                    {
                        Direction = ParameterDirection.Output
                    };
                    SqlParameter mensajeParam = new SqlParameter("@Mensaje", SqlDbType.VarChar, 500)
                    {
                        Direction = ParameterDirection.Output
                    };
                    //Parametros de salida
                    cmd.Parameters.Add(resultadoParam);
                    cmd.Parameters.Add(mensajeParam);

                    oconexion.Open();
                    cmd.ExecuteNonQuery();

                    bool resultado = Convert.ToBoolean(resultadoParam.Value);
                    mensaje = mensajeParam.Value.ToString();

                    return resultado;
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al actualizar el estado: " + ex.Message;
                return false;
            }
        }


    }
}
