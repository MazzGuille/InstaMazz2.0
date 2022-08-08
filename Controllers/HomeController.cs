using InstaMazz2._0.Permisos;
using InstaMazz2._0.Controllers;
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

        UsuarioModel model = new UsuarioModel();
        public ActionResult Index(string idE)
        {
            bool usu;
            bool _btn;
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                var cmd = new SqlCommand("sp_Get_DataUser", cn);
                cmd.Parameters.AddWithValue("idEmail", idE);//Session["usuario"]
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
                        model.imagenPerf = _byteString;
                        model.BioUsuario = dr["BioUsuario"].ToString();
                    }
                }
            }
            string _exiteUsu = Session["usuario"].ToString();
            if(_exiteUsu == idE)
            {
                usu = true;
            }
            else
            {
                usu = false;
            }

            //verificamos si tiene solicitud... si es tru o false...
            bool _act = ObtenerSolicitud(idE);
            if (_act)
            {
                _btn = true;
                var r = Session["IdUsuAmigo"]; //Session["IdUsuAmigo"]
            }
            else
            {
                if (Convert.ToBoolean(Session["activ"]))
                {
                    _btn = true;
                }
                else
                {
                    _btn = false;
                }
            }
            ViewBag.Publicaciones = ListaPublicaiones(idE);
            ViewBag.usu = usu;
            ViewBag.nBTN = _btn; // para el boton de enviar solicitud.. si es true o false...
            ViewBag.IdUsuAmigo = Session["IdUsuAmigo"];
            return View(model);
        }

        //Metodo para Obtener si Enviamos Solicitud O no...
        private bool ObtenerSolicitud(string idE)
        {
            //vamos a guardar el numero, del activo, eso si existe solicitudes para el usuario...
            int _verificar;
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                try
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("sp_Amigos", cn);
                    cmd.Parameters.AddWithValue("email", idE);
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ViewBag.IdUsu = (int)dr["IdUsu"];
                            Session["IdUsuAmigo"] = (int)dr["IdUsuAmigo"];
                            _verificar = (int)dr["Activo"];

                            ////verificamos si trae un cero... si existe la solicitud...
                            //_verificar = (int)dr["Activo"];
                            if (_verificar == 0)
                            {
                                Session["activ"] = 1;
                            }
                        }
                    }

                    return true;
                }
                catch
                {
                    return false;
                }

            }
        }

        //REVISAR ACA ---------------------------------------------------------------------------
        public ActionResult PerfilUsu()
        {
            var _id = Session["usuario"];
            return RedirectToAction("Index", "Home", new { idE = _id.ToString() });
        }

        public ActionResult MePerfil(string email)
        {
            //obtenemos el email / correo del usuario al que vamos a mandar la invitacion...
            // o ver sus publicaciones...
            return RedirectToAction("Index", "Home", new { idE = email });
        }

        public List<PublicacionesModel> ListaPublicaiones(string idE)
        {
            var oLista = new List<PublicacionesModel>();

            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("sp_Obtener", cn);
                cmd.Parameters.AddWithValue("idEmail", idE);//Session["usuario"]

                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        oLista.Add(new PublicacionesModel
                        {
                            IdPost = Convert.ToInt32(dr["IdPost"]),
                            IdUsuario = Convert.ToInt32(dr["IdUsuario"]),
                            UrlImg = dr["UrlImg"].ToString(),
                            Descripcion = dr["Descripcion"].ToString()
                        });

                    }
                }
            }
            return oLista;
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

        public ActionResult BuscarView()
        {
            ViewBag.Buscar = Buscar().ToList();
            ViewBag.nBTN = false;
            return View();
        }

        [HttpPost]
        public ActionResult Search(UsuarioModel oUsu)
        {
            //obtengo el dato del input-> del formulario...
            Session["busc"] = oUsu.Nombre;

            //redireccionamos a la vista donde se ve los datos...
            return RedirectToAction("BuscarView", "Home");
        }

        public List<UsuarioModel> Buscar()
        {
            var oLista = new List<UsuarioModel>();
            //obtengo el dato de la barible...
            var usu = Session["busc"];

            //verifico si la variable existe o si es null..
            if (usu == null)
            {
                //si la variable es null se lo coloca un vacio...
                usu = "";
            }

            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SP_Buscar", cn);
                cmd.Parameters.AddWithValue("usu", usu);

                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = cmd.ExecuteReader())
                {

                    while (dr.Read())
                    {
                        byte[] _byteImg = (byte[])dr["ImagenPerfil"];
                        var _byteString = System.Text.Encoding.Default.GetString(_byteImg);

                        oLista.Add(new UsuarioModel
                        {
                            Nombre = dr["Nombre"].ToString(),
                            UserName = dr["UserName"].ToString(),
                            IdUsuario = ((int)dr["IdUsuario"]),
                            Email = dr["Email"].ToString(),
                            imagenPerf = _byteString
                        });
                    }
                }
            }
            return oLista;
        }


        public ActionResult CerrarSesion()
        {
            Session["usuario"] = null;
            return RedirectToAction("Login", "Acceso");
        }


    }
}