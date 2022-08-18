using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InstaMazz2._0.Models
{
    public class MChats
    {
        public int ID { get; set; }
        public int IdAmigo { get; set; }
        public int IdUsuEnvio { get; set; }
        public string Mensaje { get; set; }
        public string Fecha { get; set; }
    }
}