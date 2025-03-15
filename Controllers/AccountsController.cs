using JSON_DAL;
using JsonDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static JsonDemo.Controllers.AccessControl;

namespace JsonDemo.Controllers
{
    public class AccountsController : Controller
    {
        [HttpPost]
        public JsonResult EmailExist(string Email)
        {
            return Json(DB.Users.ToList().Where(u => u.Email == Email).Any());
        }
        [HttpPost]
        public JsonResult EmailAvailable(string Email)
        {
            bool conflict = false;
            User connectedUser = (User)Session["ConnectedUser"];
            int currentId = connectedUser != null ? connectedUser.Id : 0;
            User foundUser = DB.Users.ToList().Where(u => u.Email == Email && u.Id != currentId).FirstOrDefault();
            conflict = foundUser != null;
            return Json(!conflict);
        }
        public ActionResult ExpiredSession()
        {
            return Redirect("/Accounts/Login?message=Session expirée, veuillez vous reconnecter.&success=false");
        }
        public ActionResult Logout()
        {
            return RedirectToAction("Login", "Accounts");
        }
        public ActionResult Login(string message = "", bool success = true)
        {
            Session["LoginSuccess"] = success;
            Session["LoginMessage"] = message;
            if (Session["CurrentLoginEmail"] == null) Session["currentLoginEmail"] = "";
            LoginCredential credential = new LoginCredential
            {
                Email = (string)Session["currentLoginEmail"]
            };
            User connectedUser = DB.Users.Get(Session["ConnectedUser"] != null ? ((User)Session["ConnectedUser"]).Id : 0);
            if (connectedUser != null) DB.Users.SetOnline(connectedUser, false);
            Session["ConnectedUser"] = null;
            return View(credential);
        }
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Login(LoginCredential credential)
        {
            credential.Email = credential.Email.Trim();
            credential.Password = credential.Password.Trim();
            Session["CurrentLoginEmail"] = credential.Email;
            User connectedUser = DB.Users.GetUser(credential);
            Session["ConnectedUser"] = connectedUser;
            if (connectedUser == null)
            {
                Session["LoginSuccess"] = false;
                Session["LoginMessage"] = "Courriel ou mot de passe incorrect";
                return View();
            }
            else
            {
                if (connectedUser.Blocked)
                {
                    return Redirect("/Accounts/Login?message=Votre compte a été bloqué!&success=false");
                }
                DB.Users.SetOnline(Session["ConnectedUser"], true);
            }
            return RedirectToAction("Index", "Students");
        }
        public ActionResult Subscribe()
        {
            Session["ConnectedUser"] = null;
            Session["CurrentLoginEmail"] = "";
            return View(new User());
        }
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Subscribe(User user)
        {
            DB.Users.Add(user);
            AccountsEmailing.SendEmailVerification(Url.Action("VerifyUser", "Accounts", null, Request.Url.Scheme), user);
            return Redirect("/Accounts/Login?message=Création de compte effectué avec succès! Un courriel de confirmation d'adresse vous a été envoyé.");
        }
        public ActionResult VerifyUser(string code)
        {
            UnverifiedEmail UnverifiedEmail = DB.UnverifiedEmails.ToList().Where(u => u.VerificationCode == code).FirstOrDefault();
            if (UnverifiedEmail != null)
            {
                User newlySubscribedUser = DB.Users.Get(UnverifiedEmail.UserId);

                DB.UnverifiedEmails.Delete(UnverifiedEmail.Id);
                if (newlySubscribedUser != null)
                {
                    newlySubscribedUser.Verified = true;
                    DB.Users.Update(newlySubscribedUser);
                    return Redirect("/Accounts/Login?message=Votre adresse de courriel a été vérifiée avec succès!");
                }
            }
            return Redirect("/Accounts/Login?message=Erreur de vérification de courriel!&success=false");
        }
        public ActionResult VerifyNewEmail(string code)
        {
            UnverifiedEmail UnverifiedEmail = DB.UnverifiedEmails.ToList().Where(u => u.VerificationCode == code).FirstOrDefault();
            if (UnverifiedEmail != null)
            {
                User user = DB.Users.Get(UnverifiedEmail.UserId);

                if (user != null)
                {
                    user.Verified = true;
                    user.Email = UnverifiedEmail.Email;
                    DB.UnverifiedEmails.Delete(UnverifiedEmail.Id);
                    DB.Users.Update(user);
                    return Redirect("/Accounts/Login?message=Votre adresse de courriel a été modifiée avec succès!");
                }
            }
            return Redirect("/Accounts/Login?message=Erreur de modification de courriel!&success=false");
        }
        [UserAccess]
        public ActionResult EditProfil()
        {
            User connectedUser = (User)Session["ConnectedUser"];
            if (connectedUser != null)
            {
                connectedUser.ConfirmEmail = connectedUser.Email;
                Session["CurrentEditingUserPassword"] = DateTime.Now.Ticks.ToString();
                return View(connectedUser);
            }
            return RedirectToAction("Login", "Accounts");
        }

        [UserAccess]
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult EditProfil(User user)
        {
            bool newEmail = false;
            User connectedUser = (User)Session["ConnectedUser"];
            user.Id = connectedUser.Id;
            user.Blocked = connectedUser.Blocked;
            user.Admin = connectedUser.Admin;
            user.Online = connectedUser.Online;
            user.Verified = connectedUser.Verified;
            // check password has been changed 
            if (user.Password == (string)Session["CurrentEditingUserPassword"])
                user.Password = connectedUser.Password; // no password change
            // check if Email has been changed
            if (user.Email != connectedUser.Email)
            {
                newEmail = true;
                AccountsEmailing.SendEmailChangedVerification(Url.Action("VerifyNewEmail", "Accounts", null, Request.Url.Scheme), user);
                user.Email = connectedUser.Email; // new Email will commited on verification
            }
            if (DB.Users.Update(user))
            {
                Session["ConnectedUser"] = DB.Users.Get(user.Id).Copy();
            }
            if (newEmail)
                return Redirect("/Accounts/Login?message=Un courriel de vérification d'adresse de courriel vous a été envoyé!");
            else
                return RedirectToAction("Index", "Students");
        }
        [UserAccess]
        public ActionResult DeleteProfil()
        {
            User connectedUser = (User)Session["ConnectedUser"];
            DB.Users.Delete(connectedUser.Id);
            return RedirectToAction("Login?message=Votre compte a été effacé avec succès!");
        }

        [AdminAccess]
        public ActionResult GetUsers(bool forceRefresh = false)
        {
            if (forceRefresh || DB.Users.HasChanged)
            {
                User connectedUser = (User)Session["ConnectedUser"];
                return PartialView(DB.Users.ToList().Where(u => u.Id != connectedUser.Id).OrderBy(u => u.Name).ToList());
            }
            return null;
        }

        [AdminAccess]
        public ActionResult ManageUsers()
        {
            return View();
        }
        [AdminAccess]
        public ActionResult TogglePromoteUser(int id)
        {
            User user = DB.Users.Get(id);
            if (user != null)
            {
                user.Admin = !user.Admin;
                DB.Users.Update(user);
            }
            return null;
        }
        [AdminAccess]
        public ActionResult ToggleBlockUser(int id)
        {
            User user = DB.Users.Get(id);
            if (user != null)
            {
                user.Blocked = !user.Blocked;
                user.Online = false;
                DB.Users.Update(user);
            }
            return null;
        }
        [AdminAccess]
        public ActionResult ForceVerifyUser(int id)
        {
            User user = DB.Users.Get(id);
            if (user != null)
            {
                user.Verified = true;
                DB.Users.Update(user);
            }
            return null;
        }
        [AdminAccess]
        public ActionResult DeleteUser(int id)
        {
            DB.Users.Delete(id);
            return null;
        }
    }
}