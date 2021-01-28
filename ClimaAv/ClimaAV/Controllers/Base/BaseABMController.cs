using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Data.Entity.Validation;
using System.Linq;
using AutoMapper;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using ClimaAV.Database;
using System.Linq.Expressions;
using ClimaAV.Filters;

namespace ClimaAV.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="B">Business Type</typeparam>
    /// <typeparam name="E">Entity to manage</typeparam>
    /// <typeparam name="I">ViewModel to use on index</typeparam>
    /// <typeparam name="VM">ViewModel to use on create, edit and details actions</typeparam>
    /// <typeparam name="TID">Datatype for the primary key field</typeparam>
    
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    [AuthorizeRule]
    public abstract class BaseABMController<E, I, VM, TID> : Controller
        where E : class, new()
        where I : class, new()
        where VM : class, new()
    {

        //DB methods
        public virtual E Read(object id)
        {
            E entity = null;

            using (AmericaVirtualEntities connection = new AmericaVirtualEntities())
            {
                entity = connection.Set<E>().Find(id);
            }

            return entity;
        }

        public virtual IList<E> GetList()
        {
            IList<E> list = new List<E>();

            using (AmericaVirtualEntities connection = new AmericaVirtualEntities())
            {
                connection.Configuration.LazyLoadingEnabled = false;
                connection.Configuration.ProxyCreationEnabled = false;

                IQueryable<E> query = connection.Set<E>();
                list = query.ToList();
            }

            return list;
        }
        
        public virtual IList<I> GetIndexList(string storedProcedure)
        {
            IList<I> list = new List<I>();

            using (AmericaVirtualEntities connection = new AmericaVirtualEntities())
            {
                list = connection.Database.SqlQuery<I>(storedProcedure).ToList<I>();
            }

            return list;
        }

        public virtual IList<I> GetList(string storedProcedure, params KeyValuePair<string, object>[] parameters)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();

            if (parameters != null)
            {
                for (int idx = 0; idx < parameters.Length; idx++)
                {
                    storedProcedure += " @" + parameters[idx].Key;
                    if (idx < (parameters.Length - 1))
                    {
                        storedProcedure += ",";
                    }
                }

                IList<I> list = new List<I>();

                using (AmericaVirtualEntities connection = new AmericaVirtualEntities())
                {
                    list = connection.Database.SqlQuery<I>(storedProcedure, CreateSqlParameters(parameters)).ToList<I>();
                }

                return list;
            }
            else
            {
                IList<I> list = new List<I>();

                using (AmericaVirtualEntities connection = new AmericaVirtualEntities())
                {
                    list = connection.Database.SqlQuery<I>(storedProcedure).ToList<I>();
                }

                return list;
            }
        }

        protected virtual SqlParameter[] CreateSqlParameters(KeyValuePair<string, object>[] parameters)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();

            if (parameters != null)
            {
                for (int idx = 0; idx < parameters.Length; idx++)
                {
                    SqlParameter sqlParameter = new SqlParameter("@" + parameters[idx].Key, DBNull.Value);

                    if (parameters[idx].Value != null)
                    {
                        if (parameters[idx].Value.ToString() == "true" || parameters[idx].Value.ToString() == "false")
                            sqlParameter.Value = (Convert.ToBoolean(parameters[idx].Value) ? 1 : 0);
                        else
                            sqlParameter.Value = parameters[idx].Value;
                    }

                    sqlParameters.Add(sqlParameter);
                }
            }
            return sqlParameters.ToArray();
        }

        public virtual void Insert(E entity)
        {
            using (AmericaVirtualEntities connection = new AmericaVirtualEntities())
            {
                try
                {
                    connection.Set<E>().Add(entity);
                    connection.SaveChanges();
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }
        }

        protected virtual void RemoveDeletedChild<R>(AmericaVirtualEntities connection, R oldEntity) where R : class, new()
        {
            connection.Set<R>().Remove(oldEntity);
        }

        protected void UpdateRelatedCollection<P, R, TIDR>(P entity, AmericaVirtualEntities connection, P dbEnt, Expression<Func<P, ICollection<R>>> path, System.Linq.Expressions.Expression<Func<R, TIDR>> idPath)
            where P : class, new()
            where R : class, new()
        {
            UpdateRelatedCollection<P, R, TIDR>(entity, connection, dbEnt, path, idPath, false);
        }

        protected void UpdateRelatedCollection<P, R, TIDR>(P entity, AmericaVirtualEntities connection, P dbEnt, Expression<Func<P, ICollection<R>>> path, System.Linq.Expressions.Expression<Func<R, TIDR>> idPath, bool deleteChildren)
            where P : class, new()
            where R : class, new()
        {
            LambdaExpression lambda = path;
            var compiledLambda = lambda.Compile();
            LambdaExpression lambdaID = idPath;
            var compiledLambdaID = lambdaID.Compile();
            ICollection<R> newCollection = (ICollection<R>)compiledLambda.DynamicInvoke(entity);
            ICollection<R> dbCollection = (ICollection<R>)compiledLambda.DynamicInvoke(dbEnt);

            List<TIDR> newIDs = newCollection.Select<R, TIDR>(idPath.Compile()).ToList();
            List<TIDR> oldIDs = dbCollection.Select<R, TIDR>(idPath.Compile()).ToList();

            foreach (TIDR id in oldIDs)
            {
                if (!newIDs.Contains(id))
                {
                    R oldEntity = null;
                    foreach (var e in dbCollection)
                    {
                        if (compiledLambdaID.DynamicInvoke(e).Equals(id))
                        {
                            oldEntity = e;
                            break;
                        }
                    }
                    dbCollection.Remove(oldEntity);
                    if (deleteChildren)
                        RemoveDeletedChild(connection, oldEntity);
                }
            }

            foreach (TIDR id in newIDs)
            {
                if (!oldIDs.Contains(id))
                {
                    R newEntity = connection.Set<R>().Find(id);
                    if (newEntity != null)
                        dbCollection.Add(newEntity);
                }
                else
                {
                    R oldEntity = null;
                    foreach (var e in dbCollection)
                    {
                        if (compiledLambdaID.DynamicInvoke(e).Equals(id))
                        {
                            oldEntity = e;
                            break;
                        }
                    }
                    R newEntity = null;
                    foreach (var e in newCollection)
                    {
                        if (compiledLambdaID.DynamicInvoke(e).Equals(id))
                        {
                            newEntity = e;
                            break;
                        }
                    }
                    connection.Entry(oldEntity).CurrentValues.SetValues(newEntity);
                }
            }

            //add the new objects to the collection
            long idAdd = 0;
            foreach (var e in newCollection)
            {
                if (compiledLambdaID.DynamicInvoke(e).Equals(idAdd))
                    dbCollection.Add(e);
            }
        }

        public virtual void Update(E entity)
        {
            using (AmericaVirtualEntities connection = new AmericaVirtualEntities())
            {
                try
                {
                    connection.Set<E>().Attach(entity);
                    connection.Entry<E>(entity).State = EntityState.Modified;
                    connection.SaveChanges();
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }
        }

        public virtual void Delete(E entity)
        {
            using (AmericaVirtualEntities connection = new AmericaVirtualEntities())
            {
                try
                {
                    connection.Set<E>().Attach(entity);
                    connection.Set<E>().Remove(entity);
                    connection.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException is UpdateException)
                    {
                        if (ex.InnerException.InnerException is SqlException)
                        {
                            if (((SqlException)ex.InnerException.InnerException).Number == 547)
                            {
                                throw new Exception("No se puede eliminar el registro por tener registros asociados.", ex);
                            }
                            else
                            {
                                throw new Exception("Ha ocurrido un error al intentar actualizar la entidad.", ex);
                            }
                        }
                        else
                        {
                            throw new Exception("Ha ocurrido un error al intentar actualizar la entidad.", ex);
                        }
                    }
                    else
                    {
                        throw new Exception("Ha ocurrido un error al intentar actualizar la entidad.", ex);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Ha ocurrido un error al intentar actualizar la entidad.", ex);
                }
            }
        }

        //

        public virtual ActionResult Index()
        {
            PopulateViewBagForIndex();

            IEnumerable<I> model = GetIndexList(GetIndexStoredProcedureName());

            return View(model);
        }

        public virtual string GetIndexStoredProcedureName()
        {
            return "Index" + typeof(E).Name;
        }

        public virtual ViewResult Details(TID id)
        {            
            E entity = ReadForEditOrDetail(id);
            VM model = EntityToModel(entity);
            PopulateViewBagForDetails(model);
            return View(model);
        }

        public virtual ActionResult Create()
        {
            VM model = new VM();
            PopulateViewBagForCreate(model);
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult Create(VM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    E entity = ModelToEntity(model);                    
                    Insert(entity);
                    
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                this.HandleException(ex);
            }

            PopulateViewBagForCreate(model);

            return View(model);
        }

        protected string GetModelStateValidationErrors(ModelStateDictionary modelState)
        {
            string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
            return messages;
        }
        
        protected virtual T Map<S, T>(S source)
             where T : class, new()
        {
            if (source == null)
                return null;
            if (typeof(T) == typeof(S))
                return source as T;

            T entity = new T();
            var config = new MapperConfiguration(cfg => { cfg.CreateMap<S, T>(); });
            config.CreateMapper().Map(source, entity);
            return entity;
        }

        protected virtual E ModelToEntity(VM model)
        {
            return this.Map<VM, E>(model);
        }

        protected virtual VM EntityToModel(E entity)
        {
            return this.Map<E, VM>(entity);
        }

        protected virtual List<T> MapList<S, T>(IEnumerable<S> source)
           where T : class, new()
        {
            if (source == null)
                return null;
            if (typeof(T) == typeof(S))
                return source as List<T>;

            List<T> entity = new List<T>();
            var config = new MapperConfiguration(cfg => { cfg.CreateMap<S, T>(); });
            config.CreateMapper().Map(source, entity);
            return entity;
        }

        public virtual ActionResult Edit(TID id)
        {
            E entity = ReadForEditOrDetail(id);
            VM model = EntityToModel(entity);
            PopulateViewBagForEdit(model);
            return View(model);
        }

        protected virtual E ReadForEditOrDetail(TID id)
        {
            return Read(id);
        }

        [HttpPost]
        public virtual ActionResult Edit(VM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    E entity = ModelToEntity(model);
                    Update(entity);
                    TempData["msj"] = "updateOK";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                this.HandleException(ex);
            }
            PopulateViewBagForEdit(model);
            return View(model);
        }

        protected virtual void PopulateViewBagForEdit(VM model)
        {

        }

        protected virtual void PopulateViewBagForCreate(VM model)
        {

        }

        protected virtual void PopulateViewBagForIndex()
        {
            this.GetActions();
        }

        public void GetActions()
        {
            string nombreController = this.ControllerContext.RouteData.Values["controller"].ToString();

            string accionVer = string.Format("{0}.Details", nombreController);
            string accionCrear = string.Format("{0}.Create", nombreController);
            string accionEditar = string.Format("{0}.Edit", nombreController);
            string accionEliminar = string.Format("{0}.Delete", nombreController);

            /*bool permiteVer = AccountHelper.IsAuthorized(accionVer);
            bool permiteCrear = AccountHelper.IsAuthorized(accionCrear);
            bool permiteEditar = AccountHelper.IsAuthorized(accionEditar);
            bool permiteEliminar = AccountHelper.IsAuthorized(accionEliminar);*/
            bool permiteVer = true;
            bool permiteCrear = true;
            bool permiteEditar = true;
            bool permiteEliminar = true;

            ViewBag.PermiteVer = permiteVer;
            ViewBag.PermiteCrear = permiteCrear;
            ViewBag.PermiteEditar = permiteEditar;
            ViewBag.PermiteEliminar = permiteEliminar;
        }

        protected virtual void PopulateViewBagForDetails(VM model)
        {

        }

        public virtual ActionResult Delete(object id)
        {   
            E entity = Read(id);
            return View(entity);
        }

        [HttpPost, ActionName("Delete")]
        public virtual JsonResult DeleteConfirmed(TID id)
        {
            try
            {
                Delete(id);
            }
            catch (ValidationException ex)
            {
                return Json(HandleJSONValidationException(ex), JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        protected void HandleException(Exception ex)
        {
            if (ex is ValidationException)
                ModelState.AddModelError(string.Empty, HttpContext.GetGlobalResourceObject("Resources", ex.Message).ToString());
            else
            {
                //Utils.Services.Logger.Error("Unexpected Exception", ex);
                ModelState.AddModelError("", HttpContext.GetGlobalResourceObject("Resources", "Error").ToString());
            }
        }

        protected string HandleJSONValidationException(ValidationException ex)
        {
            string errorMessage = HttpContext.GetGlobalResourceObject("Resources", ex.Message).ToString();
            ValidationException val = ex as ValidationException;
            /*
            if (val.Parameters != null)
                errorMessage = string.Format(errorMessage, val.Parameters);
                */
            return errorMessage;
        }

        protected string GetEntityValidationErrors(DbEntityValidationException ex)
        {
            List<string> errorMessages = new List<string>();
            foreach (DbEntityValidationResult validationResult in ex.EntityValidationErrors)
            {
                string entityName = validationResult.Entry.Entity.GetType().Name;
                foreach (DbValidationError error in validationResult.ValidationErrors)
                {
                    errorMessages.Add(entityName + "." + error.PropertyName + ": " + error.ErrorMessage);
                }
            }
            return string.Join("; ", errorMessages);
        }

    }
}