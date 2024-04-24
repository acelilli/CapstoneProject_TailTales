namespace CapstoneProject_TailTales.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class VacciniRecords
    {
        [Key]
        public int IdRecordVaccini { get; set; }

        public int IdLibretto_FK { get; set; }

        public bool VaccinoRabbia { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DataPrevista { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DataEffettuato { get; set; }

        public string VaccinoNLotto { get; set; }

        [Column(TypeName = "date")]
        public DateTime? Scadenza { get; set; }

        public bool? Richiamo { get; set; }

        public int? IdUtente_FK { get; set; }

        [StringLength(100)]
        public string Veterinario { get; set; }

        [Column(TypeName = "money")]
        public decimal? Prezzo { get; set; }

        [StringLength(100)]
        public string PuntoInoculo { get; set; }

        public virtual Libretto Libretto { get; set; }

        public virtual Utenti Utenti { get; set; }
    }
}
