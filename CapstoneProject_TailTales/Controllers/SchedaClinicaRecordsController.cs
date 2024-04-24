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
    public class SchedaClinicaRecordsController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: SchedaClinicaRecords
        public ActionResult Index()
        {
            var schedaClinicaRecords = db.SchedaClinicaRecords.Include(s => s.Libretto).Include(s => s.Utenti);
            return View(schedaClinicaRecords.ToList());
        }

        // GET: SchedaClinicaRecords/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SchedaClinicaRecords schedaClinicaRecords = db.SchedaClinicaRecords.Find(id);
            if (schedaClinicaRecords == null)
            {
                return HttpNotFound();
            }
            return View(schedaClinicaRecords);
        }

        // GET: SchedaClinicaRecords/Create
        // Prende come parametro ID del libretto al quale è associata e restituisce la vista
        // Recupera la lista dei veterinari e la passa alla vista tramite ViewData
        public ActionResult Create(int idlibretto_fk)
        {
            var nuovorecord = new SchedaClinicaRecords
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

        // POST: SchedaClinicaRecords/Create
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdRecordSC,IdLibretto_FK,DataVisita,Diagnosi,IdUtente_FK,Veterinario,Prezzo")] SchedaClinicaRecords schedaClinicaRecords)
        {
            if (ModelState.IsValid)
            {
                db.SchedaClinicaRecords.Add(schedaClinicaRecords);
                db.SaveChanges();
                return RedirectToAction("Details", "Libretto", new { id = schedaClinicaRecords.IdLibretto_FK });
            }

            ViewBag.IdLibretto_FK = new SelectList(db.Libretto, "IdLibretto", "NumMicrochip", schedaClinicaRecords.IdLibretto_FK);
            ViewBag.IdUtente_FK = new SelectList(db.Utenti, "IdUtente", "Username", schedaClinicaRecords.IdUtente_FK);
            return View(schedaClinicaRecords);
        }

        // GET: SchedaClinicaRecords/Edit/5
        // Prende come parametro l'id del record e ne restituisce la vista
        // Recupera la lista dei veterinari e la passa alla vista tramite ViewData
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SchedaClinicaRecords schedaClinicaRecords = db.SchedaClinicaRecords.Find(id);
            if (schedaClinicaRecords == null)
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

            return View(schedaClinicaRecords);
        }

        // POST: SchedaClinicaRecords/Edit/5
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        // Ricarica la lista dei veterinari in caso di errore di validazione
        // Return to action alla view del libretto associato
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdRecordSC,IdLibretto_FK,DataVisita,Diagnosi,IdUtente_FK,Veterinario,Prezzo")] SchedaClinicaRecords schedaClinicaRecords)
        {
            if (ModelState.IsValid)
            {
                db.Entry(schedaClinicaRecords).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Libretto", new { id = schedaClinicaRecords.IdLibretto_FK });
            }
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

            // Non mi servono nè la lista dei libretti nè quella degli utenti
            //ViewBag.IdLibretto_FK = new SelectList(db.Libretto, "IdLibretto", "NumMicrochip", schedaClinicaRecords.IdLibretto_FK);
            //ViewBag.IdUtente_FK = new SelectList(db.Utenti, "IdUtente", "Username", schedaClinicaRecords.IdUtente_FK);
            return View(schedaClinicaRecords);
        }

        // GET: SchedaClinicaRecords/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SchedaClinicaRecords schedaClinicaRecords = db.SchedaClinicaRecords.Find(id);
            if (schedaClinicaRecords == null)
            {
                return HttpNotFound();
            }
            return View(schedaClinicaRecords);
        }

        // POST: SchedaClinicaRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SchedaClinicaRecords schedaClinicaRecords = db.SchedaClinicaRecords.Find(id);
            db.SchedaClinicaRecords.Remove(schedaClinicaRecords);
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
