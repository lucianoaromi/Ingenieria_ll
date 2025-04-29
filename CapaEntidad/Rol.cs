using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    // Clase que representa un Rol en el sistema
    public class Rol
    {
        // Propiedad que almacena el identificador único del Rol
        public int IdRol { get; set; }

        // Propiedad que almacena la descripción del Rol
        public string Descripcion { get; set; }

        // Propiedad que almacena la fecha de registro del Rol en formato string
        public string FechaRegistro { get; set; }
    }
}
