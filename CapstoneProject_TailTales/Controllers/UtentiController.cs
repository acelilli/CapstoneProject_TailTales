using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CapstoneProject_TailTales.Models;

namespace CapstoneProject_TailTales.Controllers
{
    public class UtentiController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: Utenti
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var utenti = db.Utenti.Include(u => u.Ruoli);
            return View(utenti.ToList());
        }

        // GET: Utenti/Details/5
        // Prende come parametro l'id dell'utente
        // Recupera:
        // 1. I pet associati all'id utente
        // 2. Richieste in sospeso
        //    2.1 Ricupera gli username degli utenti richiedenti e richiesti passandoli alla Viewbag in due passaggi:
        //         2.1.1 Seleziona gli id
        //         2.1.2 Cerca l'id corrispondente nella tabella utenti
        // 3. Recupera le amicizie collegate all'utente
        //    3.1 Recupera gli username degli utenti richiedenti e richiesti passandoli alla Viewbag in due passaggi:
        //        3.1.1 Seleziona gli id
        //        3.1.2 Cerca l'id corrispondente nella tabella utenti
        // 4. Recupera i record di Album foto dell'id utente associato
        // Ritorna: La varie Viewbag + Utente
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Utenti utenti = db.Utenti.Find(id);
            if (utenti == null)
            {
                return HttpNotFound();
            }
            // Recupera le richieste in sospeso, gli amici dell'utente, e altri dati necessari dal database
            var userPets = db.Pet.Where(p => p.IdUtente_FK == id).ToList();
            var richiesteInSospeso = db.RichiestaContatto.Where(r => (r.IdUtenteRichiedente == id || r.IdPetRichiesto == id) && r.StatoRichiesta == "In Attesa...").ToList();
            var amiciUtente = db.AmiciziaUtenti.Where(a => a.IdUtenteRichiesto == id || a.IdUtenteRichiedente == id).ToList();

            // Recupera gli username degli utenti richiedenti e richiesti e passali alla ViewBag in due passaggi:
            // 1. Selezion gli id
            // 2. Cerca l'id corrispondente nella tabella utenti
            var idUtentiRichiedenti = richiesteInSospeso.Select(r => r.IdUtenteRichiedente).ToList();
            var idUtentiRichiesti = richiesteInSospeso.Select(r => r.IdUtenteRichiesto).ToList();

            var utentiRichiedenti = db.Utenti.Where(u => idUtentiRichiedenti.Contains(u.IdUtente)).ToList();
            var utentiRichiesti = db.Utenti.Where(u => idUtentiRichiesti.Contains(u.IdUtente)).ToList();


            // Recipera gli username degli utenti amici e li passa alla viewbag in due passaggi:
            // 1. Selezion gli id
            // 2. Cerca l'id corrispondente nella tabella utenti
            var idAmicRichiedenti = amiciUtente.Select(au => au.IdUtenteRichiedente).ToList();
            var idAmiciRichiesti = amiciUtente.Select(au => au.IdUtenteRichiesto).ToList();

            var amiciRichiedenti = db.Utenti.Where(u => idAmicRichiedenti.Contains(u.IdUtente)).ToList();
            var amiciRichiesti = db.Utenti.Where(u => idAmiciRichiesti.Contains(u.IdUtente)).ToList();

            //Recupero la raccolta fotografica dell'utente
            var userPics = db.AlbumFoto.Where(af => af.IdUtente_FK == id).ToList();

            ViewBag.UserPets = userPets;
            ViewBag.RichiesteInSospeso = richiesteInSospeso;
            ViewBag.UtentiRichiedenti = utentiRichiedenti;
            ViewBag.UtentiRichiesti = utentiRichiesti;
            ViewBag.AmiciUtente = amiciUtente;
            ViewBag.AmiciRichiedenti = amiciRichiedenti;
            ViewBag.AmiciRichiesti= amiciRichiesti;
            ViewBag.AlbumUtente = userPics;

            return View(utenti);
        }

        // GET: Utenti/Create
        /// 1. Ottengo le regioni e le province da RegioniProvince
        /// 2. Nascondo il ruolo di Admin all'elenco di ruoli selezionabili dall'utente
        /// 3. Inizializza RuoliList con la lista dei ruoli escludento quello di amministratore
        /// Ritorna la vista
        public ActionResult Create()
        {
            // RegioniProvince
            var regioniEProvince = RegioniProvince.GetRegioniEProvince();
            List<string> regioni = regioniEProvince.Item1;
            Dictionary<string, List<string>> provincePerRegione = regioniEProvince.Item2;

            var ruoliList = db.Ruoli.Where(r => r.Ruolo != "admin")
                          .Select(r => new SelectListItem { Value = r.IdRuolo.ToString(), Text = r.Ruolo })
                          .ToList();

            ViewData["Regioni"] = regioni;
            ViewData["ProvincePerRegione"] = provincePerRegione;

            var utente = new Utenti { RuoliList = ruoliList };

            return View(utente);
        }

        // POST: Utenti/Create
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        // Sommario della action: 
        /// 1. Nascondo la password in fase di creazione:
        ///    1.1 Genera la password hash (se il modello è valido)
        ///    Parametri: le proprietà di Utenti e il nuovo utenti (inserito dall'utente)
        /// 2. Check che l'username e la mail inserite non siano già presente nel db (da aggiungere)
        ///  Ritorna la view
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdUtente,IdRuolo_FK,Username,Email,Password,Nome,Cognome,Regione,Provincia,SelectedRuoloId")] Utenti utenti)
        {
            if (ModelState.IsValid)
            {
                // Converti SelectedRuoloId in int e assegna il valore a IdRuolo_FK
                if (int.TryParse(utenti.SelectedRuoloId, out int ruoloId))
                {
                    utenti.IdRuolo_FK = ruoloId;
                }
                utenti.Password = Psw.HashPassword(utenti.Password);
                db.Utenti.Add(utenti);
                db.SaveChanges();
                return RedirectToAction("Login");
            }
            return View(utenti);
        }

        /////// GET: Utenti/Edit/5
        //// Come parametro prende l'id dell'utente da editare 
        //// 1. Nasconde il ruolo admin per tutti tranne che per gli admin
        //// 2. Popolamento delle liste delle regioni e delle province
        //// 3. Solo l'utente cui ID corrisponde al cookie può modificare il proprio profilo
        //// 4. Check che l'username sia unico (da aggiungere)
        /////
        ///[ <summary>
        /// [
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            int userId;
            string userIdCookieValue = Request.Cookies["IDUserCookie"]?.Value;
            bool isUserIdValid = int.TryParse(userIdCookieValue, out userId);

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Utenti utenti = db.Utenti.Find(id);

            if (utenti == null)
            {
                return HttpNotFound();
            }

            // Se l'utente è un amministratore, può modificare qualsiasi utente
            if (User.IsInRole("Admin"))
            {
                var ruoli = db.Ruoli.Where(r => r.IdRuolo != 1 && r.Ruolo != "admin").ToList();
                var adminRole = db.Ruoli.SingleOrDefault(r => r.IdRuolo == 1 && r.Ruolo == "admin");
                if (adminRole != null)
                {
                    ruoli.Add(adminRole);
                }

                ViewBag.Ruoli = ruoli;

                var regioniEProvince = RegioniProvince.GetRegioniEProvince();
                List<string> regioni = regioniEProvince.Item1;
                Dictionary<string, List<string>> provincePerRegione = regioniEProvince.Item2;

                ViewData["Regioni"] = regioni;
                ViewData["ProvincePerRegione"] = provincePerRegione;

                return View(utenti);
            }
            else
            {
                if (isUserIdValid && (utenti.IdUtente == userId))
                {
                    var ruoli = db.Ruoli.Where(r => r.IdRuolo != 1 && r.Ruolo != "admin").ToList();

                    ViewBag.Ruoli = ruoli;

                    var regioniEProvince = RegioniProvince.GetRegioniEProvince();
                    List<string> regioni = regioniEProvince.Item1;
                    Dictionary<string, List<string>> provincePerRegione = regioniEProvince.Item2;

                    ViewData["Regioni"] = regioni;
                    ViewData["ProvincePerRegione"] = provincePerRegione;

                    return View(utenti);
                }
                else
                {
                    return RedirectToAction("AccessDenied", "Error");
                }
            }
        }


        // POST: Utenti/Edit/5
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdUtente,IdRuolo_FK,Username,Email,Password,Nome,Cognome,Regione,Provincia")] Utenti utenti)
        {

            if (ModelState.IsValid)
            {
                utenti.Password = Psw.HashPassword(utenti.Password);
                db.Entry(utenti).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMessage"] = "Le modifiche al profilo sono state salvate con successo.";
                return RedirectToAction("Index", "Home");
            }
            ViewBag.IdRuolo_FK = new SelectList(db.Ruoli, "IdRuolo", "Ruolo", utenti.IdRuolo_FK);
            return View(utenti);
        }

        // GET: Utenti/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Utenti utenti = db.Utenti.Find(id);
            if (utenti == null)
            {
                return HttpNotFound();
            }
            return View(utenti);
        }

        // POST: Utenti/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Utenti utenti = db.Utenti.Find(id);
            db.Utenti.Remove(utenti);
            db.SaveChanges();
            return RedirectToAction("Details", "Utenti", utenti.IdUtente);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /////////////////////// Metodi per LogIn e LogOut ///////////////////
        ///
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(Utenti user)
        {
            try
            {
                var utente = db.Utenti.FirstOrDefault(u => u.Username == user.Username);
                string hashedpass = Psw.HashPassword(user.Password);
                if (utente != null && hashedpass == utente.Password)
                {
                    FormsAuthentication.SetAuthCookie(user.Username, false);
                    HttpCookie LoginCookie = new HttpCookie("IDUserCookie");
                    LoginCookie.Value = utente.IdUtente.ToString();
                    LoginCookie.Expires = DateTime.Now.AddHours(72);
                    Response.Cookies.Add(LoginCookie);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("La password inserita è " + user.Password + " e quella di user è:" + utente.Password);
                    System.Diagnostics.Debug.WriteLine("Il valore di utente è " + utente.Username + " E quello di user è " + user.Username);
                    ViewBag.AuthError = "Login non riuscito: username o password non validi.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.AuthError = "Errore durante l'autenticazione: " + ex.Message;
                System.Diagnostics.Debug.WriteLine("Errore: " + ex.Message);
                return View();
            }
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            if (Request.Cookies["IDUserCookie"] != null)
            {
                var userCookie = new HttpCookie("IDUserCookie");
                userCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(userCookie);
            }

            return RedirectToAction("Index", "Home");
        }

    }
}
