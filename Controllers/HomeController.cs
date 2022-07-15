using InstaMazz2._0.Permisos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InstaMazz2._0.Models;


namespace InstaMazz2._0.Controllers
{
    [ValidarSesion]
    public class HomeController : Controller
    {
        UsuarioModel nom = new UsuarioModel();
        //AccesoController nom = new AccesoController();
        public ActionResult Index()
        {
            //ViewData["Nombre"] = nom.Name();
            //ViewData["Mail"] = mail;
            var mail = Session["usuario"];
            ViewBag.email = mail;
            ViewBag.name = "Nombre";
            ViewBag.Img = "http://www.w3bai.com/w3css/img_avatar3.png";


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