using ClimaAV.Database;
using ClimaAV.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ClimaAV.Filters
{
    public class AuthorizeRuleAttribute : FilterAttribute, IAuthorizationFilter
    {
        private string rule = string.Empty;
        private bool ignore = false;
        private bool permisoComun = false;

        public AuthorizeRuleAttribute()
        {

        }

        public AuthorizeRuleAttribute(string rule)
        {
            this.rule = rule;
            permisoComun = false;
        }

        public AuthorizeRuleAttribute(bool ignore)
        {
            this.ignore = ignore;
            if (ignore == false)
            {
                permisoComun = true;
            }
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            User usuario = new User();
            if (AccountHelper.GetCurrentUser() != null)
            {
                if (!ignore)
                {
                    usuario = AccountHelper.GetCurrentUser();

                    string ruleDefinition = string.Empty;

                    if (string.IsNullOrEmpty(rule))
                        ruleDefinition = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName + "." + filterContext.ActionDescriptor.ActionName;
                    else
                        ruleDefinition = this.rule;

                    var entityID = filterContext.RouteData.Values["id"];

                    if (!permisoComun)
                    {
                        if (!AccountHelper.IsAuthorized(ruleDefinition) ||
                            (entityID != null && usuario.UserID.ToString() != entityID.ToString()
                            && filterContext.ActionDescriptor.ActionName == "EditProfile"
                            && filterContext.ActionDescriptor.ControllerDescriptor.ControllerName == "Usuario1"))
                        {
                            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "Login" }));
                        }
                    }
                }

            }
            else
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "Login", returnUrl = filterContext.HttpContext.Request.Url.PathAndQuery }));
        }
    }
}