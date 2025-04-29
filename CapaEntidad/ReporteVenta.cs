using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class ReporteVenta
    {
        public string FechaRegistro { get; set; }
        public string TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string MontoTotal { get; set; }
        public string UsuarioRegistro { get; set; }
        public string ApellidoCliente { get; set; }
        public string DesMetPago { get; set; }
        public string EstadoEntrega { get; set; }  // Nuevo campo para almacenar el estado de entrega
        public string EstadoPago { get; set; }  // Nuevo campo para almacenar el estado de pagp
        public string IdVenta { get; set; }
    }
}
