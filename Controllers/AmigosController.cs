﻿using System;
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
        public ActionResult AgregarAmigo()
        {
            return View();
        }

        public ActionResult SolicitudesVista()
        {
            ViewBag.listaSolicitudes = Solicitudes().ToList();
            return View();
            //return View(Listar());
        }

        public List<Amigos> Solicitudes()
        {
            List<Amigos> _lista = new List<Amigos>();

            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("sp_SolicitudesPendientes", cn);
                /* cmd.Parameters.AddWithValue("idEmail", Session["usuario"]);*/
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
                /* cmd.Parameters.AddWithValue("idEmail", Session["usuario"]);*/
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
    }
}