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
            //enviamos la charla del usuario con sus amigos...
            //--- a la vista...
            ViewBag.Chats = GetChats().ToList();
            return View();
        }

        private List<MChats> GetChats()
        {
            List<MChats> _mChats = new List<MChats>();
            string _sessionEmail = (string)Session["usuario"];

            using (SqlConnection cn = new SqlConnection(cadena)) 
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("sp_Listar", cn);
                cmd.Parameters.AddWithValue("idEmail", _sessionEmail);
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

        //private bool Guardar(string emailAmigo, string mensaje)
        //{

        //}

    }
}