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

using InstaMazz2._0.Models;

namespace InstaMazz2._0.Controllers
{

    public class AccesoController : Controller
    {

        static string cadena = " Data Source=(local); Initial Catalog = InstaMazz; Integrated Security = true;";



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
        public ActionResult Registrar(HttpPostedFileBase FileText, UsuarioModel oUsuario)
        {
            var img = oUsuario.UserName + Path.GetFileName(FileText.FileName);
            var path = Path.Combine(Server.MapPath("~/Views/Upload"), img);
            FileText.SaveAs(path);

            return View();


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

            //if (oUsuario.Contraseña == oUsuario.ConfirmarClave)
            //{
            //    oUsuario.Contraseña = ConvertirSHA256(oUsuario.Contraseña);
            //}
            //else
            //{
            //    ViewData["Mensaje"] = "Las contraseñas no coinciden";
            //    return View();
            //}


            ////byte[] imagen = 

            //using (SqlConnection cn = new SqlConnection(cadena))
            //{
            //    var cmd = new SqlCommand("sp_RegistrarUsuario", cn);
            //    cmd.Parameters.AddWithValue("Nombre", oUsuario.Nombre);
            //    cmd.Parameters.AddWithValue("UserName", oUsuario.UserName);
            //    cmd.Parameters.AddWithValue("email", oUsuario.email);
            //    cmd.Parameters.AddWithValue("Contraseña", oUsuario.Contraseña);
            //    cmd.Parameters.AddWithValue("ImagenPerfil", oUsuario.ImagenPerfil);
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