using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    public class CN_AdmLicencia
    {
        // "objcd_licencia" es una instancia de la clase CD_AdmLicencia llamada objcd_licencia
        private CD_AdmLicencia objcd_licencia = new CD_AdmLicencia();

        //Mismo metodo "Listar" que se halla en la clase CD_AdmLicencia de la capa de datos
        //Retorna la lista que posee la clase "CD_AdmLicencia" que se encuentra en la capa de datos
        public List<CE_Licencia> Listar()
        {
            return objcd_licencia.Listar();

        }

        //Puente de comunicacion con la "Capa de Presentacion"
        public int Registrar(CE_Licencia obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (string.IsNullOrEmpty(obj.CodigoActivacion))
            {
                Mensaje += "Es necesario el Licencia\n";
            }
            else if (obj.CodigoActivacion.Length != 8 || !EsNumero(obj.CodigoActivacion))
            {
                Mensaje += "El número de Licencia debe tener exactamente 8 dígitos numéricos\n";
            }


            if (Mensaje != string.Empty)
            {
                return 0;
            }
            else
            {
                return objcd_licencia.Registrar(obj, out Mensaje);
            }
        }

        private bool EsNumero(string valor)
        {
            foreach (char c in valor)
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }
            return true;
        }


        //Puente de comunicacion con la "Capa de Presentacion"
        public bool Editar(CE_Licencia obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (obj.CodigoActivacion == "")
            {
                Mensaje += "Es necesario el CodigoActivacion\n";
            }


            if (Mensaje != string.Empty)
            {
                return false;
            }
            else
            {
                return objcd_licencia.Editar(obj, out Mensaje);
            }

        }


        //Puente de comunicacion con la "Capa de Presentacion"
        public bool Eliminar(CE_Licencia obj, out string Mensaje)
        {
            return objcd_licencia.Eliminar(obj, out Mensaje);

        }
    }
}
