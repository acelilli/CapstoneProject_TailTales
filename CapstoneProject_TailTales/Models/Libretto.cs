namespace CapstoneProject_TailTales.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Libretto")]
    public partial class Libretto
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Libretto()
        {
            SchedaClinicaRecords = new HashSet<SchedaClinicaRecords>();
            VacciniRecords = new HashSet<VacciniRecords>();
        }

        [Key]
        public int IdLibretto { get; set; }

        public int IdPet_FK { get; set; }

        [StringLength(15)]
        public string NumMicrochip { get; set; }

        [Required]
        [StringLength(100)]
        public string Proprietario { get; set; }

        [Required]
        public string Indirizzo { get; set; }

        public string Provenienza { get; set; }

        [StringLength(16)]
        public string NumEnci { get; set; }

        public bool Sterilizzato { get; set; }

        public virtual Pet Pet { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SchedaClinicaRecords> SchedaClinicaRecords { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VacciniRecords> VacciniRecords { get; set; }
    }
}
