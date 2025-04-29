using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    public class CN_Producto
    {
        // "objcd_usuario" es una instancia de la clase CD_Usuario llamada objcd_usuario
        private CD_Producto objcd_producto = new CD_Producto();

        //Mismo metodo "Listar" que se halla en la clase CD_Usuario de la capa de datos
        //Retorna la lista que posee la clase "CD_Usuario" que se encuentra en la capa de datos
        public List<Producto> Listar()
        {
            return objcd_producto.Listar();

        }

        //Puente de comunicacion con la "Capa de Presentacion"
        public int Registrar(Producto obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (obj.Codigo == "")
            {
                Mensaje += "Es necesario el 'Codigo' del producto\n";
            }

            if (string.IsNullOrEmpty(obj.Nombre))
            {
                Mensaje += "Es necesario el 'Nombre' del producto\n";
            }
            else if (obj.Nombre.Length > 50)
            {
                Mensaje += "El Nombre no puede exceder los 50 caracteres.\n";
            }

            if (string.IsNullOrWhiteSpace(obj.Descripcion))
            {
                Mensaje += "Es necesario la 'Descripción' del producto.\n";
            }
            else if (obj.Descripcion.Length > 200)
            {
                Mensaje += "La descripción no puede exceder los 200 caracteres.\n";
            }

            if (obj.Stock < 0)
            {
                Mensaje += "Es necesario el Stock del producto\n";
            }


            if (Mensaje != string.Empty)
            {
                return 0;
            }
            else
            {
                return objcd_producto.Registrar(obj, out Mensaje);
            }
        }



        //Puente de comunicacion con la "Capa de Presentacion"
        public bool Editar(Producto obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (obj.Codigo == "")
            {
                Mensaje += "Es necesario el 'Codigo' del producto\n";
            }

            if (obj.Nombre == "")
            {
                Mensaje += "Es necesario el 'Nombre' del producto\n";
            }
            else if (obj.Nombre.Length > 50)
            {
                Mensaje += "El Nombre no puede exceder los 50 caracteres.\n";
            }

            if (obj.Descripcion == "")
            {
                Mensaje += "Es necesario la 'Descripcion' del producto\n";
            }
            else if (obj.Descripcion.Length > 200)
            {
                Mensaje += "La descripción no puede exceder los 200 caracteres.\n";
            }

            if (obj.Stock < 0)
            {
                Mensaje += "Es necesario el 'Stock' del producto\n";
            }

            if (Mensaje != string.Empty)
            {
                return false;
            }
            else
            {
                return objcd_producto.Editar(obj, out Mensaje);
            }

        }


        //Puente de comunicacion con la "Capa de Presentacion"
        public bool Eliminar(Producto obj, out string Mensaje)
        {
            return objcd_producto.Eliminar(obj, out Mensaje);

        }
    }
}