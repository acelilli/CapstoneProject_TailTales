namespace CapstoneProject_TailTales.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Memo")]
    public partial class Memo
    {
        [Key]
        public int IdMemo { get; set; }

        public int IdUtente_FK { get; set; }

        [Column(TypeName = "date")]
        public DateTime DataMemo { get; set; }

        [Required]
        [StringLength(1700)]
        public string Descrizione { get; set; }

        public bool Completato { get; set; }

        public virtual Utenti Utenti { get; set; }
    }
}
