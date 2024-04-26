using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CapstoneProject_TailTales.Models;

namespace CapstoneProject_TailTales.Controllers
{
    public class MemoController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: Memo
        public ActionResult Index()
        {
            var memo = db.Memo.Include(m => m.Utenti);
            return View(memo.ToList());
        }

        // GET: Memo/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Memo memo = db.Memo.Find(id);
            if (memo == null)
            {
                return HttpNotFound();
            }
            return View(memo);
        }

        // GET: Memo/Create
        public ActionResult Create()
        {
            ViewBag.IdUtente_FK = new SelectList(db.Utenti, "IdUtente", "Username");
            return View();
        }

        // POST: Memo/Create
        // Azione accessibile tramite Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdMemo,IdUtente_FK,DataMemo,Descrizione,Completato")] Memo memo)
        {
            if (ModelState.IsValid)
            {
                db.Memo.Add(memo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdUtente_FK = new SelectList(db.Utenti, "IdUtente", "Username", memo.IdUtente_FK);
            return View(memo);
        }

        // GET: Memo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Memo memo = db.Memo.Find(id);
            if (memo == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdUtente_FK = new SelectList(db.Utenti, "IdUtente", "Username", memo.IdUtente_FK);
            return View(memo);
        }

        // POST: Memo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdMemo,IdUtente_FK,DataMemo,Descrizione,Completato")] Memo memo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(memo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdUtente_FK = new SelectList(db.Utenti, "IdUtente", "Username", memo.IdUtente_FK);
            return View(memo);
        }

        // GET: Memo/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Memo memo = db.Memo.Find(id);
            if (memo == null)
            {
                return HttpNotFound();
            }
            return View(memo);
        }

        // POST: Memo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Memo memo = db.Memo.Find(id);
            db.Memo.Remove(memo);
            db.SaveChanges();
            return RedirectToAction("Index");
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
