using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
            PhotoManager.Models.Photo photo = DB.Photos.Get(id);
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
            try
            {
                photo.Date = DateTime.Now;
                DB.Photos.Add(photo);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Photos/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Photos/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Photos/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Photos/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
