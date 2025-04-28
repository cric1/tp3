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
        // POST: Photos/Like/5
       
        [HttpPost]
        public JsonResult Like(int id)
        {
            var user = (User)Session["ConnectedUser"];
            var photo = DB.Photos.Get(id);
            if (photo == null)
                return Json(new { success = false, message = "Photo not found." });

            var existing = DB.Likes.ToList().FirstOrDefault(l => l.PhotoId == id && l.UserId == user.Id);

            if (existing == null)
            {
                DB.Likes.Add(new Like { PhotoId = id, UserId = user.Id });
            }
            else
            {
                DB.Likes.Delete(existing.Id);
            }

            var count = DB.Likes.ToList().Count(l => l.PhotoId == id);

            return Json(new { success = true, count });
        }


    }
}
