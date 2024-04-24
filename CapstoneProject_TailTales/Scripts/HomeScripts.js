document.addEventListener("DOMContentLoaded", function () {
    var prossimiAppuntamenti = document.getElementById("prossimiAppuntamenti");
    var proxShowMore = document.getElementById("proxShowMore");

    proxShowMore.addEventListener("click", function () { // Correzione qui
        if (prossimiAppuntamenti.style.height === "fit-content") {
            // Nascondi il contenuto quando è espanso
            prossimiAppuntamenti.style.height = "2.5rem"; // O qualsiasi altezza desiderata
            proxShowMore.innerText = "Mostra di più";
        } else {
            // Espandi il contenuto quando è nascosto
            prossimiAppuntamenti.style.height = "fit-content";
            proxShowMore.innerText = "Mostra meno";
        }
    });
});

//Funzione che nasconde/mostra gli ultimi appuntamenti
document.addEventListener("DOMContentLoaded", function () {
    var ultimiAppuntamenti = document.getElementById("ultimiAppuntamenti");
    var lastShowMore = document.getElementById("lastShowMore");

    lastShowMore.addEventListener("click", function () {
        if (ultimiAppuntamenti.style.height === "fit-content") {
            // Nascondi il contenuto quando è espanso
            ultimiAppuntamenti.style.height = "2.5rem"; // O qualsiasi altezza desiderata
            lastShowMore.innerText = "Mostra di più";
        } else {
            // Espandi il contenuto quando è nascosto
            ultimiAppuntamenti.style.height = "fit-content";
            lastShowMore.innerText = "Mostra meno";
        }
    });
});


//Funzione che nasconde/mostra i memo 
document.addEventListener("DOMContentLoaded", function () {
    var memoContainer = document.getElementById("memoContainer");
    var showMoreBtn = document.getElementById("memoShowMore");

    showMoreBtn.addEventListener("click", function () {
        if (memoContainer.style.height === "fit-content") {
            // Nascondi il contenuto quando è espanso
            memoContainer.style.height = "2.5rem"; // O qualsiasi altezza desiderata
            showMoreBtn.innerText = "Mostra di più";
        } else {
            // Espandi il contenuto quando è nascosto
            memoContainer.style.height = "fit-content";
            showMoreBtn.innerText = "Mostra meno";
        }
    });
});

// Funzione per l'aggiornamento del memo via AJAX
function updateMemo(memoId, completato) {
    // Creare un oggetto FormData per i parametri
    var formData = new FormData();
    formData.append('memoId', memoId);
    formData.append('completato', completato);

    // Effettuare la richiesta POST
    fetch('/Home/UpdateMemo', {
        method: 'POST',
        body: formData
    })
        .then(response => {
            if (response.ok) {
                console.log('Memo aggiornato con successo.');
            } else {
                console.error('Si è verificato un errore durante l\'aggiornamento del memo.');
            }
        })
        .catch(error => {
            console.error('Si è verificato un errore durante l\'invio della richiesta:', error);
        });
}

// Funzione che Invia automaticamente il form quando l'utente modifica il checkbox
function autoSubmitFormOnChange() {
    document.querySelectorAll('input[type="checkbox"]').forEach(function (checkbox) {
        checkbox.addEventListener('change', function () {
            // Otteniamo l'ID del memo e lo stato del checkbox
            var memoId = this.value;
            var completato = this.checked;

            // Chiamiamo la funzione per l'aggiornamento del memo
            updateMemo(memoId, completato);
        });
    });
}
// Chiamata alla funzione per inizializzare l'invio automatico del form
autoSubmitFormOnChange();


function calcolaEtaPet(dataDiNascita) {
    if (!dataDiNascita) {
        return "Data di nascita non disponibile";
    }

    var dataNascitaArray = dataDiNascita.split('-');
    var anno = parseInt(dataNascitaArray[dataNascitaArray.length - 1]);

    var mese = dataNascitaArray.length > 1 ? parseInt(dataNascitaArray[0]) : 1;
    var giorno = dataNascitaArray.length > 2 ? parseInt(dataNascitaArray[1]) : 1;

    var dataNascita = new Date(anno, mese - 1, giorno);

    if (isNaN(dataNascita.getTime())) {
        return "Data di nascita non valida";
    }

    var dataAttuale = new Date();
    var differenzaAnni = dataAttuale.getFullYear() - dataNascita.getFullYear();
    var differenzaMesi = dataAttuale.getMonth() - dataNascita.getMonth();
    var differenzaGiorni = dataAttuale.getDate() - dataNascita.getDate();

    if (differenzaMesi < 0 || (differenzaMesi === 0 && differenzaGiorni < 0)) {
        differenzaAnni--;
    }

    if (differenzaMesi < 0) {
        differenzaMesi = 12 + differenzaMesi;
    }

    var etaPet = differenzaAnni + " anni";
    if (differenzaAnni === 0) {
        etaPet = differenzaMesi + " mesi";
    }

    return etaPet;
}

// Chiama la funzione per ogni elemento con la classe "etaPet"
var spanEtaPet = document.querySelectorAll(".etaPet");
spanEtaPet.forEach(function (elemento) {
    var dataNascitaPet = elemento.getAttribute("data-nascita");
    elemento.innerText = calcolaEtaPet(dataNascitaPet);
});