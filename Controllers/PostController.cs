﻿using InstaMazz2._0.Models;
using InstaMazz2._0.Permisos;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InstaMazz2._0.Controllers
{
    [ValidarSesion]
    public class PostController : Controller
    {

        string cadena = ConfigurationManager.ConnectionStrings["InstaMaczzDB"].ConnectionString;

        public ActionResult ComentarView(int? idPost)
        {
            //Psamos el Id del post..
            ViewBag.Feed = Feed(idPost).ToList();
            // ViewBag.ListaComentarios = ListaComentarios().ToList();
            return View();
        }

        public ActionResult CrearPost()
        {
            var IdUsuario = (int)Session["IdUsuario"];
            ViewBag.IdUsuario = IdUsuario;
            var UserName = (string)Session["UsName"];
            ViewBag.UserName = UserName;
            return View();
        }

        [HttpPost]
        public ActionResult CrearPost(PublicacionesModel oPublicacion)
        {
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("sp_CrearPost", cn);
                cmd.Parameters.AddWithValue("IdUsuario", oPublicacion.IdUsuario);
                cmd.Parameters.AddWithValue("UrlImg", oPublicacion.UrlImg);
                cmd.Parameters.AddWithValue("Descripcion", oPublicacion.Descripcion);
                cmd.Parameters.AddWithValue("Titulo", oPublicacion.Titulo);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index", "Home", new { idE = Session["usuario"] });
        }

        public ActionResult EliminarPost(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.IdPost = id;
            return View();

        }

        [HttpPost]
        public ActionResult Delete(int? IdPost)
        {
            string _sessionEmail = sessionUsuario();
            int _idPost = Convert.ToInt32(IdPost);
            DeletePost(_idPost, _sessionEmail);
            return RedirectToAction("Index", "Home", new { idE = _sessionEmail });
        }

        private bool DeletePost(int IdPost, string IdUsu)
        {
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                try
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("sp_EliminarPost", cn);
                    cmd.Parameters.AddWithValue("IdPost", IdPost);
                    cmd.Parameters.AddWithValue("Email", IdUsu);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public ActionResult ListarVista()
        {
            ViewBag.listaPost = Listar().ToList();
            return View();
        }

        [HttpPost]
        public List<PublicacionesModel> Listar()
        {
            List<PublicacionesModel> _lista = new List<PublicacionesModel>();

            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("sp_Listar", cn);
                cmd.Parameters.AddWithValue("idEmail", sessionUsuario());
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        PublicacionesModel oLista = new PublicacionesModel();

                        //agregamos al objeto...
                        oLista.IdPost = Convert.ToInt32(dr["IdPost"]);
                        oLista.IdUsuario = Convert.ToInt32(dr["IdUsuario"]);
                        oLista.UrlImg = dr["UrlImg"].ToString();
                        oLista.Descripcion = dr["Descripcion"].ToString();

                        //agregamos a la lista el objeto de list...
                        _lista.Add(oLista);
                    }
                }
                //cerramos la Conexión...
                cn.Close();

                //retornamos la lista...
                return _lista;
            }
        }

        public ActionResult FeedView()
        {
            ViewBag.Feed = Feed(0).ToList();
            ViewBag.Ids = sessionUsuario();
            return View();
        }

        [HttpPost]
        public List<PublicacionesModel> Feed(int? idPosts)
        {
            List<PublicacionesModel> _lista = new List<PublicacionesModel>();
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("sp_Feed", cn);
                cmd.Parameters.AddWithValue("Email", sessionUsuario());
                //PASAMOS EL ID DEL POST, PERO PRIMERO VALIDAMOS EL ID...
                cmd.Parameters.AddWithValue("IdPost", idPosts);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        PublicacionesModel oLista = new PublicacionesModel();

                        oLista.IdPost = (int)dr["IdPost"];
                        oLista.IdUsuario = (int)dr["IdUsuario"];
                        oLista.UrlImg = dr["UrlImg"].ToString();
                        oLista.Descripcion = dr["Descripcion"].ToString();
                        oLista.Mgusta = (bool)dr["Mgusta"];
                        oLista.UserName = dr["UserName"].ToString();
                        oLista.Email = dr["Email"].ToString();

                        //obtener el total de los post...
                        int _totals = TMGusta(Convert.ToInt32(dr["IdPost"]));
                        oLista.TotalPost = _totals;

                        _lista.Add(oLista);
                    }
                }
                cn.Close();

                return _lista;
            }
        }

        //btn Me-Gusta
        public ActionResult MGusta(string IdUsu, int IdPost)
        {
            //aca va el metodo privado de "guardar el megusta".. 
            MGustaGuardar(IdUsu, IdPost);
            return RedirectToAction("FeedView", "Post");
        }

        private bool MGustaGuardar(string email, int idpost)
        {
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                try
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("SP_InsertDelete_MGusta", cn);
                    cmd.Parameters.AddWithValue("Email", email);
                    cmd.Parameters.AddWithValue("IdPost", idpost);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        //total de M-Gusta del Post
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

        private List<MPostComent> ComentPost(int idPost)
        {
            List<MPostComent> _ListComent = new List<MPostComent>();

            using (SqlConnection cn = new SqlConnection(cadena)) 
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SP_Get_TotalxPost", cn);
                cmd.Parameters.AddWithValue("IdPost", idPost);
                cmd.CommandType = CommandType.StoredProcedure;
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        MPostComent oComent = new MPostComent();

                        oComent.IdPost = (int)dr["IdPost"];

                    }
                }

                return _ListComent;
            }
        }

        private string sessionUsuario()
        {
            return (string)Session["usuario"];
        }

    }
}