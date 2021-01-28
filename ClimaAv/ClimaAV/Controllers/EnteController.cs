using AutoMapper;
using ClimaAV.Models;
using ClimaAV.Database;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ClimaAV.Filters;
using ClimaAV.Helpers;

namespace ClimaAV.Controllers
{
    //[AuthorizeRule]
    public class EnteController : Controller
    {
        private AmericaVirtualEntities db = new AmericaVirtualEntities();

        public ActionResult Index()
        {
            List<Ente> ente = db.Ente.Include(e => e.Localidad).ToList();
            List<EnteModel> em = ModelToEntityList(ente);

            return View(em);
        }

        protected List<Especialidad> GetEspecialidades()
        {
            using (AmericaVirtualEntities connection = new AmericaVirtualEntities())
            {
                connection.Configuration.LazyLoadingEnabled = false;
                connection.Configuration.ProxyCreationEnabled = false;

                IQueryable<Especialidad> query = connection.Set<Especialidad>();
                return query.OrderBy(e => e.Descripcion).ToList();
            }
        }

        public ActionResult Create()
        {
            EnteModel model = new EnteModel();

            IList<Especialidad> esp = GetEspecialidades();
            model.Especialidades = new List<EspecialidadVM>();
            var config = new MapperConfiguration(cfg => { cfg.CreateMap<Especialidad, EspecialidadVM>(); });
            IMapper mapper = config.CreateMapper();
            model.Especialidades = mapper.Map<ICollection<Especialidad>, List<EspecialidadVM>>(esp);

            ViewBag.Localidad = db.Localidad.OrderBy(l => l.Descripcion).ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EnteModel model)
        {
            if (ModelState.IsValid)
            {
                Ente ente = ModelToEntity(model);

                ente.ORID = Guid.NewGuid();
                ente.Foto = "mlz.png";
                ente.Descripcion = ente.Descripcion.ToUpper();
                if (ente.Calle != null) { ente.Calle = ente.Calle.ToUpper(); }

                List<Guid> espIds = model.Especialidades.Where(a => a.Selected == true)
                    .Select(y => y.ORID).ToList();
                if (espIds.Count != 0)
                {
                    foreach (var item in espIds)
                    {
                        ente.EnteEspecialidad.Add(new EnteEspecialidad
                        {
                            ORID = Guid.NewGuid(),
                            IdEnte = ente.ORID,
                            IdEspecialidad = item
                        });
                    }
                }

                db.Ente.Add(ente);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.Localidad = db.Localidad.OrderBy(l => l.Descripcion).ToList();

            return View();
        }

        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Ente ente = db.Ente.Find(id);
            EnteModel model = EntityToModel(ente);

            if (model == null)
            {
                return HttpNotFound();
            }

            IList<Especialidad> esp = GetEspecialidades();
            model.Especialidades = new List<EspecialidadVM>();
            var config = new MapperConfiguration(cfg => { cfg.CreateMap<Especialidad, EspecialidadVM>(); });
            IMapper mapper = config.CreateMapper();
            model.Especialidades = mapper.Map<ICollection<Especialidad>, List<EspecialidadVM>>(esp);

            List<EnteEspecialidad> ee = db.EnteEspecialidad.Where(e => e.IdEnte == model.ORID).ToList();

            foreach (var item in model.Especialidades)
            {
                foreach (var item2 in ee)
                {
                    if (item2.IdEspecialidad == item.ORID)
                    {
                        item.Selected = true;
                    }
                }
            }

            ViewBag.Localidad = db.Localidad.OrderBy(l => l.Descripcion).ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EnteModel model)
        {
            try
            {
                List<EnteEspecialidad> ee = db.EnteEspecialidad.Where(e => e.IdEnte == model.ORID).ToList();

                foreach (var item in ee)
                {
                    db.EnteEspecialidad.Remove(item);
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {

                throw;
            }

            Ente ente = ModelToEntity(model);

            if (ModelState.IsValid)
            {
                ente.Foto = "mlz.png";
                ente.Descripcion = ente.Descripcion.ToUpper();
                if (ente.Calle != null) { ente.Calle = ente.Calle.ToUpper(); }

                List<Guid> espIds = model.Especialidades.Where(a => a.Selected == true)
                    .Select(y => y.ORID).ToList();
                if (espIds.Count != 0)
                {
                    foreach (var item in espIds)
                    {
                        EnteEspecialidad ee = new EnteEspecialidad();

                        db.EnteEspecialidad.Add(new EnteEspecialidad
                        {
                            ORID = Guid.NewGuid(),
                            IdEnte = ente.ORID,
                            IdEspecialidad = item
                        });
                        db.SaveChanges();
                    }
                }

                db.Entry(ente).State = EntityState.Modified;
                db.SaveChanges();


                return RedirectToAction("Index");
            }

            ViewBag.Localidad = db.Localidad.OrderBy(l => l.Descripcion).ToList();

            return View();
        }

        // GET: /Ente/Delete/5
        //public ActionResult Delete(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Ente ente = db.Ente.Find(id);
        //    if (ente == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(ente);
        //}

        // POST: /Ente/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(Guid id)
        //{
        //    Ente ente = db.Ente.Find(id);
        //    db.Ente.Remove(ente);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        private Ente ModelToEntity(EnteModel model)
        {
            Ente entity = new Ente();

            var config = new MapperConfiguration(cfg => { cfg.CreateMap<EnteModel, Ente>(); });

            IMapper mapper = config.CreateMapper();
            entity = mapper.Map<EnteModel, Ente>(model);
            return entity;
        }

        private EnteModel EntityToModel(Ente entity)
        {
            EnteModel model = new EnteModel();

            var config = new MapperConfiguration(cfg => { cfg.CreateMap<Ente, EnteModel>(); });

            IMapper mapper = config.CreateMapper();
            model = mapper.Map<Ente, EnteModel>(entity);
            return model;
        }

        private List<EnteModel> ModelToEntityList(List<Ente> db)
        {
            List<EnteModel> entity = new List<EnteModel>();

            var config = new MapperConfiguration(cfg => { cfg.CreateMap<Ente, EnteModel>(); });

            IMapper mapper = config.CreateMapper();
            entity = mapper.Map<List<Ente>, List<EnteModel>>(db);
            return entity;
        }

        [HttpPost]
        //[AuthorizeRule(true)]
        public JsonResult ExisteCODE(string code)
        {
            var ente = db.Ente.Where(e => e.CODE == code).FirstOrDefault();

            return Json(ente == null);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
