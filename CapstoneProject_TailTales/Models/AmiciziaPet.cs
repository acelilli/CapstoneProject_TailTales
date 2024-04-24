namespace CapstoneProject_TailTales.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AmiciziaPet")]
    public partial class AmiciziaPet
    {
        [Key]
        public int IdAmiciziaPet { get; set; }

        public int IdPetRichiedente { get; set; }

        public int IdPetRichiesto { get; set; }

        public virtual Pet Pet { get; set; }

        public virtual Pet Pet1 { get; set; }
    }
}
