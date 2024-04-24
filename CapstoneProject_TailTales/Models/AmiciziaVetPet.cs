namespace CapstoneProject_TailTales.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AmiciziaVetPet")]
    public partial class AmiciziaVetPet
    {
        [Key]
        public int IdAmiciziaVetPet { get; set; }

        public int IdUtenteRichiedente { get; set; }

        public int IdPetRichiesto { get; set; }

        public virtual Pet Pet { get; set; }

        public virtual Utenti Utenti { get; set; }
    }
}
