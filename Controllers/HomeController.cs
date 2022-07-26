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

            using (SqlConnection cn = new SqlConnection(cadena))
            {
                var usu = Session["usuario"];

                var cmd = new SqlCommand("sp_Get_DataUser", cn);
                cmd.Parameters.AddWithValue("idEmail", Session["usuario"]);
                cmd.CommandType = CommandType.StoredProcedure;

                //cmd.CommandType = CommandType.Text;
                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        ViewBag.name = dr["Nombre"].ToString();
                        ViewBag.email = dr["email"].ToString();
                        ViewBag.userName = dr["UserName"].ToString();

                        byte[] _byteImg = (byte[])dr["ImagenPerfil"];
                        var _byteString = System.Text.Encoding.Default.GetString(_byteImg);
                        var _stringTobyte = _byteString;
                        //ViewBag.Img = byteArrayToImage(_byteImg);

                        ViewBag.Img = "../..//Views/Upload/" + _byteString;
                        //ViewBag.Img = "C:/Users/hp/Documents/GitHub/InstaMazz2.0/Views/Upload/" + _byteString;
                    }
                }
            }

            return View();
        }

        public Image byteArrayToImage(byte[] bytesArr)
        {
            using (MemoryStream memstr = new MemoryStream(bytesArr))
            {
                Image img = Image.FromStream(memstr);
                return img;
            }
        }

        public ActionResult CerrarSesion()
        {
            Session["usuario"] = null;
            return RedirectToAction("Login", "Acceso");
        }


    }
}