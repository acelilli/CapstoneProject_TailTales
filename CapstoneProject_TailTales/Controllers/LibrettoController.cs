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
    public class LibrettoController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: Libretto
        [Authorize]
        public ActionResult Index()
        {
            var libretto = db.Libretto.Include(l => l.Pet);
            return View(libretto.ToList());
        }

        // GET: Libretto/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Libretto libretto = db.Libretto.Find(id);
            if (libretto == null)
            {
                return HttpNotFound();
            }
            return View(libretto);
        }

        // GET: Libretto/Create
        // 1. Se l'utente è admin recupera tutti i Pets dal db (nome degli animali + proprietari) e restituisce la lista
        [Authorize]
        public ActionResult Create(int idPet_FK)
        {
            int userId;
            string userIdCookieValue = Request.Cookies["IDUserCookie"]?.Value;
            bool isUserIdValid = int.TryParse(userIdCookieValue, out userId);

            if (!isUserIdValid)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            var nuovoLibretto = new Libretto
            {
                IdPet_FK = idPet_FK
            };

            return View();
        }

        // POST: Libretto/Create
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "IdLibretto,IdPet_FK,NumMicrochip,Proprietario,Indirizzo,Provenienza,NumEnci,Sterilizzato")] Libretto libretto)
        {
            if (ModelState.IsValid)
            {
                db.Libretto.Add(libretto);
                db.SaveChanges();
                return RedirectToAction("Index", "Pet");
            }
            else
            {
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        ViewBag.ErrorMessage += error.ErrorMessage + "<br/>";
                    }
                }
            }

            ViewBag.IdPet_FK = new SelectList(db.Pet, "IdPet", "Tipo", libretto.IdPet_FK);
            return View(libretto);
        }

        // GET: Libretto/Edit/5
        // 1. Prende l'id del pet come parametro
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Libretto libretto = db.Libretto.Find(id);
            if (libretto == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdPet_FK = new SelectList(db.Pet, "IdPet", "Nome", libretto.IdPet_FK);
            return View(libretto);
        }

        // POST: Libretto/Edit/5
        // 1. Trova il record corrispondente nel database
        // 2. Si assicura che il record esista nel database e che IdPet_FK non sia stato modificato
        // 3. Salva i valori modificati nell'oggetto esistente
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "IdLibretto,IdPet_FK,NumMicrochip,Proprietario,Indirizzo,Provenienza,NumEnci,Sterilizzato")] Libretto libretto)
        {
            if (ModelState.IsValid)
            {
                var existingLibretto = db.Libretto.Find(libretto.IdLibretto);

                if (existingLibretto != null && existingLibretto.IdPet_FK == libretto.IdPet_FK)
                {
                    db.Entry(existingLibretto).CurrentValues.SetValues(libretto);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            ViewBag.IdPet_FK = new SelectList(db.Pet, "IdPet", "Nome", libretto.IdPet_FK);
            return View(libretto);
        }

        // GET: Libretto/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Libretto libretto = db.Libretto.Find(id);
            if (libretto == null)
            {
                return HttpNotFound();
            }
            return View(libretto);
        }

        // POST: Libretto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            Libretto libretto = db.Libretto.Find(id);
            db.Libretto.Remove(libretto);
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
