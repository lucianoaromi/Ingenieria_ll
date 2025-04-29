using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace CapaDatos
{
    public class CD_Licencia
    {
        //private readonly string connectionString = "Data Source=LUCIANO320\\LUCIANO320;Initial Catalog=LICENCIA_DB;Integrated Security=True;Encrypt=False";

        // Método para obtener el estado del software y actualizar los días permitidos
        public CE_Licencia ObtenerEstado()
        {
            CE_Licencia estado = null;

            using (SqlConnection conn = new SqlConnection(Conexion.cadena))//Conexion.cadena
            {
                conn.Open();

                // Obtener los datos actuales de la base de datos
                string query = "SELECT FechaInicio, DiasPermitidos, Activado, CodigoActivacion, FechaActivacion, UltimaVerificacion FROM SoftwareState";
                SqlCommand cmd = new SqlCommand(query, conn);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        estado = new CE_Licencia
                        {
                            FechaInicio = Convert.ToDateTime(reader["FechaInicio"]),
                            DiasPermitidos = Convert.ToInt32(reader["DiasPermitidos"]),
                            Activado = Convert.ToBoolean(reader["Activado"]),
                            CodigoActivacion = reader["CodigoActivacion"].ToString(),
                            FechaActivacion = reader["FechaActivacion"] as DateTime?,
                            UltimaVerificacion = reader["UltimaVerificacion"] as DateTime?
                        };
                    }
                }

                // Lógica para actualizar los días restantes basados en la fecha actual
                if (estado != null)
                {
                    // Verificar si hay una última verificación para evitar operaciones innecesarias
                    if (estado.UltimaVerificacion == null || estado.UltimaVerificacion.Value.Date < DateTime.Now.Date)
                    {
                        // Calcular la cantidad de días transcurridos desde la última verificación
                        int diasTranscurridos = (DateTime.Now.Date - estado.FechaInicio.Date).Days;

                        // Restar los días transcurridos de los días permitidos
                        int nuevosDiasPermitidos = estado.DiasPermitidos - diasTranscurridos;

                        // Si los días restantes son negativos, se establecen en 0
                        nuevosDiasPermitidos = Math.Max(nuevosDiasPermitidos, 0);

                        // Actualizar el valor de los días permitidos en la base de datos
                        string updateQuery = "UPDATE SoftwareState SET DiasPermitidos = @DiasPermitidos, UltimaVerificacion = @UltimaVerificacion WHERE ID = 1"; // Se asume que solo hay un registro
                        SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
                        updateCmd.Parameters.AddWithValue("@DiasPermitidos", nuevosDiasPermitidos);
                        updateCmd.Parameters.AddWithValue("@UltimaVerificacion", DateTime.Now);

                        updateCmd.ExecuteNonQuery();

                        // Actualizar el valor en la entidad
                        estado.DiasPermitidos = nuevosDiasPermitidos;
                        estado.UltimaVerificacion = DateTime.Now;
                    }
                }
            }

            return estado;
        }


        public bool ActualizarEstado(bool activado, DateTime? fechaActivacion = null, string codigo = null)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Conexion.cadena))
                {
                    conn.Open();

                    // Asegúrate de que la consulta esté usando el identificador único para actualizar
                    string query = "UPDATE SoftwareState SET Activado = @Activado, FechaActivacion = @FechaActivacion, CodigoActivacion = @Codigo WHERE CodigoActivacion = @Codigo";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Activado", activado);
                        cmd.Parameters.AddWithValue("@FechaActivacion", fechaActivacion.HasValue ? (object)fechaActivacion.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@Codigo", !string.IsNullOrEmpty(codigo) ? (object)codigo : DBNull.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        // Retorna true si se actualizó al menos una fila, de lo contrario false
                        return rowsAffected > 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                // Manejo de excepciones
                Console.WriteLine("Error de base de datos: " + ex.Message);
                return false; // Retorna false si ocurrió un error
            }
        }

    }
}
