using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    // Clase que representa un Permiso en el sistema
    public class Permiso
    {
        // Propiedad que almacena el identificador único del Permiso
        public int IdPermiso { get; set; }

        // Propiedad que representa el Rol asociado al Permiso, referenciado a la clase Rol
        public Rol oRol { get; set; }

        // Propiedad que almacena el nombre del menú al que se otorga el Permiso
        public string NombreMenu { get; set; }

        // Propiedad que almacena la fecha de registro del Permiso en formato string
        public string FechaResgistro { get; set; }
    }

}
