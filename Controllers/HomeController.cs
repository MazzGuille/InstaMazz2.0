using InstaMazz2._0.Permisos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InstaMazz2._0.Models;
using System.Data.SqlClient;
using System.Data;

namespace InstaMazz2._0.Controllers
{
    [ValidarSesion]
    public class HomeController : Controller
    {
        //static string cadena = " Data Source=(local); Initial Catalog = InstaMazz; Integrated Security = true;";
        public ActionResult Index()
        {

            var mail = Session["usuario"];
            ViewBag.email = mail;
            ViewBag.name = "Nombre";
            ViewBag.Img = "http://www.w3bai.com/w3css/img_avatar3.png";

            /* using (SqlConnection cn = new SqlConnection(cadena))
             {
                 var cmd = new SqlCommand("sp_Get_DataUser", cn);
                 cmd.Parameters.AddWithValue("email", Session["usuario"]);
                 cmd.CommandType = CommandType.Text;
                 cn.Open();

                 using (SqlDataReader dr = cmd.ExecuteReader())
                 {
                     while (dr.Read())
                     {
                         ViewBag.name = dr["Nombre"].ToString();
                         ViewBag.email = dr["email"].ToString();
                         ViewBag.Img = "http://www.w3bai.com/w3css/img_avatar3.png";
                     }
                 }
             }*/

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult CerrarSesion()
        {
            Session["usuario"] = null;
            return RedirectToAction("Login", "Acceso");
        }


    }
}