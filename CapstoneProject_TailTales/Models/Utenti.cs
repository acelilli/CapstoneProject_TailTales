namespace CapstoneProject_TailTales.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
using System.IO;
    using System.Linq;
    using System.Web.Mvc;

    [Table("Utenti")]
    public partial class Utenti
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Utenti()
        {
            AlbumFoto = new HashSet<AlbumFoto>();
            AmiciziaUtenti = new HashSet<AmiciziaUtenti>();
            AmiciziaUtenti1 = new HashSet<AmiciziaUtenti>();
            AmiciziaVetPet = new HashSet<AmiciziaVetPet>();
            Memo = new HashSet<Memo>();
            Pet = new HashSet<Pet>();
            RichiestaContatto = new HashSet<RichiestaContatto>();
            RichiestaContatto1 = new HashSet<RichiestaContatto>();
            SchedaClinicaRecords = new HashSet<SchedaClinicaRecords>();
            VacciniRecords = new HashSet<VacciniRecords>();
        }

        [Key]
        public int IdUtente { get; set; }

        public int IdRuolo_FK { get; set; }

        [Required(ErrorMessage = "L'username è obbligatorio!")]
        [StringLength(30)]
        public string Username { get; set; }

        [Required(ErrorMessage = "L'email è obbligatoria!")]
        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Non è un indirizzo di posta elettronica valido!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La password è obbligatoria!")]
        [StringLength(100)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [StringLength(30)]
        public string Nome { get; set; }

        [StringLength(30)]
        public string Cognome { get; set; }

        [StringLength(30)]
        public string Regione { get; set; }

        [StringLength(30)]
        public string Provincia { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AlbumFoto> AlbumFoto { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AmiciziaUtenti> AmiciziaUtenti { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AmiciziaUtenti> AmiciziaUtenti1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AmiciziaVetPet> AmiciziaVetPet { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Memo> Memo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Pet> Pet { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RichiestaContatto> RichiestaContatto { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RichiestaContatto> RichiestaContatto1 { get; set; }

        public virtual Ruoli Ruoli { get; set; }

        // Proprietà per popolare la dropdownlist dei ruoli
        [NotMapped]
        public IEnumerable<SelectListItem> RuoliList { get; set; }

        // Proprietà per l'ID del ruolo selezionato
        [NotMapped]
        [Display(Name = "Ruolo")]
        public string SelectedRuoloId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SchedaClinicaRecords> SchedaClinicaRecords { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VacciniRecords> VacciniRecords { get; set; }

    }
}
