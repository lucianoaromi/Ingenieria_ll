using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class CE_Licencia
    {
        public int ID { get; set; }
        public DateTime FechaInicio { get; set; }
        public int DiasPermitidos { get; set; }
        public bool Activado { get; set; }
        public string CodigoActivacion { get; set; }
        public DateTime? FechaActivacion { get; set; }
        public DateTime? UltimaVerificacion { get; set; }
    }
}
