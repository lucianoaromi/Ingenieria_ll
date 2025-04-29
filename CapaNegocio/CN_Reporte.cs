using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Reporte
    {
        private CD_Reporte objcd_reporte = new CD_Reporte();
        private object cadena;

        public CN_Reporte(object cadena)
        {
            this.cadena=cadena;
        }

        public CN_Reporte()
        {
        }

        public List<ReporteVenta> Venta(string fechainicio, string fechafin, int idusuario)
        {
            return objcd_reporte.Venta(fechainicio, fechafin, idusuario);
        }


        public bool ActualizarEstadoVenta(int idVenta, bool nuevoEstado, out string mensaje)
        {
            return objcd_reporte.ActualizarEstadoEntrega(idVenta, nuevoEstado, out mensaje);
        }

        public bool ActualizarEstadoPago(int idVenta, bool nuevoEstado, out string mensaje)
        {
            return objcd_reporte.ActualizarEstadoPago(idVenta, nuevoEstado, out mensaje);
        }
    }
}
