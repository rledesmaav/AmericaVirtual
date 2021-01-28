using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClimaAV.Models;
using ClimaAV.Database;
using System.Globalization;
using System.Threading;
using System.Web.Security;
using ClimaAV.Helpers;
//using AutoMapper;

namespace ClimaAV.Controllers
{
    //[Authorize]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        public ActionResult Login(string returnUrl, string culture = "es")
        {
            HttpContext.Session.Add("Culture", new CultureInfo(culture));
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(culture);

            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Culture = culture;

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(LoginModel model) /*, string returnURL*/
        {
            string errorLogin = "Error al intentar ingresar";

            if (ModelState.IsValid)
            {
                var result = LoginUser(model.UserName, model.Password);
                if (result == LoginResult.Succeed)
                {
                    //if (model.RememberMe)
                    //{
                    //    //Crear cookie
                    //    var authTicket = new FormsAuthenticationTicket(
                    //      1,
                    //      model.UserName,
                    //      DateTime.Now,
                    //      DateTime.Now.AddDays(7),
                    //      model.RememberMe,
                    //      model.Password
                    //    );
                    //    HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(authTicket));
                    //    Response.Cookies.Add(cookie);
                    //}

                    AccountHelper.GetCurrentUserRules();
                    //if (!string.IsNullOrEmpty(returnURL))
                    //    return RedirectToLocal(returnURL);
                    //else

                    //return RedirectToLocal(returnURL);
                    return RedirectToAction("Home", "Home");
                }
                else
                {
                    if (result == LoginResult.InvalidUserNamePassword)
                        errorLogin = "Usuario o contraseña incorrectos";
                    //else if (result == LoginResult.AccountLocked)
                    //    errorLogin = ResourcesHelper.GetResourceValue("AccountLocked");
                }

            }

            ModelState.AddModelError("", errorLogin);
            return View(model);
        }



        private LoginResult LoginUser(string username, string password)
        {
            string passwordHash = Encryption.Encrypt(password);

            User user = GetUser(username, passwordHash);
            if (user != null)
            {
                AccountHelper.SetCurrentUser(user);
                return LoginResult.Succeed;
            }
            else
            {
                AccountHelper.SetCurrentUser(null);
                AccountHelper.SetPuesto(null);
                return LoginResult.InvalidUserNamePassword;
            }
        }



        private User GetUser(string username, string hash)
        {

            User entity;
            using (AmericaVirtualEntities connection = new AmericaVirtualEntities())
            {
                entity = connection.User.FirstOrDefault(x => x.Usuario == username && x.PasswordHash == hash && x.Activo == true);
            }
            return entity;
        }


        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult LogOff()
        {
            Session.Remove("ApplicationUser");
            Session.Remove("Rules");
            Session.Remove("EnteId");

            Session.Abandon();
            Session.Clear();

            return RedirectToAction("Login", new { returnUrl = string.Empty });
        }

        //[AllowAnonymous]
        public ActionResult UpdatePassword()
        {
            var currentUser = AccountHelper.GetCurrentUser();
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            User entity = ReadForEditOrDetail(currentUser.UserID);
            UserUpdatePassword model = new UserUpdatePassword() { UserID = entity.UserID };
            return View(model);
        }

        [HttpPost]
        public ActionResult UpdatePassword(UserUpdatePassword model)
        {
            string passwordHash = Encryption.Encrypt(model.PasswordOld);
            var currentUser = AccountHelper.GetCurrentUser();

            if (currentUser.PasswordHash == passwordHash)
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
#pragma warning disable CS0168 // La variable 'ex' se ha declarado pero nunca se usa
                catch (Exception ex)
#pragma warning restore CS0168 // La variable 'ex' se ha declarado pero nunca se usa
                {
                    //this.HandleException(ex);
                }
            }

            return View(model);
        }


        protected User ReadForEditOrDetail(long id)
        {
            User entity = null;

            using (AmericaVirtualEntities connection = new AmericaVirtualEntities())
            {
                entity = connection.User.FirstOrDefault(x => x.UserID == id);
            }

            return entity;
        }

        public void Update(User entity)
        {

            using (AmericaVirtualEntities connection = new AmericaVirtualEntities())
            {
                try
                {
                    var dbEnt = connection.User
                                            .Single(x => x.UserID == entity.UserID);

                    connection.Entry(dbEnt).CurrentValues.SetValues(entity);
                    connection.SaveChanges();

                    entity = dbEnt;
                }
#pragma warning disable CS0168 // La variable 'ex' se ha declarado pero nunca se usa
                catch (Exception ex)
#pragma warning restore CS0168 // La variable 'ex' se ha declarado pero nunca se usa
                {
                    //HandleException(ex);
                }
            }
        }


    }
}