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
        // "objcd_reporte" es una instancia de la clase CD_Producto llamada objcd_reporte
        private CD_Reporte objcd_reporte = new CD_Reporte();
        private object cadena;

        public CN_Reporte(object cadena)
        {
            this.cadena=cadena;
        }

        public CN_Reporte()
        {
        }

        //Mismo metodo "Venta" que se halla en la clase CD_Reporte de la capa de datos
        //Retorna la lista que posee la clase "CD_Reporte" que se encuentra en la capa de datos
        public List<ReporteVenta> Venta(string fechainicio, string fechafin, int idusuario)
        {
            return objcd_reporte.Venta(fechainicio, fechafin, idusuario);
        }


        //Puente de comunicacion con la "Capa de Presentacion"
        public bool ActualizarEstadoVenta(int idVenta, bool nuevoEstado, out string mensaje)
        {
            return objcd_reporte.ActualizarEstadoEntrega(idVenta, nuevoEstado, out mensaje);
        }

        //Puente de comunicacion con la "Capa de Presentacion"
        public bool ActualizarEstadoPago(int idVenta, bool nuevoEstado, out string mensaje)
        {
            return objcd_reporte.ActualizarEstadoPago(idVenta, nuevoEstado, out mensaje);
        }
    }
}
