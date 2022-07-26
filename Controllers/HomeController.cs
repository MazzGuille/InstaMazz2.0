﻿using InstaMazz2._0.Permisos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InstaMazz2._0.Models;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Drawing;
using System.Configuration;

namespace InstaMazz2._0.Controllers
{
    [ValidarSesion]
    public class HomeController : Controller
    {
        string cadena = ConfigurationManager.ConnectionStrings["InstaMaczzDB"].ConnectionString;
        public ActionResult Index()
        {
            UsuarioModel model = new UsuarioModel();

            using (SqlConnection cn = new SqlConnection(cadena))
            {
                var cmd = new SqlCommand("sp_Get_DataUser", cn);
                cmd.Parameters.AddWithValue("idEmail", Session["usuario"]);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        //tratando de mostrar imagen en la vista, desde bytes a img.
                        byte[] _byteImg = (byte[])dr["ImagenPerfil"];
                        var _byteString = System.Text.Encoding.Default.GetString(_byteImg);

                        ViewBag.Img = "../..//Views/Upload/" + _byteString;
                        //ViewBag.Img = "C:/Users/hp/Documents/GitHub/InstaMazz2.0/Views/Upload/" + _byteString;
                    }
                }
            }

            return View(model);
        }

        //public ActionResult converetImagen(string correoId)
        //{
        //    //cadena de conexión...
        //    using (SqlConnection cn = new SqlConnection(cadena))
        //    {
        //        var cmd = new SqlCommand("SP_GET_ImgPerfil", cn);
        //        cmd.Parameters.AddWithValue("emailId", correoId);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cn.Open();
        //        using (SqlDataReader dr = cmd.ExecuteReader())
        //        {
        //            byte[] _byteImg = (byte[])dr["ImagenPerfil"];
        //            var _byteString = System.Text.Encoding.Default.GetString(_byteImg);

        //            return File(_byteString, "imagenes/jpg");
        //        }
        //    }
        //}

        public ActionResult CerrarSesion()
        {
            Session["usuario"] = null;
            return RedirectToAction("Login", "Acceso");
        }


    }
}