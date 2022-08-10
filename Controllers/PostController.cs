using InstaMazz2._0.Models;
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
                //cmd.Parameters.AddWithValue("UserName", Session["UsName"]);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index", "Home", new { idE = Session["usuario"] });
        }


        //public ActionResult EliminarPost(int IdPost)
        //{
        //    //ViewBag.IdInput = id;           
        //    return View(IdPost);


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
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("sp_EliminarPost", cn);
                cmd.Parameters.AddWithValue("IdPost", IdPost);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index", "Home", new { idE = Session["usuario"] });
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
                cmd.Parameters.AddWithValue("idEmail", Session["usuario"]);
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
                        //oLista.UserName = dr["UserName"].ToString();

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
            ViewBag.Feed = Feed().ToList();
            ViewBag.Ids = Session["usuario"];
            return View();
        }

        [HttpPost]
        public List<PublicacionesModel> Feed()
        {
            List<PublicacionesModel> _lista = new List<PublicacionesModel>();

            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("sp_Feed", cn);
                cmd.Parameters.AddWithValue("Email", Session["usuario"]);
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
                        oLista.UserName = dr["UserName"].ToString();

                        _lista.Add(oLista);
                    }
                }
                cn.Close();

                return _lista;
            }
        }

        //btn Me-Gusta
        public ActionResult MGusta(string email, int IdPost)
        {
            //aca va el metodo privado de "guardar el megusta".. 
            return RedirectToAction("FeedView", "Post");
        }

        private bool MGustaBTN(string email, int idpost)
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
    }
}