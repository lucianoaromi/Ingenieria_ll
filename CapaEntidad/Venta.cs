using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    // Clase que representa una Venta en el sistema
    public class Venta
    {
        // Propiedad que almacena el identificador único de la Venta
        public int IdVenta { get; set; }

        // Propiedad que representa el usuario que realizó la Venta
        public Usuario oUsuario { get; set; }
        public Usuario oUsuario2 { get; set; }

        // Propiedad que indica el tipo de documento asociado a la Venta (ejemplo: Factura, Boleta)
        public string TipoDocumento { get; set; }

        public string NumeroDocumento { get; set; }
        public string DocumentoCliente { get; set; }
        public string ApellidoCliente { get; set; }
        public string NombreCliente { get; set; }
        public decimal MontoPago { get; set; }
        public decimal MontoCambio { get; set; }
        public decimal MontoTotal { get; set; }
        public string DesMetPago { get; set; }

        // Lista que contiene los detalles específicos de los productos vendidos
        public List<Detalle_Venta> oDetalle_Venta { get; set; }

        public string FechaRegistro { get; set; }

        //public int EstadoEntrega { get; set; } // 1: Entregado, 0: No entregado
    }

}
