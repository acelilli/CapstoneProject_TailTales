using CapstoneProject_TailTales.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace CapstoneProject_TailTales.Controllers
{
    public class HomeController : Controller
    {
        private ModelDbContext db = new ModelDbContext();
        //// INDEX HOME
        /////// FASE 1: Recupero dei pet associati all'utente //////
        /// 1. Ottengo l'id dell'utente loggato dal cookie
        /// 2. Verifico se l'utente è validato e:
        ///    2.1 Se l'utente è validato, tramite il modello MyUserData recupero la lista dei suoi pet, dei libretti associati e dei record dei vaccini e delle schede cliniche 
        ///    2.2 Se l'utente non è validato, reindirizzo l'utente alla pagina di login
        /// 3. Se l'utente è validato, torna tutti i dati 
        ////// FASE 2: Calcolo degli appuntamenti futuri e di quelli passati /////
        /// 1. Definisco una var today dove viene impostata la data odierna da confrontare con i record all'interno di scheda clinica (data Effettuata) e di vaccini(data previsto)
        /// 2. I prossimi appuntamenti vengono caricati in apposite liste (prossimiAppuntamenti e passatiAppuntamenti)
        /// 3. Le liste vengono ordinate a seconda delle date, se sono minori o inferiori rispetto alla data odierna
        /// 4. Le liste vengono passate tramite ViewData alla pagina
        /// Non riceve parametri
        public ActionResult Index()
        {
            int userId;
            string userIdCookieValue = Request.Cookies["IDUserCookie"]?.Value;
            bool isUserIdValid = int.TryParse(userIdCookieValue, out userId);
            MyUserData userData = new MyUserData();
            if (isUserIdValid)
            {
                userData.MyPets = db.Pet
                                .Include("Libretto.VacciniRecords")
                                .Include("Libretto.SchedaClinicaRecords")
                                .Where(p => p.IdUtente_FK == userId)
                                .ToList();

                userData.MyMemo = db.Memo
                                     .Where(m => m.IdUtente_FK == userId)
                                    .ToList();

                userData.MyAlbum = db.AlbumFoto
                                       .Where(a => a.IdUtente_FK == userId)
                                       .ToList();

                //// FASE 2: Calcolo degli appuntamenti
                /// 2.1 Prossimi appuntamenti:
                var prossimiAppuntamenti = new List<(DateTime? DataAppuntamento, string Descrizione, string NomePet)>();

                foreach (var pet in userData.MyPets)
                {
                    var today = DateTime.Now ; 

                    foreach (var libretto in pet.Libretto)
                    {
                        foreach (var vaccino in libretto.VacciniRecords)
                        {
                            if (vaccino.DataPrevista >= today)
                            {
                                prossimiAppuntamenti.Add((vaccino.DataPrevista, vaccino.PuntoInoculo, pet.Nome));
                            }
                        }

                        foreach (var visita in libretto.SchedaClinicaRecords)
                        {
                            if (visita.DataVisita >= today)
                            {
                                prossimiAppuntamenti.Add((visita.DataVisita, visita.Diagnosi, pet.Nome));
                            }
                        }
                    }
                }
                /// 2.2 Passati Appuntamenti
                var passatiAppuntamenti = new List<(DateTime? DataAppuntamento, string Descrizione, string NomePet)>();

                foreach (var pet in userData.MyPets)
                {
                    var today = DateTime.Now;

                    foreach (var libretto in pet.Libretto)
                    {
                        foreach (var vaccino in libretto.VacciniRecords)
                        {
                            if (vaccino.DataPrevista < today)
                            {
                                passatiAppuntamenti.Add((vaccino.DataPrevista, vaccino.PuntoInoculo, pet.Nome));
                            }
                        }

                        foreach (var visita in libretto.SchedaClinicaRecords)
                        {
                            if (visita.DataVisita < today)
                            {
                                passatiAppuntamenti.Add((visita.DataVisita, visita.Diagnosi, pet.Nome));
                            }
                        }
                    }
                }
                // Ordinamento degli appuntamenti per data
                prossimiAppuntamenti = prossimiAppuntamenti.OrderBy(a => a.DataAppuntamento).ToList();
                passatiAppuntamenti.OrderByDescending(b => b.DataAppuntamento).ToList();
                // Passaggio degli appuntamenti alla vista utilizzando ViewBag
                ViewData["ProssimiAppuntamenti"] = prossimiAppuntamenti;
                ViewData["PassatiAppuntamenti"] = passatiAppuntamenti;
            }
            else
            {
                return RedirectToAction("Login", "Utenti");
            }
            return View(userData);
        }


        //---------- Pagine Extra  --------------------------

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult WorkWithUs()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Help()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Cookies()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Service()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult SiteGuide()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        /////---- Metodi per i Memos ----//////
        /////
        // 1. Recupera l'Id dell'utente dal cookie 
        // 2. Verifica se l'id dell'utente è valido 
        //    2.1 Crea un nuovo memo e lo aggiunge al db
        public ActionResult NewMemo(string descrizione)
        {
            int userId;
            string userIdCookieValue = Request.Cookies["IDUserCookie"]?.Value;
            bool isUserIdValid = int.TryParse(userIdCookieValue, out userId);

            if (isUserIdValid)
            {
                // Crea un nuovo memo
                var newMemo = new Memo
                {
                    IdUtente_FK = userId,
                    DataMemo = DateTime.Now,
                    Completato = false,
                    Descrizione = descrizione
                };
                using (var dbContext = new ModelDbContext())
                {
                    dbContext.Memo.Add(newMemo);
                    dbContext.SaveChanges();
                }
            }
            else
            {
                return RedirectToAction("Login", "Utenti");
            }
            return RedirectToAction("Index");
        }

        // Azione per l'aggiornamento del memo
        // 1. Recupera l'id dell'utente dal cookie
        // 2. Crea una lista per i memo completati e una per i memo non completati e li restituisce alla Viewbag
        [HttpGet]
        public ActionResult UpdateMemo()
        {
            int userId;
            string userIdCookieValue = Request.Cookies["IDUserCookie"]?.Value;
            bool isUserIdValid = int.TryParse(userIdCookieValue, out userId);

            if (!isUserIdValid)
            {
                return RedirectToAction("Login", "Utenti");
            }

            var userMemos = db.Memo.Where(m => m.IdUtente_FK == userId).ToList();
            var checkedMemos = userMemos.Where(m => m.Completato).ToList();
            var uncheckedMemos = userMemos.Where(m => !m.Completato).ToList();

            ViewBag.CheckedMemos = checkedMemos;
            ViewBag.UncheckedMemos = uncheckedMemos;

            return View();
        }
        [HttpPost]
        public ActionResult UpdateMemo(int memoId, bool completato)
        {
            int userId;
            string userIdCookieValue = Request.Cookies["IDUserCookie"]?.Value;
            bool isUserIdValid = int.TryParse(userIdCookieValue, out userId);

            if (isUserIdValid)
            {
                var memoToUpdate = db.Memo.FirstOrDefault(m => m.IdMemo == memoId && m.IdUtente_FK == userId);
                if (memoToUpdate != null)
                {
                    memoToUpdate.Completato = completato;
                    db.SaveChanges();
                }
            }
            else
            {
                return RedirectToAction("Login", "Utenti");
            }
            return RedirectToAction("Index");
        }

        //////// Esplora + Ricerca ////////
        ///        // Definisco un ViewModel che mi raccolga tutti gli utenti e tutti i loro pet
        public class SearchViewModel
        {
            public List<Utenti> Users { get; set; }
            public List<Pet> Pets { get; set; }
            public List<Utenti> NearbyUsers { get; set; }
            public List<Pet> MyPets { get; set; }

            public List<Utenti> MyFriends { get; set; }
        }

         //// GET: EXPLORE
        /// 1. Recupera l'utente corrente.
        /// 2. SwitchCase:
        ///    2.1 Se la regione è disponibile, visualizza gli utenti della stessa regione
        ///    2.2 Se non ci sono informazioni sulla provicnia or egione, visualizza tutti gli utenti
        /// 3. Recupera gli amici dell'utente corrente
        /// 4. Recupera gli oggetti utenti per gli Id utenti degli amici 
        /// 5. Se l'utente non è loggato ritorna alla Login
        /// 6. Concatena le liste degli utenti ottenuti in un'unica lista di utenti nearbyUsers.
        /// 7. Passa la lista degli utenti e dei loro pet alla vista Explore.
        /// Ritorna la vista Explore
        [HttpGet]
        public ActionResult Explore()
        {
            int userId;
            string userIdCookieValue = Request.Cookies["IDUserCookie"]?.Value;
            bool isUserIdValid = int.TryParse(userIdCookieValue, out userId);

            if (isUserIdValid)
            {
                var currentUser = db.Utenti.FirstOrDefault(u => u.IdUtente == userId);
                var allusers = db.Utenti.ToList();

                if (currentUser != null)
                {
                    IEnumerable<Utenti> nearbyUsers;

                    switch (currentUser)
                    {
                        case var user when string.IsNullOrEmpty(user.Provincia):
                            nearbyUsers = db.Utenti.Where(u => u.Regione == user.Regione && u.IdUtente != user.IdUtente).ToList();
                            ViewBag.Message = "Aggiungi la provincia in cui vivi per visualizzare gli utenti vicini a te!";
                            break;

                        default:
                            var usersInSameProvince = db.Utenti.Where(u => u.Provincia == currentUser.Provincia && u.IdUtente != currentUser.IdUtente).ToList();
                            var usersInSameRegion = db.Utenti.Where(u => u.Regione == currentUser.Regione && u.Provincia != currentUser.Provincia).ToList();
                            nearbyUsers = usersInSameProvince.Concat(usersInSameRegion);
                            break;
                    }

                    var friendIds = db.AmiciziaUtenti
                                        .Where(a => a.IdUtenteRichiedente == userId || a.IdUtenteRichiesto == userId)
                                        .Select(a => a.IdUtenteRichiedente == userId ? a.IdUtenteRichiesto : a.IdUtenteRichiedente)
                                        .ToList();

                    var myFriends = db.Utenti.Where(u => friendIds.Contains(u.IdUtente)).ToList();

                    var mypets = db.Pet.Where(p => p.IdUtente_FK ==  userId).ToList();

                    var viewModel = new SearchViewModel
                    {
                        Users = allusers,
                        Pets = db.Pet.ToList(),
                        NearbyUsers = nearbyUsers.ToList(),
                        MyPets = mypets.ToList(),
                        MyFriends = myFriends.ToList(),
                    };


                    return View(viewModel);
                }
                else
                {
                    return RedirectToAction("Login", "Utenti");
                }
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
        }

        ///////// AZIONI DI RICERCA /////////
        ///
        /// Recupera l'id dell'utente dal cookie, se è valido cerca l'utente corrispondente dal db
        /// Cerca gli utenti che corrispondono alla query di ricerca nella stringa query per Username, Nome e Cognome.
        /// Cerca i pet che corrispondono alla query di ricerca nella stringa query per Nome.
        /// Passa la lista degli utenti e dei loro pet, e dei pets, alla vista Search utilizzando ViewBag.
        /// Ritorna alla vista di Search per visualizzare i risultati di ricerca
        [HttpGet]
        public ActionResult Search(string query)
        {
            // Recupera l'ID dell'utente dal cookie
            int userId;
            string userIdCookieValue = Request.Cookies["IDUserCookie"]?.Value;
            bool isUserIdValid = int.TryParse(userIdCookieValue, out userId);

            // Se l'ID dell'utente nel cookie è valido, cerca l'utente corrispondente nel database
            if (isUserIdValid)
            {
                var currentUser = db.Utenti.FirstOrDefault(u => u.IdUtente == userId);
                var allusers = db.Utenti.ToList();
                var users = db.Utenti.Where(u => u.Username.Contains(query) || u.Nome.Contains(query) || u.Cognome.Contains(query)).ToList();
                var pets = db.Pet.Where(p => p.Nome.Contains(query)).ToList();
                var myPets = db.Pet.Where(p => p.IdUtente_FK == userId).ToList();
                // Recupera gli amici dell'utente corrente
                var friendIds = db.AmiciziaUtenti
                                    .Where(a => a.IdUtenteRichiedente == userId || a.IdUtenteRichiesto == userId)
                                    .Select(a => a.IdUtenteRichiedente == userId ? a.IdUtenteRichiesto : a.IdUtenteRichiedente)
                                    .ToList();
                // Recupera gli oggetti Utenti corrispondenti agli ID degli amici
                var myFriends = db.Utenti.Where(u => friendIds.Contains(u.IdUtente)).ToList();


                var viewModel = new SearchViewModel
                {
                    Users = users,
                    Pets = pets,
                    MyPets = myPets.ToList(),
                    MyFriends = myFriends.ToList(),
                };
                return View(viewModel);
            }
            return View("Login", "Utenti");
            
        }
    }
}