namespace CapstoneProject_TailTales.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Pet")]
    public partial class Pet
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Pet()
        {
            AmiciziaPet = new HashSet<AmiciziaPet>();
            AmiciziaPet1 = new HashSet<AmiciziaPet>();
            AmiciziaVetPet = new HashSet<AmiciziaVetPet>();
            Libretto = new HashSet<Libretto>();
            RichiestaContatto = new HashSet<RichiestaContatto>();
            RichiestaContatto1 = new HashSet<RichiestaContatto>();
        }

        [Key]
        public int IdPet { get; set; }

        public int IdUtente_FK { get; set; }

        [Required]
        [StringLength(15)]
        public string Tipo { get; set; }

        [Required]
        [StringLength(150)]
        public string Razza { get; set; }

        [Required]
        [StringLength(50)]
        public string Nome { get; set; }

        [Required]
        [StringLength(2)]
        public string Sesso { get; set; }

        public static readonly List<string> Generi = new List<string> { "M", "F" };

        public string ImgProfilo { get; set; }

        [StringLength(10)]
        public string DataNascita { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AmiciziaPet> AmiciziaPet { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AmiciziaPet> AmiciziaPet1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AmiciziaVetPet> AmiciziaVetPet { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Libretto> Libretto { get; set; }

        public virtual Utenti Utenti { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RichiestaContatto> RichiestaContatto { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RichiestaContatto> RichiestaContatto1 { get; set; }
    }
}
