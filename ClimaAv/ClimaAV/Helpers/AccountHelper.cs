using ClimaAV.Database;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ClimaAV.Helpers
{
    public class AccountHelper
    {
        private static AmericaVirtualEntities db = new AmericaVirtualEntities();

        private static IList<Rule> AllRulesCache = null;

        public static void GetCurrentUserRules()
        {
            User user = new User();

            //obtengo el usuario logueado
            long userid = GetCurrentUser().UserID;

            using (AmericaVirtualEntities conexion = new AmericaVirtualEntities())
            {
                user = conexion.User.Include("Rol").Include("Rol.Rule").Where(x => x.UserID == userid).FirstOrDefault();
            }

            var roles = user.Rol;

            var rules = new List<Rule>();
            foreach (Rol rol in roles)
            {
                rules.AddRange(rol.Rule);
            }
            if (rules.Count > 0)
                SetCurrentRules(rules.ToList());
            else
                SetCurrentRules(new List<Rule>());

            AccountHelper.AllRulesCache = db.Rule.ToList();
        }

        public static bool IsAuthorized(string definicion)
        {

            if (GetCurrentRules() == null || GetCurrentRules().FirstOrDefault(r => r.Definicion == definicion) == null) //unauthorized
            {
                return false;
            }
            else
                return true;
        }

        //public static bool EsExterna()
        //{
        //    var area = GetArea();

        //    if (area != null)
        //    {
        //        Guid IdArea = Guid.Parse(area);
        //        Area ar = db.Area.Where(a => a.ORID == IdArea).FirstOrDefault();

        //        if (ar.EsExterna == true)
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    return false;
        //}
        
        public static void RefreshApplicationUser(User user)
        {
            User appUser = new User
            {
                UserID = user.UserID,
                Apellido = user.Apellido,
                Email = user.Email,
                Nombre = user.Nombre
            };
            SetCurrentUser(appUser);
        }

        public static User ExisteUser(string user)
        {
            User usuario = db.User.Where(u => u.Usuario == user).FirstOrDefault();
            return usuario;
        }

        public static User GetCurrentUser()
        {
            return HttpContext.Current.Session["ApplicationUser"] as User;
        }

        public static void SetCurrentUser(User user)
        {
            if (user == null)
                HttpContext.Current.Session.RemoveAll();
            else
                HttpContext.Current.Session["ApplicationUser"] = user;
        }

        public static string GetPuesto()
        {
            return HttpContext.Current.Session["PuestoId"] as string;
        }

        public static void SetPuesto(string puestoId)
        {
            if (puestoId == null)
                HttpContext.Current.Session.RemoveAll();
            else
                HttpContext.Current.Session["PuestoId"] = puestoId;
        }

        public static string GetArea()
        {
            return HttpContext.Current.Session["AreaId"] as string;
        }

        public static void SetArea(string areaId)
        {
            if (areaId == null)
                HttpContext.Current.Session.RemoveAll();
            else
                HttpContext.Current.Session["AreaId"] = areaId;
        }

        public static string GetEspecialidad()
        {
            return HttpContext.Current.Session["EspecialidadId"] as string;
        }

        public static void SetEspecialidad(string especialidadId)
        {
            if (especialidadId == null)
                HttpContext.Current.Session.RemoveAll();
            else
                HttpContext.Current.Session["EspecialidadId"] = especialidadId;
        }

        //public static void SetEnteArea(string area)
        //{
        //    var IdArea = Guid.Parse(area);
        //    Area ar = db.Area.Where(a => a.ORID == IdArea).FirstOrDefault();

        //    string eee = ar.IdEnte.ToString();

        //    HttpContext.Current.Session["EnteId"] = eee;
        //}

        public static void SetEnte(string ente)
        {
            HttpContext.Current.Session["EnteId"] = ente;
        }

        public static string GetEnte()
        {
            return HttpContext.Current.Session["EnteId"] as string;
        }

        public static IEnumerable<Rule> GetCurrentRules()
        {
            return HttpContext.Current.Session["Rules"] as IEnumerable<Rule>;
        }

        public static void SetCurrentRules(List<Rule> rules)
        {
            HttpContext.Current.Session["Rules"] = rules;
        }

        public static string GetCurrentUserName()
        {
            var user = GetCurrentUser();
            if (user != null)
                return user.Usuario;
            else
                return "";
        }

        public static string getFoto()
        {
            var user = GetCurrentUser();
            if (user != null)
                return user.Foto;
            else
                return "~/Images/users/user.jpg";
        }

        public static string GetCurrentNyA()
        {
            var user = GetCurrentUser();
            if (user != null)
                return user.Apellido + ' ' + user.Nombre;
            else
                return "";
        }

        //public static string GetDatosEntidad()
        //{
        //    var ente = GetEnte();
        //    var area = GetArea();

        //    if (area != null)
        //    {
        //        Guid IdEnte = Guid.Parse(ente);
        //        Guid IdArea = Guid.Parse(area);

        //        Ente en = db.Ente.Where(e => e.ORID == IdEnte).FirstOrDefault();
        //        Area ar = db.Area.Where(a => a.ORID == IdArea).FirstOrDefault();

        //        return en.Descripcion + " / " + ar.Descripcion;
        //    }
        //    else
        //    {
        //        return "";
        //    }
        //}

        //public static string GetDatosLlamador()
        //{
        //    var area = GetArea();
        //    var puesto = GetPuesto();

        //    if (area != null)
        //    {
        //        Guid IdArea = Guid.Parse(area);
        //        Area ar = db.Area.Where(a => a.ORID == IdArea).FirstOrDefault();

        //        Guid IdPuesto = Guid.Parse(puesto);
        //        Puesto pu = db.Puesto.Where(p => p.ORID == IdPuesto).FirstOrDefault();

        //        return "ÁREA: " + ar.Descripcion + " | PUESTO: " + pu.Descripcion;
        //    }
        //    else
        //    {
        //        return "";
        //    }
        //}

        public static Ente getEntidad()
        {
            Ente ente = new Ente();

            using (AmericaVirtualEntities db = new AmericaVirtualEntities())
            {
                Guid IdEnte = Guid.Parse(GetEnte());
                ente = db.Ente.Where(e => e.ORID == IdEnte).FirstOrDefault();
            }

            return ente;
        }

        public static void SetLlamador(String ente)
        {
            if (ente == null)
                HttpContext.Current.Session.RemoveAll();
            else
                HttpContext.Current.Session["LlamadorEnte"] = ente;
        }

        public static string GetLlamadorEnte()
        {
            return HttpContext.Current.Session["LlamadorEnte"] as string;
        }


    }
}