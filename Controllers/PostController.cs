using InstaMazz2._0.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InstaMazz2._0.Controllers
{
    public class PostController : Controller
    {

        static string cadena = " Data Source=(local); Initial Catalog = InstaMazz; Integrated Security = true;";



        public ActionResult CrearPost()
        {
            var IdUsuario = (int)Session["IdUsuario"];
            ViewBag.IdUsuario = IdUsuario;
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
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();



                var IdPost = oPublicacion.IdPost;

                Session["IdPost"] = IdPost;

            }

            return RedirectToAction("Index", "Home");
        }

        
        public ActionResult EliminarPost(int? id)
        {
            if(id==null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.IdPost = id;
            return View();
        }
        [HttpPost/*("[Controller]/{IdPost}")*/]
        public ActionResult EliminarPost(PublicacionesModel oPublicacion)
        {
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("sp_EliminarPost", cn);
                cmd.Parameters.AddWithValue("IdPost", oPublicacion.IdPost);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("ListarVista", "Post");
        }


        public ActionResult ListarVista()
        {
            return View(Listar());
        }

        [HttpPost]
        public List<PublicacionesModel> Listar()
        {
            var oLista = new List<PublicacionesModel>();

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
    }
}