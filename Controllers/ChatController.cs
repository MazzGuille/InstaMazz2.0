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
    public class ChatController : Controller
    {
        string cadena = ConfigurationManager.ConnectionStrings["InstaMaczzDB"].ConnectionString;

        // GET: Chat
        public ActionResult Chat()
        {
            return View();
        }

        private static List<MChats> GetChats()
        {
            List<MChats> mChats = new List<MChats>();
            string _sessionEmail = (string)Session["usuario"];

            using (SqlConnection cn = new SqlConnection(cadena)) { }
        }

    }
}