namespace CapstoneProject_TailTales.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SchedaClinicaRecords
    {
        [Key]
        public int IdRecordSC { get; set; }

        public int IdLibretto_FK { get; set; }

        [Column(TypeName = "date")]
        public DateTime DataVisita { get; set; }

        [Required]
        public string Diagnosi { get; set; }

        public int? IdUtente_FK { get; set; }

        [StringLength(100)]
        public string Veterinario { get; set; }

        [Column(TypeName = "money")]
        public decimal? Prezzo { get; set; }

        public virtual Libretto Libretto { get; set; }

        public virtual Utenti Utenti { get; set; }
    }
}
