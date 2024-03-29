﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls.WebParts;
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
            AgregarAmg(sessionUsuario(), idemail, nSolt);
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
                catch
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
                        
            //Obtenemos la Session del Usuario...
            string _sessionEmail = sessionUsuario();

            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("sp_SolicitudesPendientes", cn);
                cmd.Parameters.AddWithValue("email", _sessionEmail);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        byte[] _byteImg = (byte[])dr["ImagenPerfil"];

                        bool _ceroImg = CeroImagen(_byteImg);

                        Amigos oLista = new Amigos();

                        //agregamos al objeto...
                        oLista.Id = (int)dr["Id"];
                        oLista.IdUsu = (int)dr["IdUsu"];
                        oLista.IdUsuAmigo = (int)dr["IdUsuAmigo"];
                        oLista.Activo = (int)dr["Activo"];
                        oLista.Nombre = dr["Nombre"].ToString();
                        oLista.UserName = dr["UserName"].ToString();
                        oLista.imagenPerf = PasarIMG(_ceroImg, _byteImg);
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

        public ActionResult ListaAmigosVista(string IdEmail)
        {
            //HABILITA EL BOTON, DE ELIMINAR SI ES, LA SESIÓN ACTUAL,
            // EL QUE LE PASO A LA LISTA DE AMIGOS...
            bool _PermisosPerfil;
            //session de uno mismo...
            string _sessionEmail = sessionUsuario();
            ViewBag.IdUsu = _sessionEmail;
            if(_sessionEmail == IdEmail)
            {
                _PermisosPerfil = true;
            }
            else
            {
                _PermisosPerfil = false;
            }

            //activar el btn de eliminar... o de enviar solicitud...
            ViewBag.Activo = _PermisosPerfil;
            ViewBag.listaSolicitudes = ListaAmigos(IdEmail).ToList();
            return View();
        }

        private List<Amigos> ListaAmigos(string IdEmail)
        {
            List<Amigos> _lista = new List<Amigos>();
           
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
                        bool _ceroImg = CeroImagen(_byteImg);
                        
                        Amigos oLista = new Amigos();

                        //agregamos al objeto...
                        oLista.Id = (int)dr["Id"];
                        //oLista.IdUsu = (int)dr["IdUsu"];
                        oLista.IdUsuAmigo = (int)dr["IdUsuAmigo"];
                        oLista.Activo = (int)dr["Activo"];
                        oLista.Nombre = dr["Nombre"].ToString();
                        oLista.UserName = dr["UserName"].ToString();
                        oLista.imagenPerf = PasarIMG(_ceroImg, _byteImg);
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

        public ActionResult AceptarSolicitud(int idAcp, int AcpRech)
        {
            string _Email = sessionUsuario();
            UpdateSolicitud(idAcp, AcpRech);
            return RedirectToAction("ListaAmigosVista", "Amigos", new { IdEmail = _Email });
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