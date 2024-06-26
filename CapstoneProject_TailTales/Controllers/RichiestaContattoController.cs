﻿using System;
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
    public class RichiestaContattoController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: RichiestaContatto
        // 1. Ottiene l'id dell'utente loggato dal cookie
        //    1.1 Se l'id utente non è valido, restituisci Forbidden
        // 2. Otteniamo delle liste che restituiscono i dati dell'utente loggato:
        //    2.1 Una lista di utenti escludendo l'utente loggato
        //    2.2 Una lista di pet escludento quelli dell'utente loggato 
        //    2.3 Una lista di richieste di contatto per l'utente loggato (ossia se l'ID dell'utente loggato si trova in IdUtenteRichiesto o IdUtenteRichiedente)
        //    2.4 Una lista dei pet dell'utente attualmente loggato
        // 3. A partire dal ViewModel FriendshipViewModel, ne valorizza i valori con le liste create nel punto 2.
        // 4. Restituisce la vista utilizzando il ViewModel valorizzato come parametro
        public ActionResult Index()
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
            var otherPets = db.Pet.Where(p => p.IdUtente_FK != userId).ToList();
            var pendingRequests = db.RichiestaContatto
                                     .Where(r => r.IdUtenteRichiesto == userId && r.StatoRichiesta == "In attesa...")
                                     .ToList();
            var myPets = db.Pet.Where(u => u.IdUtente_FK == userId).ToList();

            var viewModel = new FriendshipViewModel
            {
                OtherUsers = otherUsers,
                OtherPets = otherPets,
                LoggedInUserName = loggedInUser.Username,
                MyPets = myPets,
                PendingRequests = pendingRequests
            };

            return View(viewModel);
        }

        public class FriendshipViewModel
        {
            public IEnumerable<Utenti> OtherUsers { get; set; }
            public IEnumerable<Pet> OtherPets { get; set; }
            public string LoggedInUserName { get; set; }
            public IEnumerable<Pet> MyPets { get; set; } 
            public IEnumerable<RichiestaContatto> PendingRequests { get; set; }
        }

        // GET: RichiestaContatto/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RichiestaContatto richiestaContatto = db.RichiestaContatto.Find(id);
            if (richiestaContatto == null)
            {
                return HttpNotFound();
            }
            return View(richiestaContatto);
        }

        // GET: RichiestaContatto/Create
        // Azione accessibile da pulsante nella pagina EXPLORE
        // 1. Ottiene l'id dell'utente loggato
        // 2. Crea tre liste:
        //    2.1 La lista degli utenti escludendo l'utente loggato
        //    2.2 La lista dei pets escludendo quelli dell'utente loggato
        //    2.3 La lista dei pets dell'utente loggato
        // 2. Porta le liste nella pagina tramite ViewBag
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
            var otherPets = db.Pet.Where(u => u.IdUtente_FK != userId).ToList();
            var allmypets = db.Pet.Where(u => u.IdUtente_FK == userId).ToList();

            ViewBag.IdUtenteRichiedente = loggedInUser.IdUtente;
            ViewBag.IdUtenteRichiesto = new SelectList(otherUsers, "IdUtente", "Username");
            ViewBag.IdPetRichiesto = new SelectList(otherPets, "IdPet", "Nome");
            ViewBag.PetRichiedente = new SelectList(allmypets, "IdPet", "Nome");

            return View();
        }

        // POST: RichiestaContatto/Create
        
        // 1. Ottiene l'utente loggato
        // 2. Verifica se l'utente richieste esiste, altrimenti resituisce BadRequest
        // 3. Imposta il tipo di relazione in base alla presenza o meno dell'ID dei pet nella richiesta di contatto
        // 4. Crea una richiesta di contatto con stato "In Attesa..." e la aggiunge al db
        // 5. Reindirizza alla pagina index con un messaggio di successo.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int idUtenteRichiesto, int? idPetRichiedente, int? idPetRichiesto)
        {
            int userId;
            string userIdCookieValue = Request.Cookies["IDUserCookie"]?.Value;
            bool isUserIdValid = int.TryParse(userIdCookieValue, out userId);

            if (!isUserIdValid)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            var loggedInUser = db.Utenti.FirstOrDefault(u => u.IdUtente == userId);

            var richiesto = db.Utenti.FirstOrDefault(u => u.IdUtente == idUtenteRichiesto);
            if (richiesto == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Utente richiesto non valido");
            }

            string tipoRelazione = (idPetRichiedente == null && idPetRichiesto == null) ? "AmiciziaUtenti" : "AmiciziaPet";

            var richiestaContatto = new RichiestaContatto
            {
                IdUtenteRichiedente = userId,
                IdUtenteRichiesto = idUtenteRichiesto,
                IdPetRichiedente = idPetRichiedente,
                IdPetRichiesto = idPetRichiesto,
                DataRichiesta = DateTime.Now,
                TipoRelazione = tipoRelazione,
                StatoRichiesta = "In Attesa..."
            };
            // Aggiungi la richiesta di contatto al database
            db.RichiestaContatto.Add(richiestaContatto);
            db.SaveChanges();
            TempData["SuccessMessage"] = "La richiesta di contatto è stata inviata con successo!";

            // Reindirizza l'utente a una pagina di conferma o a una pagina principale
            return RedirectToAction("Index", "Home");
        }

        // base della action come reference
        /*
        public ActionResult Create([Bind(Include = "IdRichiesta,IdUtenteRichiedente,IdUtenteRichiesto,TipoRelazione,IdPetRichiedente,IdPetRichiesto,StatoRichiesta,DataRichiesta")] RichiestaContatto richiestaContatto)
        {
            if (ModelState.IsValid)
            {
                db.RichiestaContatto.Add(richiestaContatto);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdPetRichiedente = new SelectList(db.Pet, "IdPet", "Tipo", richiestaContatto.IdPetRichiedente);
            ViewBag.IdPetRichiesto = new SelectList(db.Pet, "IdPet", "Tipo", richiestaContatto.IdPetRichiesto);
            ViewBag.IdUtenteRichiedente = new SelectList(db.Utenti, "IdUtente", "Username", richiestaContatto.IdUtenteRichiedente);
            ViewBag.IdUtenteRichiesto = new SelectList(db.Utenti, "IdUtente", "Username", richiestaContatto.IdUtenteRichiesto);
            return View(richiestaContatto);
        }
        */

        // GET: RichiestaContatto/Edit/5
        [Authorize(Roles ="Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RichiestaContatto richiestaContatto = db.RichiestaContatto.Find(id);
            if (richiestaContatto == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdPetRichiedente = new SelectList(db.Pet, "IdPet", "Tipo", richiestaContatto.IdPetRichiedente);
            ViewBag.IdPetRichiesto = new SelectList(db.Pet, "IdPet", "Tipo", richiestaContatto.IdPetRichiesto);
            ViewBag.IdUtenteRichiedente = new SelectList(db.Utenti, "IdUtente", "Username", richiestaContatto.IdUtenteRichiedente);
            ViewBag.IdUtenteRichiesto = new SelectList(db.Utenti, "IdUtente", "Username", richiestaContatto.IdUtenteRichiesto);
            return View(richiestaContatto);
        }

        // POST: RichiestaContatto/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdRichiesta,IdUtenteRichiedente,IdUtenteRichiesto,TipoRelazione,IdPetRichiedente,IdPetRichiesto,StatoRichiesta,DataRichiesta")] RichiestaContatto richiestaContatto)
        {
            if (ModelState.IsValid)
            {
                db.Entry(richiestaContatto).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdPetRichiedente = new SelectList(db.Pet, "IdPet", "Tipo", richiestaContatto.IdPetRichiedente);
            ViewBag.IdPetRichiesto = new SelectList(db.Pet, "IdPet", "Tipo", richiestaContatto.IdPetRichiesto);
            ViewBag.IdUtenteRichiedente = new SelectList(db.Utenti, "IdUtente", "Username", richiestaContatto.IdUtenteRichiedente);
            ViewBag.IdUtenteRichiesto = new SelectList(db.Utenti, "IdUtente", "Username", richiestaContatto.IdUtenteRichiesto);
            return View(richiestaContatto);
        }

        // GET: RichiestaContatto/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RichiestaContatto richiestaContatto = db.RichiestaContatto.Find(id);
            if (richiestaContatto == null)
            {
                return HttpNotFound();
            }
            return View(richiestaContatto);
        }

        // POST: RichiestaContatto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            RichiestaContatto richiestaContatto = db.RichiestaContatto.Find(id);
            db.RichiestaContatto.Remove(richiestaContatto);
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

        //GetPendingRequests
        // Azione accessibile sulla navbar
        // 1. Ottiene l'utente loggato dal cookie
        // 2. Verifica se l'utente richieste esiste, altrimenti resituisce Forbidden
        // 3. Conta il num. di richieste in attesa (in cui l'id utente si trova in IdUtenteRichiesto) e restituisce il conteggio come stringa nel badge del Navbar
        [HttpGet]
        
        public ActionResult GetPendingRequests()
        {
            int userId;
            string userIdCookieValue = Request.Cookies["IDUserCookie"]?.Value;
            bool isUserIdValid = int.TryParse(userIdCookieValue, out userId);

            // Se l'ID utente non è valido, restituisci un errore di autorizzazione
            if (!isUserIdValid)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            using (var db = new ModelDbContext())
            {
                int pendingCount = db.RichiestaContatto
                                      .Count(r => r.IdUtenteRichiesto == userId && r.StatoRichiesta == "In attesa...");
                return Content(pendingCount.ToString());
            }
        }

        // Rifiuta richiesta contatto
        // 1. Trova la richiesta di contatto corrispondente all'id della richiesta
        // 2. Verifica se la richiesta esiste, se non esiste restituisce NotFound
        // 3. Cambia lo stato della richiesta in "Rifiutata"
        // 4. Salva il cambiamento e ritorna l'utente alla vista index dele richieste di contatto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RejectReq(int requestId)
        {
            var request = db.RichiestaContatto.FirstOrDefault(r => r.IdRichiesta == requestId);

            // Verifica se la richiesta esiste
            if (request == null)
            {
                return HttpNotFound();
            }

            request.StatoRichiesta = "Rifiutata";

            db.SaveChanges();

            return RedirectToAction("Index");
        }

    }
}
