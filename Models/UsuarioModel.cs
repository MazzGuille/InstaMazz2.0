using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InstaMazz2._0.Models
{
    public class UsuarioModel
    {

        public string Name()
        {
            return Nombre;
        }


        public int IdUsuario { get; set; }


        public string Nombre { get; set; }


        public string UserName { get; set; }


        public string Contraseña { get; set; }


        public string email { get; set; }



        public string ConfirmarClave { get; set; }

    }
}