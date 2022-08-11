using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InstaMazz2._0.Models
{
    public class PublicacionesModel
    {
        UsuarioModel nam = new UsuarioModel();

        public int IdPost { get; set; }
        public int IdUsuario { get; set; }
        public string UrlImg { get; set; }
        public string Descripcion { get; set; }
        public string UserName { get; set; }

        //solo para el total de post....
        public int TotalPost { get; set; }
        public string Titulo { get; set; }




        //private string Name(string n)
        //{
        //    n = nam.UserName;

        //    return n;
        //}
    }
}