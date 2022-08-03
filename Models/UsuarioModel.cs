using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InstaMazz2._0.Models
{
    public class UsuarioModel
    {

        public int IdUsuario { get; set; }

        public string Nombre { get; set; }

        public string UserName { get; set; }

        [Required]
        public string Contraseña { get; set; }

        public string email { get; set; }

        public byte ImagenPerfil { get; set; }

        [Required]
        public string ConfirmarClave { get; set; }

        public string imagenPerf { get; set; }
        public string BioUsuario { get; set; }

    }
}