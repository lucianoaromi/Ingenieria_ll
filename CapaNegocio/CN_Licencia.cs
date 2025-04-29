using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaEntidad;
using CapaDatos;


namespace CapaNegocio
{
    public class CN_Licencia
    {
        private readonly CD_Licencia _softwareStateDAL;

        public CN_Licencia()
        {
            _softwareStateDAL = new CD_Licencia();
        }

        // Método para verificar si el software puede usarse y actualizar los días permitidos
        public bool VerificarUsoPermitido(out int diasRestantes)
        {
            var estado = _softwareStateDAL.ObtenerEstado(); // Trae el estado actual del software

            if (estado == null)
            {
                throw new InvalidOperationException("El estado del software no está configurado.");
            }

            // Si el software está activado permanentemente
            if (estado.Activado)
            {
                diasRestantes = -1;  // -1 para indicar que el software está activado permanentemente
                return true; // El software está activado permanentemente
            }

            // Calcular los días restantes basados en la fecha de inicio y los días permitidos
            DateTime fechaFin = estado.FechaInicio.AddDays(estado.DiasPermitidos);
            diasRestantes = (fechaFin - DateTime.Now).Days;

            // Verificar si el software todavía puede usarse
            if (DateTime.Now > fechaFin)
            {
                diasRestantes = 0; // Si ya no puede usarse, los días restantes son 0
                return false; // El software ya no puede usarse
            }

            // Si aún puede usarse, actualizar los días permitidos en la base de datos
            // Este paso ya es realizado en el método ObtenerEstado dentro de la capa de datos
            return true; // El software todavía puede usarse
        }


        // Método para activar el software con un código
        public bool ActivarSoftware(string codigo)
        {
            var estado = _softwareStateDAL.ObtenerEstado();

            if (estado == null)
            {
                throw new InvalidOperationException("El estado del software no está configurado.");
            }

            // Verifica si ya está activado
            if (estado.Activado)
            {
                throw new InvalidOperationException("El software ya está activado.");
            }

            // Verifica si el código de activación es correcto
            if (estado.CodigoActivacion == codigo)
            {
                // Actualiza el estado a activado y registra la fecha de activación
                bool resultado = _softwareStateDAL.ActualizarEstado(activado: true, fechaActivacion: DateTime.Now, codigo: estado.CodigoActivacion);

                if (resultado)
                {
                    Console.WriteLine("El software ha sido activado correctamente.");
                    return true;
                }
                else
                {
                    Console.WriteLine("Hubo un problema al actualizar el estado de activación.");
                    return false;
                }
            }

            return false; // Código incorrecto
        }

    }
}


