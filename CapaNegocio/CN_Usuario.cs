using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using CapaDatos;
using CapaEntidad;


//Puente de comunicacion con la "Capa de Presentacion"

namespace CapaNegocio
{
    public class CN_Usuario
    {
        // "objcd_usuario" es una instancia de la clase CD_Usuario llamada objcd_usuario
        private CD_Usuario objcd_usuario = new CD_Usuario();

        //Mismo metodo "Listar" que se halla en la clase CD_Usuario de la capa de datos
        //Retorna la lista que posee la clase "CD_Usuario" que se encuentra en la capa de datos
        public List<Usuario> Listar()
        {
            return objcd_usuario.Listar();
        }

        //--------------------------------------------------------------------------------------------------------

        public int Registrar(Usuario obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (string.IsNullOrEmpty(obj.Documento))
            {
                Mensaje += "Es necesario el Documento del usuario\n";
            }
            else if (obj.Documento.Length != 8 || !EsNumero(obj.Documento)) // 8 digitos de longitud y que sea numero
            {
                Mensaje += "El número de Documento debe tener exactamente 8 dígitos numéricos\n";
            }

            if (string.IsNullOrEmpty(obj.Apellido))
            {
                Mensaje += "Es necesario el Apellido del usuario\n";
            }

            if (string.IsNullOrEmpty(obj.Nombre))
            {
                Mensaje += "Es necesario el Nombre del usuario\n";
            }

            if (string.IsNullOrEmpty(obj.Clave))
            {
                Mensaje += "Es necesario la Contraseña del usuario\n";
            }

            if (Mensaje != string.Empty)
            {
                return 0;
            }
            else
            {
                // Llama al método 'Registrar' del objeto 'objcd_usuario', pasando el objeto 'obj' 
                // y una variable de salida 'Mensaje' para obtener información adicional.
                // Retorna el resultado de la operación.
                return objcd_usuario.Registrar(obj, out Mensaje);
            }
        }

        //--------------------------------------------------------------------------------------------------------

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

        //--------------------------------------------------------------------------------------------------------

        public bool Editar(Usuario obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (obj.Documento == "")
            {
                Mensaje += "Es necesario el Documento del usuario\n";
            }
            else if (obj.Documento.Length != 8 || !EsNumero(obj.Documento)) // 8 digitos de longitud y que sea numero
            {
                Mensaje += "El número de Documento debe tener exactamente 8 dígitos numéricos\n";
            }

            if (obj.Apellido == "")
            {
                Mensaje += "Es necesario el Apellido del usuario\n";
            }

            if (obj.Nombre == "")
            {
                Mensaje += "Es necesario el Nombre del usuario\n";
            }

            if (obj.Clave == "")
            {
                Mensaje += "Es necesario la Contraseña del usuario\n";
            }


            if (Mensaje != string.Empty)
            {
                return false;
            }
            else
            {
                return objcd_usuario.Editar(obj, out Mensaje);
            }

        }

        //--------------------------------------------------------------------------------------------------------

        public bool Eliminar(Usuario obj, out string Mensaje)
        {
            return objcd_usuario.Eliminar(obj, out Mensaje);
        }
    }
}