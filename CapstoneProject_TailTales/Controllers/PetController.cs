using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Drawing;
using CapstoneProject_TailTales.Models;
using System.Web.ModelBinding;
using System.EnterpriseServices;

namespace CapstoneProject_TailTales.Controllers
{
    public class PetController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: Pet
        // 1. Verifica se l'utente è autenticato e ne recupera l'ID dal cookie
        //    Se non è autenticato o non ha un id valido allora ritorna 403 Forbidden
        // 2. Controlla se l'utente è un amministratore 
        //    Se l'utente è amministratore, recupera tutti i pet,
        //    Altrimenti li filtra per l'ID utente corrente
        [Authorize]
        public ActionResult Index()
        {
            int userId;
            string userIdCookieValue = Request.Cookies["IDUserCookie"]?.Value;
            bool isUserIdValid = int.TryParse(userIdCookieValue, out userId);
            if (!isUserIdValid)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            bool isAdmin = User.IsInRole("admin");

            List<Pet> pets; // o qualsiasi altra logica per ottenere i pet
            var libretti = db.Libretto.ToList(); // ottieni tutti i libretti
            ViewBag.Libretti = libretti; // aggiungi i libretti alla ViewBag

            if (isAdmin)
            {
                pets = db.Pet.ToList();
            }
            else
            {
                pets = db.Pet.Include("Utenti").Where(p => p.IdUtente_FK == userId).ToList();
            }
            // Estrai gli ID dei pets dalla lista
            var petIds = pets.Select(p => p.IdPet).ToList();

            // Cerca gli ID dei pets nella tabella AmiciziaPet
            var amiciziaPet = db.AmiciziaPet.Where(a => petIds.Contains(a.IdPetRichiedente) || petIds.Contains(a.IdPetRichiesto)).ToList();

            // Estrai gli ID dei pet richiedenti e richiesti
            var idPetRichiedenti = amiciziaPet.Select(a => a.IdPetRichiedente).ToList();
            var idPetRichiesti = amiciziaPet.Select(a => a.IdPetRichiesto).ToList();

            // Ottieni i dati relativi ai pet richiedenti e richiesti dalla tabella Utenti
            var petRichiedenti = db.Pet.Where(p => idPetRichiedenti.Contains(p.IdPet)).ToList();
            var petRichiesti = db.Pet.Where(p => idPetRichiesti.Contains(p.IdPet)).ToList();

            ViewBag.AmiciPet = amiciziaPet;
            ViewBag.AmiciPetRichiedenti = petRichiedenti;
            ViewBag.AmiciPetRichiesti = petRichiesti;

            return View(pets);
        }

        
        // GET: Pet/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pet pet = db.Pet.Find(id);
            if (pet == null)
            {
                return HttpNotFound();
            }
            return View(pet);
        }

        // GET: Pet/Create
        // 1. Recupera le liste di Specie e razze
        [Authorize]
        public ActionResult Create()
        {
            var specieERazze = SpecieRazze.GetSpecieRazze();
            List<string> specie = specieERazze.Item1;
            Dictionary<string, List<string>> razzePerSpecie = specieERazze.Item2;

            // Passa le specie e le razze alla vista
            ViewData["Specie"] = specie;
            ViewData["RazzePerSpecie"] = razzePerSpecie;
            ViewBag.IdUtente_FK = new SelectList(db.Utenti, "IdUtente", "Username");
            return View();
        }

        // POST: Pet/Create
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        // 1. Riceve come parametri le proprietà da bindare, l'oggetto Pet e l'immagine profilo
        // 2. Verifica che l'utente sia autenticato e ne recupera l'ID dal cookie
        //    Se non è autenticato o l'id non è valido torna 403 Forbidden
        // 3. Controlla che l'utente sia amministratore
        //    Se l'utente è un amministratore il campo IdUtente_FK rimane una lista dinamica
        //    Se l'utente non è un amministratore imposta l'ID dell'utente estratto dal cookie come IdUtente_FK del pet
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "IdPet,IdUtente_FK,Tipo,Razza,Nome,Sesso,ImgProfilo,DataNascita")] Pet pet, HttpPostedFileBase ImgProfilo)
        {
            int userId;
            string userIdCookieValue = Request.Cookies["IDUserCookie"]?.Value;
            bool isUserIdValid = int.TryParse(userIdCookieValue, out userId);

            if (!isUserIdValid)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            bool isAdmin = User.IsInRole("admin");

            if (!isAdmin)
            {
                pet.IdUtente_FK = userId;
            }
            if (ImgProfilo != null && ImgProfilo.ContentLength > 0)
            {
                string nomeFile = Path.GetFileName(ImgProfilo.FileName);
                string pathToSave = Path.Combine(Server.MapPath("~/Content/UploadedPics/"), nomeFile);
                ImgProfilo.SaveAs(pathToSave);
                pet.ImgProfilo = "/Content/UploadedPics/" + nomeFile;
            }
            if (ModelState.IsValid)
            {
                db.Pet.Add(pet);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdUtente_FK = new SelectList(db.Utenti, "IdUtente", "Username", pet.IdUtente_FK);
            ViewBag.Generi = Pet.Generi;
            return View(pet);
        }


        // GET: Pet/Edit/5
        // 1. Recupera l'id utente dal cookie
        //    Se l'id è null restutuisce Bad Request
        // 2. Cerca il pet e verifica se esiste.
        //    Confronta l'ID del cookie con l'idUtente_FK del pet, se coincidono allora l'utente è autorizzato a modificarlo
        // 3. Se l'utente è amministratore però può vedere l'elenco degli utenti per selezionarlo manualmente
        //    Per gli utenti che NON sono amministratori il campo IdUtente_FK viene nascosto
        // 4.  Recupera le specie e le razze per popolare le dropdown nella vista
        [HttpGet]
        [Authorize]
        public ActionResult Edit(int? id)
        {
            int userId;
            string userIdCookieValue = Request.Cookies["IDUserCookie"]?.Value;
            bool isUserIdValid = int.TryParse(userIdCookieValue, out userId);

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Pet pet = db.Pet.Find(id);

            if (pet == null || (!User.IsInRole("admin") && pet.IdUtente_FK != userId))
            {
                return HttpNotFound();
            }

            if (User.IsInRole("admin"))
            {
                ViewBag.IdUtente_FK = new SelectList(db.Utenti, "IdUtente", "Username", pet.IdUtente_FK);
            }
            else
            {
                ViewBag.IdUtente_FK = pet.IdUtente_FK;
            }

            var specieERazze = SpecieRazze.GetSpecieRazze();
            List<string> specie = specieERazze.Item1;
            Dictionary<string, List<string>> razzePerSpecie = specieERazze.Item2;

            ViewData["Specie"] = specie;
            ViewData["RazzePerSpecie"] = razzePerSpecie;

            return View(pet);
        }

        // POST: Pet/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "IdPet, IdUtente_FK, Tipo,Razza,Nome,Sesso,ImgProfilo,DataNascita")] Pet editedPet, HttpPostedFileBase ImgProfilo)
        {
            // Verifica se l'utente è autenticato e recupera l'ID dell'utente dal cookie
            int userId;
            string userIdCookieValue = Request.Cookies["IDUserCookie"]?.Value;
            bool isUserIdValid = int.TryParse(userIdCookieValue, out userId);

            if (!isUserIdValid)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            if (ModelState.IsValid)
            {
                // Aggiorna l'immagine se è stata fornita
                if (ImgProfilo != null && ImgProfilo.ContentLength > 0)
                {
                    string nomeFile = Path.GetFileName(ImgProfilo.FileName);
                    string pathToSave = Path.Combine(Server.MapPath("/Content/UploadedPics/"), nomeFile);
                    ImgProfilo.SaveAs(pathToSave);
                    editedPet.ImgProfilo = "/Content/UploadedPics/" + nomeFile;
                }

                db.Entry(editedPet).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdUtente_FK = editedPet.IdUtente_FK;
            return View(editedPet);
        }


        // GET: Pet/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pet pet = db.Pet.Find(id);
            if (pet == null)
            {
                return HttpNotFound();
            }
            return View(pet);
        }

        // POST: Pet/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            Pet pet = db.Pet.Find(id);
            db.Pet.Remove(pet);
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
