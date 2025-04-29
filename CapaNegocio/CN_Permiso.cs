using CapaDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CapaEntidad;


namespace CapaNegocio
{
    public class CN_Permiso
    {
        //crea una instancia privada de la clase CD_Permiso
        private CD_Permiso objcd_permiso = new CD_Permiso();

        //Metodo que recuperar los permisos que están asociados a un usuario en particular.
        public List<Permiso> Listar(int IdUsuario)
        {
            //retorno una lista de permisos 
            return objcd_permiso.Listar(IdUsuario);
        }


    }
}

