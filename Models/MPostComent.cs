using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InstaMazz2._0.Models
{
    public class MPostComent
    {
        public int IdPost { get; set; }
        public string Coment { get; set; }
        public string Fecha { get; set; }
        public bool Activo { get; set; }
    }
}