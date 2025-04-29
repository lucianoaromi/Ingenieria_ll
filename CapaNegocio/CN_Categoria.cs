using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    public class CN_Categoria
    {
        // "objcd_usuario" es una instancia de la clase CD_Usuario llamada objcd_usuario
        private CD_Categoria objcd_categoria = new CD_Categoria();

        //Mismo metodo "Listar" que se halla en la clase CD_Usuario de la capa de datos
        //Retorna la lista que posee la clase "CD_Usuario" que se encuentra en la capa de datos
        public List<Categoria> Listar()
        {
            return objcd_categoria.Listar();

        }

        //Puente de comunicacion con la "Capa de Presentacion"
        public int Registrar(Categoria obj, out string Mensaje)
        {
            Mensaje = string.Empty;


            if (string.IsNullOrEmpty(obj.Descripcion))
            {
                Mensaje += "Es necesario la 'Descripcion' de la categoria\n";
            }



            if (Mensaje != string.Empty)
            {
                return 0;
            }
            else
            {
                return objcd_categoria.Registrar(obj, out Mensaje);
            }
        }



        //Puente de comunicacion con la "Capa de Presentacion"
        public bool Editar(Categoria obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (obj.Descripcion == "")
            {
                Mensaje += "Es necesario la 'Descripcion' de la categoria\n";
            }

            if (Mensaje != string.Empty)
            {
                return false;
            }
            else
            {
                return objcd_categoria.Editar(obj, out Mensaje);
            }

        }


        //Puente de comunicacion con la "Capa de Presentacion"
        public bool Eliminar(Categoria obj, out string Mensaje)
        {
            return objcd_categoria.Eliminar(obj, out Mensaje);

        }
    }
}
