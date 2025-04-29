using CapaEntidad;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;



namespace CapaDatos
{
    public class CD_Permiso
    {
        //Lista una lista de Permisos
        public List<Permiso> Listar(int idusuario)
        {
            //Lista de objetos de tipo Permiso
            List<Permiso> lista = new List<Permiso>();

            //Se conecta a la BD mediante la "cadena de conexion"
            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    //StringBuilder es una clase  utiliza para construir y manipular cadenas de texto de manera eficiente.
                    StringBuilder consultita = new StringBuilder();


                    // Se agrega la consulta SQL al StringBuilder. "AppendLine" es un método de la clase StringBuilder
                    // que se utiliza para agregar una línea de texto al final del objeto StringBuilder.
                    // En este caso, la línea de texto que se agrega es la consulta SQL.

                    //consulta de relacion entre el IdRol y el NombreMenu de la tabla Permiso. La letra "p" especifica que es de la tabla Permiso
                    consultita.AppendLine("select p.IdRol, p.NombreMenu from PERMISO p");

                    //La operación de unión se realiza entre dos tablas: PERMISO (con alias p) y ROL (con alias r)
                    //La tabla resultante de esta operación contendrá datos de ambas tablas
                    consultita.AppendLine("inner join ROL r on r.IdRol = p.IdRol");
                    consultita.AppendLine("inner join USUARIO u on u.IdRol = r.IdRol");
                    consultita.AppendLine("where u.IdUsuario = @idusuario");
                    SqlCommand cmd = new SqlCommand(consultita.ToString(), oconexion);
                    cmd.Parameters.AddWithValue("@idusuario", idusuario);
                    cmd.CommandType = CommandType.Text;

                    oconexion.Open();

                    //cmd.ExecuteReader ejecuta la consulta SQL y devuelve un SqlDataReader
                    //que permite leer los resultados de la consulta.
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            //se crea una nueva instancia de la clase Permiso y se inicializa con los datos
                            //de la fila actual del SqlDataReader.
                            lista.Add(new Permiso()
                            {
                                //se crea un objeto Rol y se inicializa con el valor del campo "IdRol" en la fila actual 
                                //se convierte el valor a un entero antes de asignarlo
                                oRol = new Rol() { IdRol = Convert.ToInt32(dr["IdRol"]) },
                                NombreMenu = dr["NombreMenu"].ToString(),
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    lista = new List<Permiso>();
                }
            }
            return lista;
        }
    }
}
