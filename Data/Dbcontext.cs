using Microsoft.EntityFrameworkCore;
using VeterinarySystem.Models;

namespace VeterinarySystem.Data;

public class VeterinaryContext : DbContext
{
    public VeterinaryContext(DbContextOptions<VeterinaryContext> options) 
        : base(options) { }

    public DbSet<Owner> Owners => Set<Owner>();
    public DbSet<Pet> Pets => Set<Pet>();
    public DbSet<Veterinarian> Veterinarians => Set<Veterinarian>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Treatment> Treatments => Set<Treatment>();
    public DbSet<Medication> Medications => Set<Medication>();
    public DbSet<TreatmentMedication> TreatmentMedications => Set<TreatmentMedication>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Owner: unicidad en Document y Email
        modelBuilder.Entity<Owner>()
            .HasIndex(o => o.Document).IsUnique();
        modelBuilder.Entity<Owner>()
            .HasIndex(o => o.Email).IsUnique();

        // Owner a Pets (1 a muchos)
        modelBuilder.Entity<Pet>()
            .HasOne(p => p.Owner)
            .WithMany(o => o.Pets)
            .HasForeignKey(p => p.OwnerId)
            .OnDelete(DeleteBehavior.Restrict); // esta validacion es para que no se puedan borrar Owners con mascotas registradas.

        // Veterinarian: unicidad en Name + Specialty
        modelBuilder.Entity<Veterinarian>()
            .HasIndex(v => new { v.Name, v.Specialty }).IsUnique();

        // Appointment → Pet, Owner, Veterinarian
        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Pet)
            .WithMany(p => p.Appointments)
            .HasForeignKey(a => a.PetId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Owner)
            .WithMany(o => o.Appointments) // navegación indirecta, se resuelve via Pet
            .HasForeignKey(a => a.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Veterinarian)
            .WithMany(v => v.Appointments)
            .HasForeignKey(a => a.VeterinarianId)
            .OnDelete(DeleteBehavior.Restrict);

        // Treatment: relación 1 a 1 con Appointment
        modelBuilder.Entity<Treatment>()
            .HasOne(t => t.Appointment)
            .WithOne(a => a.Treatment)
            .HasForeignKey<Treatment>(t => t.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // TreatmentMedication: tabla pivote
        modelBuilder.Entity<TreatmentMedication>()
            .HasOne(tm => tm.Treatment)
            .WithMany(t => t.TreatmentMedications)
            .HasForeignKey(tm => tm.TreatmentId);

        modelBuilder.Entity<TreatmentMedication>()
            .HasOne(tm => tm.Medication)
            .WithMany(m => m.TreatmentMedications)
            .HasForeignKey(tm => tm.MedicationId);

        // Precisión decimal para peso
        modelBuilder.Entity<Pet>()
            .Property(p => p.Weight)
            .HasPrecision(5, 2);
    }
}