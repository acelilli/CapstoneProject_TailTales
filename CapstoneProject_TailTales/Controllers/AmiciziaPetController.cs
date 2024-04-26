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
    public class AmiciziaPetController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: AmiciziaPet
        public ActionResult Index()
        {
            var amiciziaPet = db.AmiciziaPet.Include(a => a.Pet).Include(a => a.Pet1);
            return View(amiciziaPet.ToList());
        }

        // GET: AmiciziaPet/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AmiciziaPet amiciziaPet = db.AmiciziaPet.Find(id);
            if (amiciziaPet == null)
            {
                return HttpNotFound();
            }
            return View(amiciziaPet);
        }

        // GET: AmiciziaPet/Create
        // 1. Ottiene l'id dell'utente loggato dal cookie, se non è valido restituisce Forbidden
        // 2. A partire dall'id dell'utente loggato crea una lista dei suoi pets
        // 3. A partire dall'id dell'utente loggato crea una lista dei pet degli altri utenti escludendo quelli dell'utente loggato
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
            //Ottengo la lista dei miei pet
            var myPets = db.Pet.Where(u => u.IdUtente_FK == userId).ToList();
            //Ottengo la lista dei pet degli altri utenti escludendo quelli dell'utente loggato
            var otherPets = db.Pet.Where(p => p.IdUtente_FK != userId).ToList();

            ViewBag.IdPetRichiedente = new SelectList(otherPets, "IdPet", "Nome");
            ViewBag.IdPetRichiesto = new SelectList(myPets, "IdPet", "Nome");
            return View();
        }

        // POST: AmiciziaPet/Create
        // 1. Verifica che gli ID dei pet (Richiedente e Richiesto) forniti siano validi
        // 2. Verifica che i pet esistano nel db -> se non esistono restituisce NotFound
        // 3. Cerca la richiesta di amicizia tra i pet nel database in cui ci sono entrambi gli Id dei Pets e lo stato è "In Attesa..." -> Se non la trova restituisce NotFound
        // 4. Aggiorna lo stato della richiesta in "Compeletata"
        // 5. Crea l'amicizia tra i pet valorizzando i campi degli IdPet con quelli forniti dalla richiesta
        // 6. Reindirizza alla Home
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int idPetRichiedente, int idPetRichiesto)
        {
            var petRichiedente = db.Pet.Find(idPetRichiedente);
            var petRichiesto = db.Pet.Find(idPetRichiesto);

            if (petRichiedente == null || petRichiesto == null || petRichiedente == petRichiesto)
            {
                return HttpNotFound();
            }

            var richiesta = db.RichiestaContatto.FirstOrDefault(r => r.IdPetRichiedente == idPetRichiedente && r.IdPetRichiesto == idPetRichiesto && r.StatoRichiesta == "In Attesa...");
            if (richiesta != null)
            {
                richiesta.StatoRichiesta = "Completata!";

                var amiciziaPets = new AmiciziaPet
                {
                    IdPetRichiedente = richiesta.IdPetRichiedente.Value,
                    IdPetRichiesto = richiesta.IdPetRichiesto.Value,
                };

                db.AmiciziaPet.Add(amiciziaPets);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            return HttpNotFound();
        }


        // GET: AmiciziaPet/Edit/5
        [Authorize(Roles ="Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AmiciziaPet amiciziaPet = db.AmiciziaPet.Find(id);
            if (amiciziaPet == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdPetRichiedente = new SelectList(db.Pet, "IdPet", "Tipo", amiciziaPet.IdPetRichiedente);
            ViewBag.IdPetRichiesto = new SelectList(db.Pet, "IdPet", "Tipo", amiciziaPet.IdPetRichiesto);
            return View(amiciziaPet);
        }

        // POST: AmiciziaPet/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdAmiciziaPet,IdPetRichiedente,IdPetRichiesto")] AmiciziaPet amiciziaPet)
        {
            if (ModelState.IsValid)
            {
                db.Entry(amiciziaPet).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdPetRichiedente = new SelectList(db.Pet, "IdPet", "Tipo", amiciziaPet.IdPetRichiedente);
            ViewBag.IdPetRichiesto = new SelectList(db.Pet, "IdPet", "Tipo", amiciziaPet.IdPetRichiesto);
            return View(amiciziaPet);
        }

        // GET: AmiciziaPet/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AmiciziaPet amiciziaPet = db.AmiciziaPet.Find(id);
            if (amiciziaPet == null)
            {
                return HttpNotFound();
            }
            return View(amiciziaPet);
        }

        // POST: AmiciziaPet/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            AmiciziaPet amiciziaPet = db.AmiciziaPet.Find(id);
            db.AmiciziaPet.Remove(amiciziaPet);
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
