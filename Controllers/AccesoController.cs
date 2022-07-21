using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNetCore.Hosting;

using InstaMazz2._0.Models;
using System.Web.Helpers;
using Windows.Storage.Pickers;
using System.Drawing;

namespace InstaMazz2._0.Controllers
{
    public class AccesoController : Controller
    {

        static string cadena = " Data Source=(local); Initial Catalog = InstaMazz; Integrated Security = true;";
        //COMENTARIO DE PRUEBA 2

        private static byte[] Convertir_Img_Bytes(Image img)
        {
            string sTemp = Path.GetTempFileName();
            FileStream fs = new FileStream(sTemp, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            img.Save(fs, System.Drawing.Imaging.ImageFormat.Png);
            fs.Position = 0;

            int imgLength = Convert.ToInt32(fs.Length);
            byte[] bytes = new byte[imgLength];
            fs.Read(bytes, 0, imgLength);
            fs.Close();
            return bytes;
        }

        // GET: Acceso
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(UsuarioModel oUsuario)
        {
            if (oUsuario.email == "")
            {
                ViewBag.missingMail = "El campo E-Mail no puede estar vacio";
            }

            if (oUsuario.Contraseña == null)
            {
                ViewBag.missingPassword = "El campo Contraseña no puede estar vacio";
            }


            oUsuario.Contraseña = ConvertirSHA256(oUsuario.Contraseña);

            using (SqlConnection cn = new SqlConnection(cadena))
            {
                var cmd = new SqlCommand("sp_ValidarUsuario", cn);
                cmd.Parameters.AddWithValue("email", oUsuario.email);
                cmd.Parameters.AddWithValue("Contraseña", oUsuario.Contraseña);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                oUsuario.IdUsuario = Convert.ToInt32(cmd.ExecuteScalar().ToString());
                var result = oUsuario.IdUsuario;
            }


            if (oUsuario.IdUsuario != 0)
            {
                var usu = oUsuario.email;
                Session["usuario"] = usu;

                var IdUsu = oUsuario.IdUsuario;
                Session["IdUsuario"] = IdUsu;


                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewData["Mensaje"] = "Usuario o contraseña incorrectos";
                return View();
            }
        }


        public ActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registrar(UsuarioModel oUsuario)
        {
            //obtenemos la imagen seleccionada...
            HttpPostedFileBase ImgPerfil = Request.Files[0];
            //mandamos la imagen obtenida al siguiente carpeta...
            var str = System.IO.Path.Combine(Server.MapPath("~/Views/Upload"), ImgPerfil.FileName);
            //copiamos la imagen seleccionada...
            ImgPerfil.InputStream.CopyToAsync(new System.IO.FileStream(str, System.IO.FileMode.Create));

            byte[] _imagen = System.IO.File.ReadAllBytes(str);

            var resul = _imagen;
           
            return View();

            //Convertir_Img_Bytes(ImgPerfil);

            ////Convertimos la imagen a bytes
            //var bytes = new byte[ImgPerfil.ContentLength];


            //WebImage image = new WebImage(ImgPerfil.InputStream);

            //oUsuario.ImagenPerfil = image.GetBytes();
            //byte[] imagen = image.GetBytes();

            //var resul = imagen;

            //return View();




            //bool registrado;
            //string mensaje;
            //bool email;

            //email = oUsuario.email.Contains('.');

            //if (email == false)
            //{
            //    ViewData["email"] = "Formato de E-Mail invalido";

            //    return View();
            //}

            //if (oUsuario.Nombre.Length < 3 || oUsuario.Nombre.Length > 30)
            //{
            //    ViewData["Nombre"] = "El nombre debe tener entre 3 y 20 caracteres";


            //    return View();
            //}

            //if (oUsuario.UserName.Length < 3 || oUsuario.UserName.Length > 10)
            //{
            //    ViewData["NombreUsuario"] = "El nombre de usuario debe tener entre 3 y 10 caracteres";

            //    return View();
            //}

            ////if (registrado)
            ////{

            ////    return RedirectToAction("Login", "Acceso");
            ////}
            ////else
            ////{
            ////    return View();

            ////}

            //if (oUsuario.Contraseña == oUsuario.ConfirmarClave)
            //{
            //    oUsuario.Contraseña = ConvertirSHA256(oUsuario.Contraseña);
            //}
            //else
            //{
            //    ViewData["Mensaje"] = "Las contraseñas no coinciden";
            //    return View();
            //}

            //using (SqlConnection cn = new SqlConnection(cadena))
            //{
            //    var cmd = new SqlCommand("sp_RegistrarUsuario", cn);
            //    cmd.Parameters.AddWithValue("Nombre", oUsuario.Nombre);
            //    cmd.Parameters.AddWithValue("UserName", oUsuario.UserName);
            //    cmd.Parameters.AddWithValue("email", oUsuario.email);
            //    cmd.Parameters.AddWithValue("Contraseña", oUsuario.Contraseña);
            //    //cmd.Parameters.AddWithValue("ImagenPerfil", bytes);
            //    cmd.Parameters.Add("Registrado", SqlDbType.Bit).Direction = ParameterDirection.Output;
            //    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
            //    cmd.CommandType = CommandType.StoredProcedure;

            //    cn.Open();

            //    cmd.ExecuteNonQuery();

            //    registrado = (bool)cmd.Parameters["Registrado"].Value;
            //    mensaje = (string)cmd.Parameters["Mensaje"].Value;
            //}

            //ViewData["Mensaje"] = mensaje;

            //if (registrado)
            //{

            //    return RedirectToAction("Login", "Acceso");
            //}
            //else
            //{
            //    return View();

            //}
        }


        public ActionResult EditarPerfil()
        {
            var IdUsuario = (int)Session["IdUsuario"];
            ViewBag.IdUsuario = IdUsuario;
            return View();

        }

        [HttpPost]
        public ActionResult EditarPerfil(UsuarioModel oUsario)
        {
            oUsario.Contraseña = ConvertirSHA256(oUsario.Contraseña);

            using (SqlConnection cn = new SqlConnection(cadena))
            {
                var cmd = new SqlCommand("sp_Editar", cn);
                cmd.Parameters.AddWithValue("IdUsuario", oUsario.IdUsuario);
                cmd.Parameters.AddWithValue("Nombre", oUsario.Nombre);
                cmd.Parameters.AddWithValue("UserName", oUsario.UserName);
                cmd.Parameters.AddWithValue("email", oUsario.email);
                cmd.Parameters.AddWithValue("Contraseña", oUsario.Contraseña);
                //cmd.Parameters.AddWithValue("ImagenPerfil", oUsario.ImagenPerfil);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                cmd.ExecuteNonQuery();

                ViewBag.Succes = "Perfil editado correctamente";
            }

            return RedirectToAction("Index", "Home");
        }

        public static string ConvertirSHA256(string text)
        {
            if (text == null)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder();
            using (SHA256 hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(text));
                foreach (byte b in result)
                    sb.Append(b.ToString("x2"));
            }
            return sb.ToString();

        }
    }
}