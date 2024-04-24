namespace CapstoneProject_TailTales.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RichiestaContatto")]
    public partial class RichiestaContatto
    {
        [Key]
        public int IdRichiesta { get; set; }

        public int IdUtenteRichiedente { get; set; }

        public int IdUtenteRichiesto { get; set; }

        [Required]
        [StringLength(30)]
        public string TipoRelazione { get; set; }

        public int? IdPetRichiedente { get; set; }

        public int? IdPetRichiesto { get; set; }

        [Required]
        [StringLength(15)]
        public string StatoRichiesta { get; set; }

        public DateTime DataRichiesta { get; set; }

        public virtual Pet Pet { get; set; }

        public virtual Pet Pet1 { get; set; }

        public virtual Utenti Utenti { get; set; }

        public virtual Utenti Utenti1 { get; set; }
    }
}
