using InstaMazz2._0.Models;
using InstaMazz2._0.Permisos;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Helpers;
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
            //enviamos la charla del usuario con sus amigos...
            //--- a la vista...
            ViewBag.Chats = GetChats().ToList();
            //lista de Amigos...
            ViewBag.listaSolicitudes = ListaAmigos(sessionUsuario()).ToList();
            return View();
        }

        private List<MChats> GetChats()
        {
            List<MChats> _mChats = new List<MChats>();

            //Obtenemos la session del usuario...
            string _sessionEmail = sessionUsuario();

            using (SqlConnection cn = new SqlConnection(cadena)) 
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SP_Get_ChatsAmig", cn);
                cmd.Parameters.AddWithValue("emailUsu", _sessionEmail);
                cmd.CommandType = CommandType.StoredProcedure;

                //Obtenemos todo la combersaciones del usuario con sus amigos...
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        MChats oChat = new MChats();

                        //Guardamos en la lista del Modelo...
                        oChat.ID = (int)dr["ID"];
                        oChat.Mensaje = (string)dr["Mensaje"];
                        oChat.Fecha = (string)dr["Fecha"];

                        //Ahora lo Guardamos en la Lista del Chat...
                        _mChats.Add(oChat);
                    }
                }

                //Retornamos la lista del chat del usuario...
                return _mChats;
            }
        }

        private bool Guardar(string emailAmigo, string mensaje)
        {
            string _sessionEmail = sessionUsuario();
            //Guardamos la conversación del usuario con sus amigos...
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                try
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("SP_Insert_ChatsAmig", cn);
                    cmd.Parameters.AddWithValue("emailUsu", _sessionEmail);
                    cmd.Parameters.AddWithValue("emailAmig", emailAmigo);
                    cmd.Parameters.AddWithValue("mensaje", mensaje);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();

                    //reactivamos el metodo de ver el chat...
                    GetChats();

                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        private List<Amigos> ListaAmigos(string IdEmail)
        {
            List<Amigos> _lista = new List<Amigos>();
            // esta variable es para pasar el nombre de la imagen obtenida desde la bd...
            string _byteString;

            //esta variable cirbe para determinar, si el usuario tiene una foto de perfil o no...
            bool _ceroImg;

            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("sp_ListaAmigos", cn);
                cmd.Parameters.AddWithValue("email", IdEmail); //Session["usuario"]
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        byte[] _byteImg = (byte[])dr["ImagenPerfil"];
                        if (BitConverter.ToInt32(_byteImg, 0) > 0)
                        {
                            _byteString = System.Text.Encoding.Default.GetString(_byteImg);
                            _ceroImg = true;
                        }
                        else
                        {
                            _byteString = "avatar.jpg";
                            _ceroImg = false;
                        }
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
                        oLista.ceroImg = _ceroImg;

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

        private string sessionUsuario()
        {
            return (string)Session["usuario"];
        }

    }
}