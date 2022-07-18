using InstaMazz2._0.Permisos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InstaMazz2._0.Models;
using System.Data.SqlClient;
using System.Data;
using System.IO;

namespace InstaMazz2._0.Controllers
{
    [ValidarSesion]
    public class HomeController : Controller
    {
        static string cadena = " Data Source=(local); Initial Catalog = InstaMazz; Integrated Security = true;";
        public ActionResult Index()
        {

            //var mail = Session["usuario"];
            //ViewBag.email = mail;
            //ViewBag.name = "Nombre";
            //ViewBag.Img = "http://www.w3bai.com/w3css/img_avatar3.png";

            using (SqlConnection cn = new SqlConnection(cadena))
            {
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
                        ViewBag.Img = "http://www.w3bai.com/w3css/img_avatar3.png";
                        //ViewBag.Img = dr["ImagenPerfil"];
                    }
                }
            }

            return View();
        }

        public ActionResult CerrarSesion()
        {
            Session["usuario"] = null;
            return RedirectToAction("Login", "Acceso");
        }


    }
}