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
using Windows.UI.Xaml.Documents;

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
            //Pasamos los comantarios de los usuarios...
            ViewBag.ListaComentarios = ComentPost(idPost).ToList();
            //Pasamos la session del usuario que inicio la session...
            ViewBag.IdSession = sessionUsuario();
            //Pasamos el id del Post...
            ViewBag.IdPost = idPost;
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

        private List<MPostComent> ComentPost(int? idPost)
        {
            List<MPostComent> _ListComent = new List<MPostComent>();

            using (SqlConnection cn = new SqlConnection(cadena)) 
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SP_Get_ComentariosPost", cn);
                cmd.Parameters.AddWithValue("IdPost", idPost);
                cmd.CommandType = CommandType.StoredProcedure;
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        //pasamos la Imagen del Perfil y comvertimos a string...
                        byte[] _byteImg = (byte[])dr["ImagenPerfil"];

                        bool _ceroImg = CeroImagen(_byteImg);

                        MPostComent oComent = new MPostComent();

                        oComent.IdPost = (int)dr["IdPost"];
                        oComent.Coment = (string)dr["Coment"];
                        oComent.Fecha = (string)dr["Fecha"];
                        oComent.Activo = (bool)dr["Activo"];
                        oComent.imagenPerf = PasarIMG(_ceroImg, _byteImg);
                        oComent.ceroImg = _ceroImg;

                        //lo agregamos en la lista de comentario...
                        _ListComent.Add(oComent);
                    }
                }

                return _ListComent;
            }
        }

        [HttpPost]
        public ActionResult SavePost(string email, int id, MPostComent oPostComent)
        {
            //Guardamos los datos...
            GuardarPostComent(id, email, oPostComent.Mensaje);
            //retornamos la pagina al que estamos cometando...
            return RedirectToAction("ComentarView", "Post", new { IdPost = id});
        }

        private bool GuardarPostComent(int idPost, string email, string coment)
        {
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                try
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("SP_Insert_ComentariosPost", cn);
                    cmd.Parameters.AddWithValue("idPost", idPost);
                    cmd.Parameters.AddWithValue("email", email);
                    cmd.Parameters.AddWithValue("coment", coment);
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

        private string sessionUsuario()
        {
            return (string)Session["usuario"];
        }

        private bool CeroImagen(Byte[] IMG)
        {
            //esta variable cirbe para determinar, si el usuario tiene una foto de perfil o no...
            bool _ceroImg;

            if (BitConverter.ToInt32(IMG, 0) > 0)
            {
                _ceroImg = true;
            }
            else
            {
                _ceroImg = false;
            }

            //Retornamos la repuesta...
            return _ceroImg;
        }

        private string PasarIMG(bool v_f, byte[] imagen)
        {
            // esta variable es para pasar el nombre de la imagen obtenida desde la bd...
            string _byteString;
            if (v_f)
            {
                _byteString = System.Text.Encoding.Default.GetString(imagen);
            }
            else
            {
                _byteString = "avatar.jpg";
            }

            //Retornamos la Imagen Correspondiente...
            return _byteString;
        }

    }
}