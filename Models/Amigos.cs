using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InstaMazz2._0.Models
{
    public class Amigos
    {
        public int Id { get; set; }
        public int IdUsu { get; set; }
        public int IdUsuAmigo { get; set; }
        public int Activo { get; set; } = 3;
    }
}