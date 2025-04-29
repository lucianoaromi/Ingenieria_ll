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
    public class CD_Producto
    {
        // Metodo que permite listar todos los usuarios desde la base de datos
        public List<Producto> Listar()
        {
            //Genera la variable de tipo Lista que contiene Usuarios
            List<Producto> lista = new List<Producto>();

            //Conecta a la base por medio de la "cadena de conexion" -> (Conexiones.(clase en la capa de datos))
            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {

                //Capturador de errores por si falla la conexion a la BD
                try
                {
                    StringBuilder query = new StringBuilder();

                    //Consulta a la BD la tabla Producto y Categoria
                    query.AppendLine("select IdProducto,Codigo,Nombre,p.Descripcion,c.IdCategoria,c.Descripcion[DescripcionCategoria],p.Stock,p.Precio,p.Estado from PRODUCTO p");

                    //Consulta a la BD la tabla Producto y Categoria, relacionando su IdCategoria
                    query.AppendLine("inner join CATEGORIA c on c.IdCategoria = p.IdCategoria");

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
                            lista.Add(new Producto()
                            {
                                IdProducto = Convert.ToInt32(dr["IdProducto"]),
                                Codigo = dr["Codigo"].ToString(),
                                Nombre = dr["Nombre"].ToString(),
                                Descripcion = dr["Descripcion"].ToString(),
                                oCategoria = new Categoria() { IdCategoria = Convert.ToInt32(dr["IdCategoria"]), Descripcion = dr["DescripcionCategoria"].ToString() },
                                Stock = Convert.ToInt32(dr["Stock"].ToString()),
                                Precio = Convert.ToDecimal(dr["Precio"].ToString()),
                                Estado = Convert.ToBoolean(dr["Estado"])


                            });
                        }
                    }

                }
                //si se produce una excepción se crea una nueva lista de usuarios, se garantiza que lista esté en un estado válido y vacío
                catch (Exception ex)
                {

                    lista = new List<Producto>();
                }

            }

            return lista;


        }

        //Parametros de entrada y salida - "obj" objeto declaro de tipo usuario
        public int Registrar(Producto obj, out string Mensaje)
        {
            int idproductogenerado = 0;
            Mensaje = string.Empty;

            try
            {
                //Realiza la conexion a la base de datos con la cadena de conexion
                using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
                {
                    //Recibe como parametro el nombre del procedimiento almacenado 
                    SqlCommand cmd = new SqlCommand("SP_REGISTRARPRODUCTO".ToString(), oconexion);

                    //Parametros de entrada
                    cmd.Parameters.AddWithValue("Codigo", obj.Codigo);
                    cmd.Parameters.AddWithValue("Nombre", obj.Nombre);
                    cmd.Parameters.AddWithValue("Descripcion", obj.Descripcion);
                    cmd.Parameters.AddWithValue("IdCategoria", obj.oCategoria.IdCategoria);

                    cmd.Parameters.AddWithValue("Stock", obj.Stock);
                    cmd.Parameters.AddWithValue("Precio", obj.Precio);

                    cmd.Parameters.AddWithValue("Estado", obj.Estado);
                    

                    //Parametros de salida
                    cmd.Parameters.Add("Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;

                    cmd.CommandType = CommandType.StoredProcedure;
                    oconexion.Open();

                    cmd.ExecuteNonQuery();

                    idproductogenerado = Convert.ToInt32(cmd.Parameters["Resultado"].Value);
                    Mensaje = cmd.Parameters["Mensaje"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                idproductogenerado =0;
                Mensaje = ex.Message;


            }

            return idproductogenerado;
        }

        //Parametros de entrada y salida - "obj" objeto declaro de tipo usuario
        public bool Editar(Producto obj, out string Mensaje)
        {
            bool respuesta = false;
            Mensaje = string.Empty;

            try
            {
                //Realiza la conexion a la base de datos con la cadena de conexion
                using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
                {
                    //Recibe como parametro el nombre del procedimiento almacenado 
                    SqlCommand cmd = new SqlCommand("SP_EDITARPRODUCTO".ToString(), oconexion);

                    //Parametros de entrada
                    cmd.Parameters.AddWithValue("IdProducto", obj.IdProducto);
                    cmd.Parameters.AddWithValue("Codigo", obj.Codigo);
                    cmd.Parameters.AddWithValue("Nombre", obj.Nombre);
                    cmd.Parameters.AddWithValue("Descripcion", obj.Descripcion);
                    cmd.Parameters.AddWithValue("IdCategoria", obj.oCategoria.IdCategoria);

                    cmd.Parameters.AddWithValue("Stock", obj.Stock);
                    cmd.Parameters.AddWithValue("Precio", obj.Precio);

                    cmd.Parameters.AddWithValue("Estado", obj.Estado);

                    //Parametros de salida
                    cmd.Parameters.Add("Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;

                    cmd.CommandType = CommandType.StoredProcedure;
                    oconexion.Open();

                    cmd.ExecuteNonQuery();

                    respuesta = Convert.ToBoolean(cmd.Parameters["Resultado"].Value);
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
        public bool Eliminar(Producto obj, out string Mensaje)
        {
            bool respuesta = false;
            Mensaje = string.Empty;

            try
            {
                //Realiza la conexion a la base de datos con la cadena de conexion
                using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
                {
                    //Recibe como parametro el nombre del procedimiento almacenado 
                    SqlCommand cmd = new SqlCommand("SP_ELIMINARPRODUCTO", oconexion);

                    //Parametros de entrada
                    cmd.Parameters.AddWithValue("IdProducto", obj.IdProducto);

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
