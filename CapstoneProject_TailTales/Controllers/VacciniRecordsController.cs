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
    public class VacciniRecordsController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: VacciniRecords
        [Authorize(Roles = "admin")]
        public ActionResult Index()
        {
            var vacciniRecords = db.VacciniRecords.Include(v => v.Libretto).Include(v => v.Utenti);
            return View(vacciniRecords.ToList());
        }

        // GET: VacciniRecords/Details/5
        [Authorize(Roles = "admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VacciniRecords vacciniRecords = db.VacciniRecords.Find(id);
            if (vacciniRecords == null)
            {
                return HttpNotFound();
            }
            return View(vacciniRecords);
        }

        // GET: VacciniRecords/Create
        // Prende come parametro l'id del libretto al quale appartiene il record
        //  Recupera la lista dei veterinari e la passa alla vista tramite ViewData
        public ActionResult Create(int idlibretto_fk)
        {
            var nuovorecord = new VacciniRecords
            {
                IdLibretto_FK = idlibretto_fk
            };

            var veterinari = db.Utenti.Where(u => u.IdRuolo_FK == 3)
                               .Select(u => new SelectListItem
                               {
                                   Value = u.IdUtente.ToString(),
                                   Text = (!string.IsNullOrEmpty(u.Nome) && !string.IsNullOrEmpty(u.Cognome))
                                               ? u.Nome + " " + u.Cognome
                                               : u.Username
                               })
                               .ToList();
            ViewBag.IdLibretto_FK = new SelectList(db.Libretto, "IdLibretto", "NumMicrochip");
            ViewData["VeterinariList"] = new SelectList(veterinari, "Value", "Text");


            return View(nuovorecord);
        }

        // POST: VacciniRecords/Create
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdRecordVaccini,IdLibretto_FK,VaccinoRabbia,DataPrevista,DataEffettuato,VaccinoNLotto,Scadenza,Richiamo,IdUtente_FK,Veterinario,Prezzo,PuntoInoculo")] VacciniRecords vacciniRecords)
        {
            if (ModelState.IsValid)
            {
                db.VacciniRecords.Add(vacciniRecords);
                db.SaveChanges();
                return RedirectToAction("Details", "Libretto", new { id = vacciniRecords.IdLibretto_FK });
            }

            ViewBag.IdLibretto_FK = new SelectList(db.Libretto, "IdLibretto", "NumMicrochip", vacciniRecords.IdLibretto_FK);
            ViewBag.IdUtente_FK = new SelectList(db.Utenti, "IdUtente", "Username", vacciniRecords.IdUtente_FK);
            return View(vacciniRecords);
        }

        // GET: VacciniRecords/Edit/5
        // Prende come parametro l'ID del record e restituisce la vista
        // Recupera la lista dei veterinari e la passa alla vista tramite ViewData
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VacciniRecords vacciniRecords = db.VacciniRecords.Find(id);
            if (vacciniRecords == null)
            {
                return HttpNotFound();
            }
            // Carica la lista dei veterinari per il dropdown
            var veterinari = db.Utenti.Where(u => u.IdRuolo_FK == 3)
                                       .Select(u => new SelectListItem
                                       {
                                           Value = u.IdUtente.ToString(),
                                           Text = (!string.IsNullOrEmpty(u.Nome) && !string.IsNullOrEmpty(u.Cognome))
                                                       ? u.Nome + " " + u.Cognome
                                                       : u.Username
                                       })
                                       .ToList();
            ViewData["VeterinariList"] = new SelectList(veterinari, "Value", "Text");
            return View(vacciniRecords);
        }

        // POST: VacciniRecords/Edit/5
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdRecordVaccini,IdLibretto_FK,VaccinoRabbia,DataPrevista,DataEffettuato,VaccinoNLotto,Scadenza,Richiamo,IdUtente_FK,Veterinario,Prezzo,PuntoInoculo")] VacciniRecords vacciniRecords)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vacciniRecords).State = EntityState.Modified;
                db.SaveChanges();
                RedirectToAction("Details", "Libretto", new { id = vacciniRecords.IdLibretto_FK });
            }
            // Ricarica la lista dei veterinari nel caso di errore di validazione
            var veterinari = db.Utenti.Where(u => u.IdRuolo_FK == 3)
                                       .Select(u => new SelectListItem
                                       {
                                           Value = u.IdUtente.ToString(),
                                           Text = (!string.IsNullOrEmpty(u.Nome) && !string.IsNullOrEmpty(u.Cognome))
                                                       ? u.Nome + " " + u.Cognome
                                                       : u.Username
                                       })
                                       .ToList();
            ViewData["VeterinariList"] = new SelectList(veterinari, "Value", "Text");

            return View(vacciniRecords);
        }

        // GET: VacciniRecords/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VacciniRecords vacciniRecords = db.VacciniRecords.Find(id);
            if (vacciniRecords == null)
            {
                return HttpNotFound();
            }
            return View(vacciniRecords);
        }

        // POST: VacciniRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            VacciniRecords vacciniRecords = db.VacciniRecords.Find(id);
            db.VacciniRecords.Remove(vacciniRecords);
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
