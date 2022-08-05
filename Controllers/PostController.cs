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
        public ActionResult Delete(int? IdPost) //PublicacionesModel oPublicacion
        {
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("sp_EliminarPost", cn);
                cmd.Parameters.AddWithValue("IdPost", IdPost); //oPublicacion.IdPost
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index", "Home");
        }


        public ActionResult ListarVista()
        {
            ViewBag.listaPost = Listar().ToList();
            return View();
            //return View(Listar());
        }

        [HttpPost]
        public List<PublicacionesModel> Listar()
        {
            //var oLista = new List<PublicacionesModel>();

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

                        //oLista.Add(new PublicacionesModel
                        //{
                        //    IdPost = Convert.ToInt32(dr["IdPost"]),
                        //    IdUsuario = Convert.ToInt32(dr["IdUsuario"]),
                        //    UrlImg = dr["UrlImg"].ToString(),
                        //    Descripcion = dr["Descripcion"].ToString()
                        //});
                    }
                }
                //cerramos la Conexión...
                cn.Close();

                //retornamos la lista...
                return _lista;
            }
            //return oLista;
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
            //var oLista = new List<PublicacionesModel>();
            List<PublicacionesModel> _lista = new List<PublicacionesModel>();

            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("sp_Feed", cn);

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

                        //oLista.Add(new PublicacionesModel
                        //{
                        //    IdPost = Convert.ToInt32(dr["IdPost"]),
                        //    IdUsuario = Convert.ToInt32(dr["IdUsuario"]),
                        //    UrlImg = dr["UrlImg"].ToString(),
                        //    Descripcion = dr["Descripcion"].ToString(),
                        //    UserName = dr["UserName"].ToString()
                        //});
                    }
                }
                cn.Close();

                return _lista;
            }
            //return oLista;
        }
    }
}