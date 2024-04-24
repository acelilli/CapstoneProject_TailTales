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
        public ActionResult Create()
        {
            int userId;
            string userIdCookieValue = Request.Cookies["IDUserCookie"]?.Value;
            bool isUserIdValid = int.TryParse(userIdCookieValue, out userId);

            if (!isUserIdValid)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            // Ottieni l'utente loggato
            var loggedInUser = db.Utenti.FirstOrDefault(u => u.IdUtente == userId);

            // Ottieni una lista di utenti escludendo l'utente loggato
            var otherUsers = db.Utenti.Where(u => u.IdUtente != userId).ToList();

            ViewBag.IdUtenteRichiedente = loggedInUser.IdUtente;
            ViewBag.IdUtenteRichiesto = new SelectList(otherUsers, "IdUtente", "Username");

            return View();
        }

        // POST: AmiciziaUtenti/Create
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int idUtenteRichiedente)
        {
            // Verifica se l'ID utente richiedente è valido
            var utenteRichiedente = db.Utenti.Find(idUtenteRichiedente);
            if (utenteRichiedente == null)
            {
                // Ritorna un errore se l'utente richiedente non esiste
                return HttpNotFound();
            }

            // Ottieni l'ID dell'utente loggato dal cookie
            int userId;
            string userIdCookieValue = Request.Cookies["IDUserCookie"]?.Value;
            bool isUserIdValid = int.TryParse(userIdCookieValue, out userId);

            // Se l'ID utente loggato non è valido, restituisci un errore di autorizzazione
            if (!isUserIdValid)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            if (userId == idUtenteRichiedente) //  controllo se sta prendendo lo stesso id
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Cerca la richiesta di contatto esistente
            var richiesta = db.RichiestaContatto.FirstOrDefault(r => r.IdUtenteRichiedente == idUtenteRichiedente && r.IdUtenteRichiesto == userId && r.StatoRichiesta == "In attesa...");
            if (richiesta != null)
            {
                // Aggiorna lo stato della richiesta
                richiesta.StatoRichiesta = "Completata!";

                // Crea l'amicizia tra gli utenti
                var amicizia = new AmiciziaUtenti
                {
                    IdUtenteRichiedente = richiesta.IdUtenteRichiedente,
                    IdUtenteRichiesto = richiesta.IdUtenteRichiesto,
                };

                // Aggiungi l'amicizia al contesto e salva le modifiche nel database
                db.AmiciziaUtenti.Add(amicizia);
                db.SaveChanges();
            }

            // Reindirizza alla vista di conferma o ad un'altra pagina
            return RedirectToAction("Index", "Home"); 
        }




        // GET: AmiciziaUtenti/Edit/5
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
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
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
