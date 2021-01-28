using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ClimaAV.Database;
using ClimaAV.Models;
using ClimaAV.Helpers;
using AutoMapper;
using ClimaAV.Filters;
using System.Data.Entity;
using System.Collections;

//jgjhgjhgjhg

namespace ClimaAV.Controllers
{
    public class UserController : BaseABMController<User, UserIndex, UserModel, long>
    {
        public override ActionResult Index()
        {
            var user = String.Empty;
            var activo = true;

            List<KeyValuePair<string, object>> parametros = new List<KeyValuePair<string, object>>();
            parametros.Add(new KeyValuePair<string, object>("@User", user));
            parametros.Add(new KeyValuePair<string, object>("@Activo", activo));

            var lista = GetList("IndexUser", parametros.ToArray());
            if (activo) { ViewBag.Estado = true; } else { ViewBag.Estado = false; }

            return View(lista);
        }

        [AuthorizeRule(true)]
        public ActionResult GetTabla(string user, bool activo)
        {
            //if (String.IsNullOrEmpty(ente))
            //{
            //    List<UserIndex> listado = new List<UserIndex>();
            //    return PartialView("_Tabla", listado);
            //}

            List<KeyValuePair<string, object>> parametros = new List<KeyValuePair<string, object>>();
            parametros.Add(new KeyValuePair<string, object>("@User", user));
            parametros.Add(new KeyValuePair<string, object>("@Activo", activo));

            var lista = GetList("IndexUser", parametros.ToArray());
            if (activo) { ViewBag.Estado = true; } else { ViewBag.Estado = false; }

            return PartialView("_Tabla", lista);
        }

        [HttpPost]
        public override ActionResult Edit(UserModel model)
        {
            try
            {
                User oldEntity = Read(model.UserID);
                User entity = ModelToEntity(model);
                entity.PasswordHash = oldEntity.PasswordHash;
                entity.Apellido = entity.Apellido.ToUpper();
                entity.Nombre = entity.Nombre.ToUpper();

                if (entity.Email != null)
                {
                    entity.Email = entity.Email.ToLower();
                }

                Update(entity);
                TempData["msj"] = "updateOK";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                this.HandleException(ex);
            }

            PopulateViewBagForEdit(model);

            return View(model);
        }

        public ActionResult Estado(long id, byte opcion)
        {
            User user = new User();

            using (AmericaVirtualEntities connection = new AmericaVirtualEntities())
            {
                user = connection.User.Find(id);
            }

            if (opcion == 0) { ViewBag.Estado = "Baja"; } else { ViewBag.Estado = "Alta"; }

            return View(user);
        }

        [HttpPost, ActionName("Estado")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id, byte opcion)
        {
            User user = new User();

            using (AmericaVirtualEntities connection = new AmericaVirtualEntities())
            {
                user = connection.User.Find(id);
                if (opcion == 0) { user.Activo = false; } else { user.Activo = true; }
                connection.Entry(user).State = EntityState.Modified;
                connection.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        protected override User ReadForEditOrDetail(long id)
        {
            User entity = null;

            using (AmericaVirtualEntities connection = new AmericaVirtualEntities())
            {
                entity = connection.User.Include("Rol").FirstOrDefault(x => x.UserID == id);
            }

            //using (AmericaVirtualEntities connection = new AmericaVirtualEntities())
            //{
            //    Guid ente = Guid.Parse(AccountHelper.GetEnte());
            //    ViewBag.Entes = connection.Ente.Where(e => e.ORID == ente).ToList();
            //}

            return entity;
        }

        protected override void PopulateViewBagForCreate(UserModel model)
        {
            IList<Rol> rules = GetRoles();
            model.Roles = new List<RolVM>();
            var config = new MapperConfiguration(cfg => { cfg.CreateMap<Rol, RolVM>(); });
            IMapper mapper = config.CreateMapper();
            model.Roles = mapper.Map<ICollection<Rol>, List<RolVM>>(rules);

            //using (AmericaVirtualEntities connection = new AmericaVirtualEntities())
            //{
            //    User user = AccountHelper.GetCurrentUser();

            //    if (user.Usuario == "SIIT")
            //    {
            //        string en = AccountHelper.GetEnte();
            //        Guid ente = Guid.Parse(en);
            //        ViewBag.Entes = connection.Ente.Where(e => e.ORID == ente).ToList();
            //    }

            //    ViewBag.Entes = connection.Ente.ToList();
            //}

            base.PopulateViewBagForCreate(model);
        }

        public override void Insert(User entity)
        {
            using (AmericaVirtualEntities connection = new AmericaVirtualEntities())
            {
                List<long> rolIds = entity.Rol.Select(y => y.RolID).ToList();
                entity.Rol = connection.Rol.Where(x => rolIds.Contains(x.RolID)).ToList();

                entity.Activo = true;
                entity.Foto = "user.jpg";
                entity.PasswordHash = "D44559F602EAB6FD62AC7680DACBFAADD13630335E951F97AF390E9DE176B6DB28512F2E0B9D4FBA5133E8B1C6E8DF59DB3A8AB9D60BE4B97CC9E81DB";

                entity.Usuario = entity.Usuario.ToLower();
                entity.Apellido = entity.Apellido.ToUpper();
                entity.Nombre = entity.Nombre.ToUpper();
                entity.IdEnte = connection.Ente.Where(e => e.CODE == "0001").Select(e => e.ORID).FirstOrDefault();

                if (entity.Email != null) { entity.Email = entity.Email.ToLower(); }

                connection.User.Add(entity);
                connection.SaveChanges();
            }
        }

        public override void Update(User entity)
        {
            entity.Activo = true;
            entity.Foto = "user.jpg";

            using (AmericaVirtualEntities connection = new AmericaVirtualEntities())
            {
                try
                {
                    var dbEnt = connection.User
                                            .Include("Rol")
                                            .Single(x => x.UserID == entity.UserID);

                    connection.Entry(dbEnt).CurrentValues.SetValues(entity);

                    UpdateRelatedCollection<User, Rol, long>(entity, connection, dbEnt, x => x.Rol, y => y.RolID);

                    connection.SaveChanges();

                    entity = dbEnt;
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }
        }

        [AuthorizeRule(true)]
        public virtual ActionResult UpdatePassword()
        {
            var currentUser = AccountHelper.GetCurrentUser();
            User entity = ReadForEditOrDetail(currentUser.UserID);
            UserUpdatePassword model = new UserUpdatePassword() { UserID = entity.UserID };
            return View(model);
        }

        [HttpPost]
        [AuthorizeRule(true)]
        public ActionResult UpdatePassword(UserUpdatePassword model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    User oldEntity = ReadForEditOrDetail(model.UserID);
                    oldEntity.PasswordHash = Encryption.Encrypt(model.Password);
                    Update(oldEntity);
                    TempData["msj"] = "updateOK";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                this.HandleException(ex);
            }
            return View(model);
        }

        protected List<Rol> GetRoles()
        {
            using (AmericaVirtualEntities connection = new AmericaVirtualEntities())
            {
                connection.Configuration.LazyLoadingEnabled = false;
                connection.Configuration.ProxyCreationEnabled = false;

                IQueryable<Rol> query = connection.Set<Rol>();
                return query.OrderBy(r => r.Description).ToList();
            }
        }

        protected Rol GetRol(long id)
        {
            Rol entity = null;
            using (AmericaVirtualEntities connection = new AmericaVirtualEntities())
            {
                entity = connection.Set<Rol>().Find(id);
            }

            return entity;
        }

        protected List<RolVM> EntityToModel(List<Rol> entity)
        {
            var model = new List<RolVM>();
            var configRules = new MapperConfiguration(cfg => { cfg.CreateMap<Rol, RolVM>(); });
            IMapper mapperRules = configRules.CreateMapper();
            model = mapperRules.Map<ICollection<Rol>, List<RolVM>>(entity);

            return model;
        }

        protected override UserModel EntityToModel(User entity)
        {
            UserModel model = new UserModel();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserModel>();
                cfg.CreateMap<Rol, RolVM>();
            });
            IMapper mapper = config.CreateMapper();
            model = mapper.Map<User, UserModel>(entity);
            // ROLES
            List<RolVM> allRoles = EntityToModel(GetRoles());
            foreach (Rol item in entity.Rol)
            {
                allRoles.Find(x => x.RolID == item.RolID).Selected = true;
            }
            model.Roles = allRoles.OrderBy(x => x.Description).ThenBy(x => x.Selected).ToList();

            return model;
        }

        protected override User ModelToEntity(UserModel model)
        {
            User entity = base.ModelToEntity(model);
            //if (model.Password != null)
            //    entity.PasswordHash = Encryption.Encrypt(model.Password);

            if (model.Roles != null)
            {
                var RolIdSelected = model.Roles.Where(x => x.Selected == true && x.RolID > 0).Select(x => x.RolID).ToList();
                foreach (long id in RolIdSelected)
                {
                    var selectedRol = GetRol(id);
                    if (selectedRol != null) entity.Rol.Add(selectedRol);
                }
            }

            return entity;
        }

        [HttpPost]
        [AuthorizeRule(true)]
        public JsonResult ExisteUsuario(string Usuario)
        {
            var user = AccountHelper.ExisteUser(Usuario);

            return Json(user == null);
        }
    }
}