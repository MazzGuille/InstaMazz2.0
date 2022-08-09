using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InstaMazz2._0.Models;
using InstaMazz2._0.Permisos;

namespace InstaMazz2._0.Controllers
{
    [ValidarSesion]
    public class AmigosController : Controller
    {
        string cadena = ConfigurationManager.ConnectionStrings["InstaMaczzDB"].ConnectionString;
        // GET: Amigos
        public ActionResult AgregarAmigo(int nSolt, string idemail) //Amigos oAmigo
        {
            //si es 0 el btn va hacer de color azul..
            //sino el btn va hacer de color rojo...
            Session["activ"] = nSolt;
            //enviamos los datos al metodo de agregar amigos...
            AgregarAmg(Session["usuario"].ToString(), idemail, nSolt);
            //retornamos la vista del usuario...
            return RedirectToAction("Index", "Home", new { idE = idemail });
        }

        //CREAR EL MEDO DE AGREGAR...
        private bool AgregarAmg(string EmailMio, string EmailAmigo, int Bits)
        {
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                try
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("SP_Insert_Amigos", cn);
                    cmd.Parameters.AddWithValue("Email_Mio", EmailMio);
                    cmd.Parameters.AddWithValue("Email_Amigo", EmailAmigo);
                    cmd.Parameters.AddWithValue("Solict", Bits);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();

                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }

        public ActionResult SolicitudesVista()
        {
            ViewBag.listaSolicitudes = Solicitudes().ToList();
            return View();
        }

        public List<Amigos> Solicitudes()
        {
            List<Amigos> _lista = new List<Amigos>();
            
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("sp_SolicitudesPendientes", cn);
                cmd.Parameters.AddWithValue("email", Session["usuario"]);
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
                        oLista.IdUsu = (int)dr["IdUsu"];
                        oLista.IdUsuAmigo = (int)dr["IdUsuAmigo"];
                        oLista.Activo = (int)dr["Activo"];
                        oLista.Nombre = dr["Nombre"].ToString();
                        oLista.UserName = dr["UserName"].ToString();
                        oLista.imagenPerf = _byteString;

                        

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

        public ActionResult ListaAmigosVista()
        {
            ViewBag.listaSolicitudes = ListaAmigos().ToList();
            return View();
            //return View(Listar());
        }

        public List<Amigos> ListaAmigos()
        {
            List<Amigos> _lista = new List<Amigos>();

            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("sp_ListaAmigos", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        Amigos oLista = new Amigos();

                        //agregamos al objeto...
                        oLista.Id = (int)dr["Id"];
                        oLista.IdUsu = (int)dr["IdUsu"];
                        oLista.IdUsuAmigo = (int)dr["IdUsuAmigo"];
                        oLista.Activo = (int)dr["Activo"];

                        //oLista.UserName = dr["UserName"].ToString();

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

        public ActionResult AceptarSolicitud(int idAcp, int AcpRech)
        {
            UpdateSolicitud(idAcp, AcpRech);
            return RedirectToAction("ListaAmigosVista","Amigos");
        }

        private bool UpdateSolicitud(int id, int Bits)
        {
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                try
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("SP_Update_AceptarSolicitud", cn);
                    cmd.Parameters.AddWithValue("idAcptar", id);
                    cmd.Parameters.AddWithValue("AceptarRechazo", Bits);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();

                    return true;
                }
                catch
                {
                    //cerramos la Conexión...
                    cn.Close();
                    return false;
                }
            }
        }

    }
}