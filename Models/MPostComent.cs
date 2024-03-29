﻿using System;
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

        //COLOCAR PARA MOSTRAR EL NOMBRE DEL USUARIO...
        public string UserName { get; set; }

        //PARA MOSTRAR LA IMG DEL USUARIO QUE COMENTA EL POST...
        public byte ImagenPerfil { get; set; }
        public string imagenPerf { get; set; }

        //IMAGEN POR DEFECTO...
        public bool ceroImg { get; set; }

        //ENVIAR EL COMENTARIO DEL POST..
        public string Mensaje { get; set; }
    }
}