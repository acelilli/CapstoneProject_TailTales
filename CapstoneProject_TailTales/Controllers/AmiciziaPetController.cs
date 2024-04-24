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
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int idPetRichiedente, int idPetRichiesto)
        {
            // Verifica se gli ID dei pet sono validi
            var petRichiedente = db.Pet.Find(idPetRichiedente);
            var petRichiesto = db.Pet.Find(idPetRichiesto);

            // Verifica se i pet esistono nel database
            if (petRichiedente == null || petRichiesto == null || petRichiedente == petRichiesto)
            {
                return HttpNotFound();
            }

            // Cerca la richiesta di amicizia tra pet nel database dove ci sono i valori di IdPetRichiedente, richiesto e lo stato è "In Attesa..."
            var richiesta = db.RichiestaContatto.FirstOrDefault(r => r.IdPetRichiedente == idPetRichiedente && r.IdPetRichiesto == idPetRichiesto && r.StatoRichiesta == "In Attesa...");
            if (richiesta != null)
            {
                // Aggiorna lo stato della richiesta
                richiesta.StatoRichiesta = "Completata!";

                // Crea l'amicizia tra i pets
                var amiciziaPets = new AmiciziaPet
                {
                    IdPetRichiedente = richiesta.IdPetRichiedente.Value,
                    IdPetRichiesto = richiesta.IdPetRichiesto.Value,
                };

                db.AmiciziaPet.Add(amiciziaPets);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            // Se la richiesta non è stata trovata o non è in attesa, restituisci una view vuota o gestisci l'errore come meglio ritieni opportuno
            return HttpNotFound();
        }


        // GET: AmiciziaPet/Edit/5
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
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
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
