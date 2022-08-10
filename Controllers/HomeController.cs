﻿using InstaMazz2._0.Permisos;
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
            bool _usu;
            bool _btn;
            bool _serAmigo;
            int _idUsuAmigo;
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

                        model.IdUsuario = (int)dr["IdUsuario"];
                        model.Nombre = dr["Nombre"].ToString();
                        model.Email = dr["Email"].ToString();
                        model.UserName = dr["UserName"].ToString();
                        model.imagenPerf = _byteString;
                        model.BioUsuario = dr["BioUsuario"].ToString();
                    }
                }
            }
            string _exiteUsu = Session["usuario"].ToString();
            if (_exiteUsu == idE)
            {
                _usu = true;
                
            }
            else
            {
                _usu = false;
            }

            //verificamos si tiene solicitud... si es tru o false...
            bool _act = ObtenerSolicitud(idE); // _act = activo
            if (_act)
            {
                //colocamos en true, al boton para que los demas usuario tengan 
                //solo en estado del boton original, pero solo cambiaria el que envio la solicitud...
                _btn = true;
                //pasamos el id del usuario amigo...
                _idUsuAmigo = Convert.ToInt32(Session["IdUsuAmigo"]);
                //verificamos si fue aceptado o no la solicitud...
                if (Convert.ToBoolean(Session["amigxs"]))
                {
                    _serAmigo = true;
                }
                else
                {
                    _serAmigo = false;
                }
                //pasamos si es fue aceptado o no la solicitud...
                //ViewBag.Amigos = _serAmigo;
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
                _idUsuAmigo = 0;
                _serAmigo = false;
            }
            ViewBag.Publicaciones = ListaPublicaiones(idE);
            ViewBag.usu = _usu;
            // para el boton de enviar solicitud.. si es true o false...
            ViewBag.nBTN = _btn;
            //pasamos si es fue aceptado o no la solicitud...
            ViewBag.Amigos = _serAmigo;
            //pasamos el id del usuario del amigo...
            ViewBag.IdUsuAmigo = _idUsuAmigo;
            return View(model);
        }

        //Metodo para Obtener si Enviamos Solicitud O no...
        private bool ObtenerSolicitud(string idE)
        {
            //vamos a guardar el numero, del activo, eso si existe solicitudes para el usuario...
            int _verificar;
            int _idSesionAmig;
            bool _esVerdFal;
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("sp_Amigos", cn);
                cmd.Parameters.AddWithValue("email", idE);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = cmd.ExecuteReader())
                {
                    //verifica si existe el ususrio en la tabla amigos...
                    if (dr.Read())
                    {
                        ViewBag.IdUsu = (int)dr["IdUsu"];
                        _idSesionAmig = (int)dr["IdUsuAmigo"];
                        _verificar = (int)dr["Activo"];

                        ////verificamos si trae un cero... si existe la solicitud...
                        //_verificar = (int)dr["Activo"];
                        if (_verificar == 0)
                        {
                            Session["activ"] = 1;
                            Session["IdUsuAmigo"] = _idSesionAmig;
                            Session["amigxs"] = 0;
                        }
                        else
                        {
                            Session["amigxs"] = 1;
                        }

                        _esVerdFal = true;
                    }
                    else
                    {
                        _esVerdFal = false;
                    }
                }

                return _esVerdFal;

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
            //var oLista = new List<PublicacionesModel>();
            List<PublicacionesModel> _lista = new List<PublicacionesModel>();

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
                        PublicacionesModel oLista = new PublicacionesModel();

                        oLista.IdPost = Convert.ToInt32(dr["IdPost"]);
                        oLista.IdUsuario = Convert.ToInt32(dr["IdUsuario"]);
                        oLista.UrlImg = dr["UrlImg"].ToString();
                        oLista.Descripcion = dr["Descripcion"].ToString();
                        //obtener el total de los post...
                        int _totals = TMGusta(Convert.ToInt32(dr["IdPost"]));
                        oLista.TotalPost = _totals;
                        _lista.Add(oLista);

                        //oLista.Add(new PublicacionesModel
                        //{
                        //    IdPost = Convert.ToInt32(dr["IdPost"]),
                        //    IdUsuario = Convert.ToInt32(dr["IdUsuario"]),
                        //    UrlImg = dr["UrlImg"].ToString(),
                        //    Descripcion = dr["Descripcion"].ToString()
                        //});

                    }
                }
                cn.Close();

                return _lista;
            }
            //return oLista;
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
            ViewBag.IdS = Session["usuario"];
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

        private int TMGusta(int idPost)
        {
            int _totalEs;
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SP_Get_TotalxPost", cn);
                cmd.Parameters.AddWithValue("IdPost", idPost);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        _totalEs = Convert.ToInt32(dr["TOTAL"]);
                    }
                    else
                    {
                        _totalEs = 0;
                    }
                }
                return _totalEs;
            }
        }

        public ActionResult CerrarSesion()
        {
            Session["usuario"] = null;
            return RedirectToAction("Login", "Acceso");
        }

    }
}