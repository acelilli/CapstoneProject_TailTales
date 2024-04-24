namespace CapstoneProject_TailTales.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AmiciziaUtenti")]
    public partial class AmiciziaUtenti
    {
        [Key]
        public int IdAmicizia { get; set; }

        [Required]
        public int IdUtenteRichiedente { get; set; }

        [Required]
        public int IdUtenteRichiesto { get; set; }

        public virtual Utenti Utenti { get; set; }

        public virtual Utenti Utenti1 { get; set; }
    }
}
