using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ClimaAV.Database;
using ClimaAV.Models;
using AutoMapper;

namespace ClimaAV.Controllers
{
    public class RolController : BaseABMController<Rol, RolIndex, RolModel, long>
    {
        protected override Rol ReadForEditOrDetail(long id)
        {
            Rol entity = null;

            using (AmericaVirtualEntities connection = new AmericaVirtualEntities())
            {
                entity = connection.Rol.Include("Rule").FirstOrDefault(x => x.RolID == id);
            }

            return entity;
        }
        protected override void PopulateViewBagForCreate(RolModel model)
        {
            IList<Rule> rules = GetReglas();
            model.Rules = new List<RuleModel>();
            var config = new MapperConfiguration(cfg => { cfg.CreateMap<Rule, RuleModel>(); });
            IMapper mapper = config.CreateMapper();
            model.Rules = mapper.Map<ICollection<Rule>, List<RuleModel>>(rules);

            model.Rules = GenerateGroupNames(model.Rules);

            base.PopulateViewBagForCreate(model);
        }
        private List<RuleModel> GenerateGroupNames(List<RuleModel> list)
        {
            foreach (RuleModel rule in list.OrderBy(l => l.Definicion))
            {
                if (rule.Definicion.Contains("."))
                    rule.GroupName = rule.Definicion.Split('.')[0];
            }

            var groups = list.Select(l => l.GroupName).Distinct();

            List<RuleModel> newModel = new List<RuleModel>();

            int nextId = 0;
            foreach (string g in groups)
            {
                RuleModel ruleGroup = new RuleModel();
                ruleGroup.GroupName = g;
                ruleGroup.Id = nextId--;

                newModel.Add(ruleGroup);

                bool selectedAll = true;

                foreach (RuleModel rule in list.Where(l => l.GroupName == g).OrderBy(l => l.Definicion))
                {
                    newModel.Add(rule);
                    if (!rule.Selected)
                        selectedAll = false;
                }
                ruleGroup.Selected = selectedAll;
            }

            return newModel;
        }
        protected List<Rule> GetReglas()
        {
            using (AmericaVirtualEntities connection = new AmericaVirtualEntities())
            {
                connection.Configuration.LazyLoadingEnabled = false;
                connection.Configuration.ProxyCreationEnabled = false;

                IQueryable<Rule> query = connection.Set<Rule>();
                return query.ToList();
            }
        }
        protected Rule GetRegla(int id)
        {
            Rule entity = null;
            using (AmericaVirtualEntities connection = new AmericaVirtualEntities())
            {
                entity = connection.Set<Rule>().Find(id);
            }

            return entity;
        }
        protected override Rol ModelToEntity(RolModel model)
        {
            Rol entity = new Rol();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RolModel, Rol>().ForMember(x => x.Rule, opt => opt.Ignore());
            });
            IMapper mapper = config.CreateMapper();
            entity = mapper.Map<RolModel, Rol>(model);

            if (model.Rules != null)
            {
                var RuleIdSelected = model.Rules.Where(x => x.Selected == true && x.Id > 0).Select(x => x.Id).ToList();
                foreach (int id in RuleIdSelected)
                {
                    var selectedRule = GetRegla(id);
                    if (selectedRule != null) entity.Rule.Add(selectedRule);
                }
            }
            return entity;
        }

        protected override RolModel EntityToModel(Rol entity)
        {
            RolModel model = new RolModel();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Rol, RolModel>();
                cfg.CreateMap<Rule, RuleModel>();
            });
            IMapper mapper = config.CreateMapper();
            model = mapper.Map<Rol, RolModel>(entity);

            List<RuleModel> allRules = EntityToModel(GetReglas());

            foreach (Rule item in entity.Rule)
            {
                allRules.Find(x => x.Id == item.Id).Selected = true;
            }

            model.Rules = allRules.OrderBy(x => x.Definicion).ThenBy(x => x.Selected).ToList();

            return model;
        }

        private List<RuleModel> EntityToModel(List<Rule> entity)
        {
            List<RuleModel> model = new List<RuleModel>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Rule, RuleModel>()
                .ForMember(x => x.Selected, opt => opt.MapFrom(y => false));
            });

            IMapper mapper = config.CreateMapper();
            model = mapper.Map<List<Rule>, List<RuleModel>>(entity);
            return model;
        }

        public override ViewResult Details(long id)
        {
            Rol entity = ReadForEditOrDetail(id);

            RolModel rolModel = new RolModel();
            rolModel = EntityToModel(entity);

            rolModel.Rules = GenerateGroupNames(rolModel.Rules);

            return View(rolModel);
        }

        public override ActionResult Edit(long id)
        {
            Rol entity = ReadForEditOrDetail(id);

            RolModel rolModel = new RolModel();
            rolModel = EntityToModel(entity);

            rolModel.Rules = GenerateGroupNames(rolModel.Rules);

            return View(rolModel);
        }

        public override void Insert(Rol entity)
        {
            using (AmericaVirtualEntities connection = new AmericaVirtualEntities())
            {
                List<int> ruleIds = entity.Rule.Select(y => y.Id).ToList();
                entity.Rule = connection.Rule.Where(x => ruleIds.Contains(x.Id)).ToList();

                connection.Rol.Add(entity);
                connection.SaveChanges();
            }
        }

        public override void Update(Rol entity)
        {
            using (AmericaVirtualEntities connection = new AmericaVirtualEntities())
            {
                try
                {
                    var dbEnt = connection.Rol
                                            .Include("Rule")
                                            .Single(x => x.RolID == entity.RolID);

                    connection.Entry(dbEnt).CurrentValues.SetValues(entity);

                    UpdateRelatedCollection<Rol, Rule, int>(entity, connection, dbEnt, x => x.Rule, y => y.Id);

                    connection.SaveChanges();

                    entity = dbEnt;
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }
        }

        //public override ActionResult Create(RoleVM model)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            Role entity = ModelToEntity(model);

        //            Business.Insert(entity);
        //            TempData["msj"] = "createOK";
        //            return RedirectToAction("Index");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        this.HandleException(ex);
        //    }

        //    PopulateViewBagForCreate(model);

        //    return View(model);

        //}

        //protected override void PopulateViewBagForCreate(RoleVM model)
        //{
        //    RuleBusiness rBusiness = new RuleBusiness();
        //    IList<Rule> rules = rBusiness.GetList(x => x.Active);
        //    AutoMapper.Mapper.CreateMap<Rule, RuleVM>();
        //    model.Rules = new List<RuleVM>();
        //    AutoMapper.Mapper.Map(rules, model.Rules);

        //    model.Rules = GenerateGroupNames(model.Rules);

        //    UserBusiness uBusiness = new UserBusiness();
        //    IList<User> users = uBusiness.GetList(x => x.Active);
        //    AutoMapper.Mapper.CreateMap<User, UserRoleVM>();
        //    model.Users = new List<UserRoleVM>();
        //    AutoMapper.Mapper.Map(users, model.Users);

        //    base.PopulateViewBagForCreate(model);
        //}

        //private List<RuleVM> GenerateGroupNames(List<RuleVM> list)
        //{
        //    foreach (RuleVM rule in list.OrderBy(l => l.RuleDefinition))
        //    {
        //        if (rule.RuleDefinition.Contains("."))
        //            rule.GroupName = rule.RuleDefinition.Split('.')[0];
        //    }

        //    var groups = list.Select(l => l.GroupName).Distinct();

        //    List<RuleVM> newModel = new List<RuleVM>();

        //    int nextId = 0;
        //    foreach (string g in groups)
        //    {
        //        RuleVM ruleGroup = new RuleVM();
        //        ruleGroup.GroupName = g;
        //        ruleGroup.RuleID = nextId--;

        //        newModel.Add(ruleGroup);

        //        bool selectedAll = true;

        //        foreach (RuleVM rule in list.Where(l => l.GroupName == g).OrderBy(l => l.RuleDefinition))
        //        {
        //            newModel.Add(rule);
        //            if (!rule.Selected)
        //                selectedAll = false;
        //        }
        //        ruleGroup.Selected = selectedAll;
        //    }

        //    return newModel;
        //}

        //public override ViewResult Details(long id)
        //{
        //    Role entity = this.Business.Get(x => x.RoleID == id, new string[] { "User", "Rule" });

        //    RoleVM rolModel = new RoleVM();
        //    rolModel = EntityToModel(entity);

        //    rolModel.Rules = GenerateGroupNames(rolModel.Rules);

        //    return View(rolModel);
        //}

        //public override ActionResult Edit(long id)
        //{
        //    Role entity = this.Business.Get(x => x.RoleID == id, new string[] { "User", "Rule" });

        //    RoleVM rolModel = new RoleVM();
        //    rolModel = EntityToModel(entity);

        //    rolModel.Rules = GenerateGroupNames(rolModel.Rules);

        //    return View(rolModel);
        //}

        //[HttpPost]
        //public override ActionResult Edit(RoleVM model)
        //{
        //    Role role = new Role();
        //    role = ModelToEntity(model);

        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            Business.Update(role);
        //            TempData["msj"] = "updateOK";
        //            return RedirectToAction("Index");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        if (ex.GetType() == typeof(ValidationException))
        //            ModelState.AddModelError("", ResourcesHelper.GetResourceValue(ex.Message));
        //        else
        //            ModelState.AddModelError("", ResourcesHelper.GetResourceValue("Error"));
        //    }

        //    return View(model);
        //}

        //public override JsonResult DeleteConfirmed(long id)
        //{
        //    try
        //    {
        //        Role entity = Business.Single(x => x.RoleID == id, new string[] { "User", "Rule" });
        //        Business.Delete(entity);
        //    }
        //    catch (ValidationException ex)
        //    {
        //        return Json(HandleJSONValidationException(ex), JsonRequestBehavior.AllowGet);
        //    }
        //    return Json(true, JsonRequestBehavior.AllowGet);
        //}
    }
}