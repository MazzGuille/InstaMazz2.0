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
using System.Configuration;

namespace InstaMazz2._0.Controllers
{
    public class AccesoController : Controller
    {

        string cadena = ConfigurationManager.ConnectionStrings["InstaMaczzDB"].ConnectionString;
        //COMENTARIO DE PRUEBA 2

        // GET: Acceso
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(UsuarioModel oUsuario)
        {

            if (string.IsNullOrEmpty(oUsuario.Email))
            {
                ViewBag.MailNull = "El campo \"E-Mail\" es requerido";
                return View();
            }

            if (string.IsNullOrEmpty(oUsuario.Contraseña))
            {
                ViewBag.PassNull = "El campo \"Contraseña\" es requerido";
                return View();
            }

            oUsuario.Contraseña = ConvertirSHA256(oUsuario.Contraseña);

            using (SqlConnection cn = new SqlConnection(cadena))
            {
                var cmd = new SqlCommand("sp_ValidarUsuario", cn);
                cmd.Parameters.AddWithValue("Email", oUsuario.Email);
                cmd.Parameters.AddWithValue("Contraseña", oUsuario.Contraseña);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                oUsuario.IdUsuario = Convert.ToInt32(cmd.ExecuteScalar().ToString());
                var result = oUsuario.IdUsuario;
            }


            if (oUsuario.IdUsuario != 0)
            {
                var usu = oUsuario.Email;
                Session["usuario"] = usu;

                var IdUsu = oUsuario.IdUsuario;
                Session["IdUsuario"] = IdUsu;

                var pass = oUsuario.Contraseña;
                Session["Pass"] = pass;


                return RedirectToAction("FeedView", "Post");
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
            if (string.IsNullOrEmpty(oUsuario.Nombre))
            {
                ViewBag.NombreNull = "El campo \"Nombre\" es requerido";
                return View();
            }

            if (string.IsNullOrEmpty(oUsuario.UserName))
            {
                ViewBag.UserNameNull = "El campo \"Nombre de usuario\" es requerido";
                return View();
            }

            if (string.IsNullOrEmpty(oUsuario.Email))
            {
                ViewBag.MailNull = "El campo \"E-Mail\" es requerido";
                return View();
            }

            if (String.IsNullOrEmpty(oUsuario.Contraseña) || String.IsNullOrEmpty(oUsuario.ConfirmarClave))
            {
                ViewData["Mensaje2"] = "Las contraseñas no pueden estar vacias";
                return View();
            }

            //CODIGO REUTILISABLE... PARA PASAR LA IMAGEN A BYTE...
            HttpPostedFileBase ImgPerfil = Request.Files[0];
            var ruta = Server.MapPath("~/Upload");
            byte[] _byteString = deCadenaToBytes(ImgPerfil, ruta, oUsuario.Email);

            bool registrado;
            string mensaje;
            bool email;

            email = oUsuario.Email.Contains('.');

            if (email == false)
            {
                ViewData["email"] = "Formato de E-Mail invalido";

                return View();
            }

            if (oUsuario.Nombre.Length < 3 || oUsuario.Nombre.Length > 30)
            {
                ViewData["Nombre"] = "El nombre debe tener entre 3 y 20 caracteres";


                return View();
            }

            if (oUsuario.UserName.Length < 3 || oUsuario.UserName.Length > 10)
            {
                ViewData["NombreUsuario"] = "El nombre de usuario debe tener entre 3 y 10 caracteres";

                return View();
            }

            //if (registrado)
            //{

            //    return RedirectToAction("Login", "Acceso");
            //}
            //else
            //{
            //    return View();

            //}

            if (oUsuario.Contraseña == oUsuario.ConfirmarClave)
            {
                oUsuario.Contraseña = ConvertirSHA256(oUsuario.Contraseña);
            }
            else
            {
                ViewData["Mensaje"] = "Las contraseñas no coinciden";
                return View();
            }

            using (SqlConnection cn = new SqlConnection(cadena))
            {
                var cmd = new SqlCommand("sp_RegistrarUsuario", cn);
                cmd.Parameters.AddWithValue("Nombre", oUsuario.Nombre);
                cmd.Parameters.AddWithValue("UserName", oUsuario.UserName);
                cmd.Parameters.AddWithValue("Email", oUsuario.Email);
                cmd.Parameters.AddWithValue("Contraseña", oUsuario.Contraseña);
                cmd.Parameters.AddWithValue("ImagenPerfil", _byteString);//_byteString
                cmd.Parameters.AddWithValue("BioUsuario", oUsuario.BioUsuario);
                cmd.Parameters.Add("Registrado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                cmd.ExecuteNonQuery();

                registrado = (bool)cmd.Parameters["Registrado"].Value;
                mensaje = (string)cmd.Parameters["Mensaje"].Value;
                Session["ImgPerf"] = _byteString;
            }

            ViewData["Mensaje"] = mensaje;

            if (registrado)
            {

                return RedirectToAction("Login", "Acceso");
            }
            else
            {
                return View();
            }

        }


        public ActionResult EditarPerfil()
        {
            var IdUsuario = (int)Session["IdUsuario"];
            if (IdUsuario != 0)
            {
                //si existe la bariable session...
                ViewBag.IdUsuario = IdUsuario;

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

                            model.Nombre = dr["Nombre"].ToString();
                            model.Email = dr["Email"].ToString();
                            model.UserName = dr["UserName"].ToString();
                            model.BioUsuario = dr["BioUsuario"].ToString();
                            model.imagenPerf = _byteString;
                        }
                    }
                }

                ViewBag.Nom = model.Nombre;
                ViewBag.NomUs = model.UserName;
                ViewBag.Celec = model.Email;
                ViewBag.Bio = model.BioUsuario;
                ViewBag.Img = model.imagenPerf;

                return View();
            }
            else
            {
                return Redirect("Index");
            }
        }

        [HttpPost]
        public ActionResult EditarPerfil(UsuarioModel oUsario)
        {
            //CODIGO REUTILISABLE... PARA PASAR LA IMAGEN A BYTE...
            HttpPostedFileBase ImgPerfil = Request.Files[0];
            byte[] _byteString;
            if (ImgPerfil.FileName != "")
            {
                var ruta = Server.MapPath("~/Upload");
                _byteString = deCadenaToBytes(ImgPerfil, ruta, oUsario.Email);
            }
            else
            {
                //mantener igual el byte...
                _byteString = igualByteToByte(oUsario.ImagenPerfil);
            }


            if (string.IsNullOrEmpty(oUsario.Nombre))
            {
                ViewBag.NombreNull = "El campo \"Nombre\" es requerido";
                return View();
            }

            if (string.IsNullOrEmpty(oUsario.UserName))
            {
                ViewBag.UserNameNull = "El campo \"Nombre de usuario\" es requerido";
                return View();
            }

            if (string.IsNullOrEmpty(oUsario.BioUsuario))
            {
                ViewBag.BioNull = "El campo \"Biografia\" es requerido";
                return View();
            }

            using (SqlConnection cn = new SqlConnection(cadena))
            {
                var cmd = new SqlCommand("sp_Editar", cn);
                cmd.Parameters.AddWithValue("IdUsuario", oUsario.IdUsuario);
                cmd.Parameters.AddWithValue("Nombre", oUsario.Nombre);
                cmd.Parameters.AddWithValue("UserName", oUsario.UserName);
                cmd.Parameters.AddWithValue("ImagenPerfil", _byteString);
                cmd.Parameters.AddWithValue("BioUsuario", oUsario.BioUsuario);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index", "Home", new { idE = Session["usuario"] });
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

        private static byte[] deCadenaToBytes(HttpPostedFileBase ImgPerfil, string Server, string email)
        {
            //HttpPostedFileBase ImgPerfil = Request.Files[0];

            //Colocar el nombre de la Img + email.
            var _newNameImg = email + '_' + Path.GetFileName(ImgPerfil.FileName);//System.IO.Path.GetFileName(ImgPerfil.FileName);

            //mandamos la imagen obtenida al siguiente carpeta...
            var str = Path.Combine(Server, _newNameImg);

            //copiamos la imagen seleccionada...
            ImgPerfil.InputStream.CopyToAsync(new FileStream(str, FileMode.Create));

            //string imgName = ImgPerfil.FileName;
            //Obtenemos un string y lo convertimos a byte... usando la imagen de perfil...
            byte[] _byteString = Encoding.ASCII.GetBytes(_newNameImg);

            return _byteString;
        }

        private static byte[] igualByteToByte(byte args)
        {
            byte[] _result = new byte[args];
            return _result;
        }
    }
}