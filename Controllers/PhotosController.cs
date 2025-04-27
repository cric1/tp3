using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using PhotoManager.Models;
using PhotosManager.Models;
using static PhotosManager.Controllers.AccessControl;

namespace PhotoManager.Controllers
{
    [UserAccess]
    public class PhotosController : Controller
    {
        // GET: Photos
        public ActionResult Index()
        {
            var photos = DB.Photos.ToList();
            return View(photos);
        }

        // GET: Photos/Details/5
        public ActionResult Details(int id)
        {
            var photo = DB.Photos.Get(id);
            if (photo == null) return HttpNotFound();
            return View(photo);
        }

        // GET: Photos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Photos/Create
        [HttpPost]
        public ActionResult Create(Photo photo)
        {
            if (!ModelState.IsValid)
                return View(photo);

            photo.Date = DateTime.Now;
            photo.OwnerId = ((User)Session["ConnectedUser"]).Id;
            DB.Photos.Add(photo);
            return RedirectToAction("Index");
        }

        // GET: Photos/Edit/5
        public ActionResult Edit(int id)
        {
            var photo = DB.Photos.Get(id);
            if (photo == null)
                return HttpNotFound();

            var currentUser = (User)Session["ConnectedUser"];
            if (photo.OwnerId != currentUser.Id && !currentUser.IsAdmin)
                return new HttpStatusCodeResult(403);

            return View(photo);
        }

        // POST: Photos/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Photo edited)
        {
            if (!ModelState.IsValid)
                return View(edited);

            var photo = DB.Photos.Get(id);
            if (photo == null)
                return HttpNotFound();

            var currentUser = (User)Session["ConnectedUser"];
            if (photo.OwnerId != currentUser.Id && !currentUser.IsAdmin)
                return new HttpStatusCodeResult(403);

            photo.Title = edited.Title;
            photo.Description = edited.Description;
            photo.Path = edited.Path;
            photo.Shared = edited.Shared;
            DB.Photos.Update(photo);

            return RedirectToAction("Details", new { id });
        }

        // GET: Photos/Delete/5
        public ActionResult Delete(int id)
        {
            var photo = DB.Photos.Get(id);
            if (photo == null)
                return HttpNotFound();

            var currentUser = (User)Session["ConnectedUser"];
            if (photo.OwnerId != currentUser.Id && !currentUser.IsAdmin)
                return new HttpStatusCodeResult(403);

            DB.Photos.Delete(id);
            return RedirectToAction("Index");
        }

        //jsp comment faire
        //[HttpGet]
        //public JsonResult HasLiked(int id)
        //{
        //    var user = Session["ConnectedUser"] as User;
        //    if (user == null)
        //        return Json(false, JsonRequestBehavior.AllowGet);

        //    var photo = DB.Photos.Get(id);
        //    bool hasLiked = photo.Likes.Any(l => l.UserId == user.Id);
        //    return Json(hasLiked, JsonRequestBehavior.AllowGet);
        //}

    }
}
