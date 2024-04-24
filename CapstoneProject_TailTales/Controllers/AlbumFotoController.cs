using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CapstoneProject_TailTales.Models;

namespace CapstoneProject_TailTales.Controllers
{
    public class AlbumFotoController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: AlbumFoto
        public ActionResult Index(int? id)
        {
            int userId;
            string userIdCookieValue = Request.Cookies["IDUserCookie"]?.Value;
            bool isUserIdValid = int.TryParse(userIdCookieValue, out userId);
            if (!isUserIdValid)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            var albumFoto = db.AlbumFoto.Where(a => a.IdUtente_FK == id).ToList();

            // Potresti voler passare l'ID dell'utente visitato alla vista, se necessario
            ViewBag.UserId = id;

            return View(albumFoto);
        }

        // GET: AlbumFoto/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AlbumFoto albumFoto = db.AlbumFoto.Find(id);
            if (albumFoto == null)
            {
                return HttpNotFound();
            }
            return View(albumFoto);
        }

        // GET: AlbumFoto/Create
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.IdUtente_FK = new SelectList(db.Utenti, "IdUtente", "Username");
            return View();
        }

        // POST: AlbumFoto/Create
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "IdAlbum,DataRecord,ImgUrl,Descrizione")] AlbumFoto albumFoto, HttpPostedFileBase ImgUrl)
        {
            int userId;
            string userIdCookieValue = Request.Cookies["IDUserCookie"]?.Value;
            bool isUserIdValid = int.TryParse(userIdCookieValue, out userId);
            if (!isUserIdValid)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            albumFoto.IdUtente_FK = userId; // valorizza l'IdUtente_FK con l'id user preso dal cookie
            albumFoto.DataRecord = DateTime.Now;

            if (ImgUrl != null && ImgUrl.ContentLength > 0)
            {
                string nomeFile = Path.GetFileName(ImgUrl.FileName);
                string pathToSave = Path.Combine(Server.MapPath("~/Content/UploadedPics/"), nomeFile);
                ImgUrl.SaveAs(pathToSave);
                albumFoto.ImgUrl = "/Content/UploadedPics/" + nomeFile;
            }

            if (ModelState.IsValid)
            {
                db.AlbumFoto.Add(albumFoto);
                db.SaveChanges();
                return RedirectToAction("Index", new { id = albumFoto.IdUtente_FK });
            }
            else
            {
                // Ottieni gli errori di validazione del modello
                ViewBag.ModelErrors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));

                // Reimposta IdUtente_FK e DataRecord nella ViewBag
                ViewBag.IdUtente_FK = albumFoto.IdUtente_FK;
                ViewBag.DataRecord = albumFoto.DataRecord;
            }
            return View(albumFoto);
        }

        // GET: AlbumFoto/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AlbumFoto albumFoto = db.AlbumFoto.Find(id);
            if (albumFoto == null)
            {
                return HttpNotFound();
            }
            return View(albumFoto);
        }

        // POST: AlbumFoto/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdAlbum,IdUtente_FK,DataRecord,ImgUrl,Descrizione")] AlbumFoto albumFoto, HttpPostedFileBase ImgUrl)
        {
            if (ModelState.IsValid)
            {
                if (ImgUrl != null && ImgUrl.ContentLength > 0)
                {
                    string nomeFile = Path.GetFileName(ImgUrl.FileName);
                    string pathToSave = Path.Combine(Server.MapPath("~/Content/UploadedPics/"), nomeFile);
                    ImgUrl.SaveAs(pathToSave);
                    albumFoto.ImgUrl = "/Content/UploadedPics/" + nomeFile;
                }

                db.Entry(albumFoto).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = albumFoto.IdUtente_FK });
            }
            return View(albumFoto);
        }

        // GET: AlbumFoto/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AlbumFoto albumFoto = db.AlbumFoto.Find(id);
            if (albumFoto == null)
            {
                return HttpNotFound();
            }
            return View(albumFoto);
        }

        // POST: AlbumFoto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AlbumFoto albumFoto = db.AlbumFoto.Find(id);
            db.AlbumFoto.Remove(albumFoto);
            db.SaveChanges();
            return RedirectToAction("Index", new { id = albumFoto.IdUtente_FK });
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
