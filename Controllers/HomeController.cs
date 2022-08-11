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
            string _SessionUsuario = Session["usuario"].ToString();
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
            //PASAMOS LA SESÍÓN EL QUIEN ESTA LOGEADO, PARA OBTENER LA LISTA DE AMIGOS...
            // Y ASI VALIDAR...
            bool _act = ObtenerSolicitud(_SessionUsuario, idE); // _act = activo --->idE
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
            //Vemos las Publicaciones del Usuario... Segun sea el EMAIL que le pasemos...
            ViewBag.Publicaciones = ListaPublicaiones(idE);
            ViewBag.usu = _usu;
            //pasamos si es fue aceptado o no la solicitud...
            ViewBag.Amigos = _serAmigo;
            // para el boton de enviar solicitud.. si es true o false...
            ViewBag.nBTN = _btn;
            //pasamos el id del usuario del amigo...
            ViewBag.IdUsuAmigo = _idUsuAmigo;
            return View(model);
        }

        //Metodo para Obtener si Enviamos Solicitud O no...
        private bool ObtenerSolicitud(string idUsuSession,string idE)
        {
            //vamos a guardar el numero, del activo, eso si existe solicitudes para el usuario...
            int _verificar;
            int _idSesionAmig;
            bool _esVerdFal;
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("sp_Amigos", cn);
                cmd.Parameters.AddWithValue("email", idUsuSession);
                cmd.Parameters.AddWithValue("emailAmigo", idE);
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
                            Session["IdUsuAmigo"] = _idSesionAmig;
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
                cmd.Parameters.AddWithValue("idEmail", idE);
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
                        oLista.Titulo = dr["Titulo"].ToString();
                        //obtener el total de los post...
                        int _totals = TMGusta(Convert.ToInt32(dr["IdPost"]));
                        oLista.TotalPost = _totals;
                        _lista.Add(oLista);
                    }
                }
                cn.Close();

                return _lista;
            }
            //return oLista;
        }

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

        public ActionResult ListaAmigosDeAmigoVista()
        {
            ViewBag.listaAmigosDeAmigo = ListaAmigosDeAmigo().ToList();
            ViewBag.IdUsu = Session["usuario"].ToString();
            return View();
            //return View(Listar());
        }

        public List<Amigos> ListaAmigosDeAmigo()
        {
            List<Amigos> _lista = new List<Amigos>();

            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("sp_ListaAmigos", cn);
                cmd.Parameters.AddWithValue("email", ViewBag.nBTN);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        byte[] _byteImg = (byte[])dr["ImagenPerfil"];
                        var _byteString = System.Text.Encoding.Default.GetString(_byteImg);

                        Amigos oLista = new Amigos();

                        //agregamos al objeto...
                        oLista.Id = (int)dr["Id"];
                        //oLista.IdUsu = (int)dr["IdUsu"];
                        oLista.IdUsuAmigo = (int)dr["IdUsuAmigo"];
                        oLista.Activo = (int)dr["Activo"];
                        oLista.Nombre = dr["Nombre"].ToString();
                        oLista.UserName = dr["UserName"].ToString();
                        oLista.imagenPerf = _byteString;
                        oLista.Email = dr["Email"].ToString();

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

        public ActionResult CerrarSesion()
        {
            Session["usuario"] = null;
            return RedirectToAction("Login", "Acceso");
        }

    }
}