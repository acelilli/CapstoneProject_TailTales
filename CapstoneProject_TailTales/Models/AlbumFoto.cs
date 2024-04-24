namespace CapstoneProject_TailTales.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AlbumFoto")]
    public partial class AlbumFoto
    {
        [Key]
        public int IdAlbum { get; set; }

        public int IdUtente_FK { get; set; }

        public DateTime DataRecord { get; set; } = DateTime.Now;

        [Required]
        public string ImgUrl { get; set; }

        public string Descrizione { get; set; }

        public virtual Utenti Utenti { get; set; }
    }
}
