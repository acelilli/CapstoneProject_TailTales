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
    public class AmiciziaUtentiController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: AmiciziaUtenti
        public ActionResult Index()
        {
            var amiciziaUtenti = db.AmiciziaUtenti.Include(a => a.Utenti).Include(a => a.Utenti1);
            return View(amiciziaUtenti.ToList());
        }

        // GET: AmiciziaUtenti/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AmiciziaUtenti amiciziaUtenti = db.AmiciziaUtenti.Find(id);
            if (amiciziaUtenti == null)
            {
                return HttpNotFound();
            }
            return View(amiciziaUtenti);
        }

        // GET: AmiciziaUtenti/Create
        // 1. Otteniamo l'utente loggato tramite il cookie
        //    1.1 Se non è loggato restituisce Forbidden
        // 2. Crea una lista di utenti chre esclude l'utente loggato (l'utente non potrà essere amico di sè stesso)
        public ActionResult Create()
        {
            int userId;
            string userIdCookieValue = Request.Cookies["IDUserCookie"]?.Value;
            bool isUserIdValid = int.TryParse(userIdCookieValue, out userId);

            if (!isUserIdValid)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            var loggedInUser = db.Utenti.FirstOrDefault(u => u.IdUtente == userId);

            var otherUsers = db.Utenti.Where(u => u.IdUtente != userId).ToList();

            ViewBag.IdUtenteRichiedente = loggedInUser.IdUtente;
            ViewBag.IdUtenteRichiesto = new SelectList(otherUsers, "IdUtente", "Username");

            return View();
        }

        // POST: AmiciziaUtenti/Create
        // 1. Verifica che l'Id dell'utente Richiedente è valido (altrimenti torna NotFound)
        // 2. Ottiene l'id dell'utente dal Cookie (se non è velido restituisce Forbidden)
        // 3. L'utente dell'id loggato deve essere lo stesso dell'id utente richiedente, altrimenti restituisce BadRequest
        // 4. Cerca la richiesta di contatto che ha Id Richiesto e Id Richiedente corrispondente e stato "in attesa..."
        // 5. !! Modifica lo stato della richiesta di contatto in "Completata!"
        // 6. Crea un nuovo record AmiciziaUtenti nel database prendendo gli Id dai campi della RichiestaContatto corrispondente
        // 7. Reindirizza alla Home
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int idUtenteRichiedente)
        {
            var utenteRichiedente = db.Utenti.Find(idUtenteRichiedente);
            if (utenteRichiedente == null)
            {
                return HttpNotFound();
            }

            int userId;
            string userIdCookieValue = Request.Cookies["IDUserCookie"]?.Value;
            bool isUserIdValid = int.TryParse(userIdCookieValue, out userId);

            if (!isUserIdValid)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            if (userId == idUtenteRichiedente)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var richiesta = db.RichiestaContatto.FirstOrDefault(r => r.IdUtenteRichiedente == idUtenteRichiedente && r.IdUtenteRichiesto == userId && r.StatoRichiesta == "In attesa...");
            if (richiesta != null)
            {
                richiesta.StatoRichiesta = "Completata!";

                var amicizia = new AmiciziaUtenti
                {
                    IdUtenteRichiedente = richiesta.IdUtenteRichiedente,
                    IdUtenteRichiesto = richiesta.IdUtenteRichiesto,
                };

                db.AmiciziaUtenti.Add(amicizia);
                db.SaveChanges();
            }

            return RedirectToAction("Index", "Home"); 
        }




        // GET: AmiciziaUtenti/Edit/5
        [Authorize(Roles ="Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AmiciziaUtenti amiciziaUtenti = db.AmiciziaUtenti.Find(id);
            if (amiciziaUtenti == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdUtenteRichiedente = new SelectList(db.Utenti, "IdUtente", "Username", amiciziaUtenti.IdUtenteRichiedente);
            ViewBag.IdUtenteRichiesto = new SelectList(db.Utenti, "IdUtente", "Username", amiciziaUtenti.IdUtenteRichiesto);
            return View(amiciziaUtenti);
        }

        // POST: AmiciziaUtenti/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "IdAmicizia,IdUtenteRichiedente,IdUtenteRichiesto")] AmiciziaUtenti amiciziaUtenti)
        {
            if (ModelState.IsValid)
            {
                db.Entry(amiciziaUtenti).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdUtenteRichiedente = new SelectList(db.Utenti, "IdUtente", "Username", amiciziaUtenti.IdUtenteRichiedente);
            ViewBag.IdUtenteRichiesto = new SelectList(db.Utenti, "IdUtente", "Username", amiciziaUtenti.IdUtenteRichiesto);
            return View(amiciziaUtenti);
        }

        // GET: AmiciziaUtenti/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AmiciziaUtenti amiciziaUtenti = db.AmiciziaUtenti.Find(id);
            if (amiciziaUtenti == null)
            {
                return HttpNotFound();
            }
            return View(amiciziaUtenti);
        }

        // POST: AmiciziaUtenti/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AmiciziaUtenti amiciziaUtenti = db.AmiciziaUtenti.Find(id);
            db.AmiciziaUtenti.Remove(amiciziaUtenti);
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
