using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace CapstoneProject_TailTales.Models
{
    public partial class ModelDbContext : DbContext
    {
        public ModelDbContext()
            : base("name=ModelDbContext")
        {
        }

        public virtual DbSet<AlbumFoto> AlbumFoto { get; set; }
        public virtual DbSet<AmiciziaPet> AmiciziaPet { get; set; }
        public virtual DbSet<AmiciziaUtenti> AmiciziaUtenti { get; set; }
        public virtual DbSet<AmiciziaVetPet> AmiciziaVetPet { get; set; }
        public virtual DbSet<Libretto> Libretto { get; set; }
        public virtual DbSet<Memo> Memo { get; set; }
        public virtual DbSet<Pet> Pet { get; set; }
        public virtual DbSet<RichiestaContatto> RichiestaContatto { get; set; }
        public virtual DbSet<Ruoli> Ruoli { get; set; }
        public virtual DbSet<SchedaClinicaRecords> SchedaClinicaRecords { get; set; }
        public virtual DbSet<Utenti> Utenti { get; set; }
        public virtual DbSet<VacciniRecords> VacciniRecords { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Libretto>()
                .Property(e => e.NumEnci)
                .IsFixedLength();

            modelBuilder.Entity<Libretto>()
                .HasMany(e => e.SchedaClinicaRecords)
                .WithRequired(e => e.Libretto)
                .HasForeignKey(e => e.IdLibretto_FK)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Libretto>()
                .HasMany(e => e.VacciniRecords)
                .WithRequired(e => e.Libretto)
                .HasForeignKey(e => e.IdLibretto_FK)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Pet>()
                .HasMany(e => e.AmiciziaPet)
                .WithRequired(e => e.Pet)
                .HasForeignKey(e => e.IdPetRichiedente)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Pet>()
                .HasMany(e => e.AmiciziaPet1)
                .WithRequired(e => e.Pet1)
                .HasForeignKey(e => e.IdPetRichiesto)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Pet>()
                .HasMany(e => e.AmiciziaVetPet)
                .WithRequired(e => e.Pet)
                .HasForeignKey(e => e.IdPetRichiesto)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Pet>()
                .HasMany(e => e.Libretto)
                .WithRequired(e => e.Pet)
                .HasForeignKey(e => e.IdPet_FK)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Pet>()
                .HasMany(e => e.RichiestaContatto)
                .WithOptional(e => e.Pet)
                .HasForeignKey(e => e.IdPetRichiedente);

            modelBuilder.Entity<Pet>()
                .HasMany(e => e.RichiestaContatto1)
                .WithOptional(e => e.Pet1)
                .HasForeignKey(e => e.IdPetRichiesto);

            modelBuilder.Entity<Ruoli>()
                .HasMany(e => e.Utenti)
                .WithRequired(e => e.Ruoli)
                .HasForeignKey(e => e.IdRuolo_FK)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SchedaClinicaRecords>()
                .Property(e => e.Prezzo)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Utenti>()
                .HasMany(e => e.AlbumFoto)
                .WithRequired(e => e.Utenti)
                .HasForeignKey(e => e.IdUtente_FK)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Utenti>()
                .HasMany(e => e.AmiciziaUtenti)
                .WithRequired(e => e.Utenti)
                .HasForeignKey(e => e.IdUtenteRichiedente)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Utenti>()
                .HasMany(e => e.AmiciziaUtenti1)
                .WithRequired(e => e.Utenti1)
                .HasForeignKey(e => e.IdUtenteRichiesto)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Utenti>()
                .HasMany(e => e.AmiciziaVetPet)
                .WithRequired(e => e.Utenti)
                .HasForeignKey(e => e.IdUtenteRichiedente)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Utenti>()
                .HasMany(e => e.Memo)
                .WithRequired(e => e.Utenti)
                .HasForeignKey(e => e.IdUtente_FK)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Utenti>()
                .HasMany(e => e.Pet)
                .WithRequired(e => e.Utenti)
                .HasForeignKey(e => e.IdUtente_FK)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Utenti>()
                .HasMany(e => e.RichiestaContatto)
                .WithRequired(e => e.Utenti)
                .HasForeignKey(e => e.IdUtenteRichiedente)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Utenti>()
                .HasMany(e => e.RichiestaContatto1)
                .WithRequired(e => e.Utenti1)
                .HasForeignKey(e => e.IdUtenteRichiesto)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Utenti>()
                .HasMany(e => e.SchedaClinicaRecords)
                .WithOptional(e => e.Utenti)
                .HasForeignKey(e => e.IdUtente_FK);

            modelBuilder.Entity<Utenti>()
                .HasMany(e => e.VacciniRecords)
                .WithOptional(e => e.Utenti)
                .HasForeignKey(e => e.IdUtente_FK);

            modelBuilder.Entity<VacciniRecords>()
                .Property(e => e.Prezzo)
                .HasPrecision(19, 4);
        }
    }
}
