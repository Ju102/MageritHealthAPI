using MageritHealthAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MageritHealthAPI.Data
{
    public class MageritHealthDbContext : DbContext
    {
        public MageritHealthDbContext(DbContextOptions<MageritHealthDbContext> options) : base(options) {}

        public DbSet<Analitica> Analiticas { get; set; }
        public DbSet<AntecedenteMedico> AntecedentesMedicos { get; set; }
        public DbSet<Cita> Citas { get; set; }
        public DbSet<Credencial> Credenciales { get; set; }
        public DbSet<Especialidad> Especialidades { get; set; }
        public DbSet<InfoClinicaPaciente> InfoClinicaPacientes { get; set; }
        public DbSet<Medicamento> Medicamentos { get; set; }
        public DbSet<Medicion> Mediciones { get; set; }
        public DbSet<Notificacion> Notificaciones { get; set; }
        public DbSet<Prescripcion> Prescripciones { get; set; }
        public DbSet<TipoMedicion> TiposMedicion { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- CONFIGURACIÓN DE USUARIO ---
            modelBuilder.Entity<Usuario>(entity =>
            {
                // DNI y Email únicos
                entity.HasIndex(u => u.Dni).IsUnique();
                entity.HasIndex(u => u.Email).IsUnique();

                // Relación 1:N con Especialidad (Restringir borrado si hay doctores asignados)
                entity.HasOne(u => u.Especialidad)
                    .WithMany(e => e.Doctores)
                    .HasForeignKey(u => u.IdEspecialidad)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación 1:1 con Credenciales (Borrado en cascada para seguridad)
                entity.HasOne(u => u.Credencial)
                    .WithOne(c => c.Usuario)
                    .HasForeignKey<Credencial>(c => c.IdUsuario)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación 1:1 con Info Clínica
                entity.HasOne(u => u.InfoClinica)
                    .WithOne(i => i.Paciente)
                    .HasForeignKey<InfoClinicaPaciente>(i => i.IdPaciente)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación 1:N - Antecedentes (Protección de datos históricos)
                entity.HasMany(u => u.Antecedentes)
                    .WithOne(a => a.Paciente)
                    .HasForeignKey(a => a.IdPaciente)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // --- CONFIGURACIÓN DE CITA ---
            modelBuilder.Entity<Cita>(entity =>
            {
                // Relación con Paciente
                entity.HasOne(c => c.Paciente)
                    .WithMany(u => u.CitasComoPaciente)
                    .HasForeignKey(c => c.IdPaciente)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación con Doctor
                entity.HasOne(c => c.Doctor)
                    .WithMany(u => u.CitasComoDoctor)
                    .HasForeignKey(c => c.IdDoctor)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación 1:N - Analíticas
                entity.HasMany(c => c.Analiticas)
                    .WithOne(a => a.Cita)
                    .HasForeignKey(a => a.IdCita)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación 1:N - Prescripciones (Protección de historial farmacológico)
                entity.HasMany(c => c.Prescripciones)
                    .WithOne(p => p.Cita)
                    .HasForeignKey(p => p.IdCita)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // --- CONFIGURACIÓN DE PRECRIPCIÓN ---
            modelBuilder.Entity<Prescripcion>()
                .HasOne(p => p.Medicamento)
                .WithMany(m => m.Prescripciones)
                .HasForeignKey(p => p.IdMedicamento)
                .OnDelete(DeleteBehavior.Restrict);

            // --- CONFIGURACIÓN DE NOTIFICACIÓN ---
            modelBuilder.Entity<Notificacion>()
                .HasOne(n => n.Usuario)
                .WithMany()
                .HasForeignKey(n => n.IdUsuario)
                .OnDelete(DeleteBehavior.Cascade);

            // --- CONFIGURACIÓN DE MEDICIÓN ---
            modelBuilder.Entity<Medicion>(entity =>
            {
                // Relación con Analítica (Borrar mediciones si se elimina la analítica)
                entity.HasOne(m => m.Analitica)
                    .WithMany(a => a.Mediciones)
                    .HasForeignKey(m => m.IdAnalitica)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación con Tipo de Medición
                entity.HasOne(m => m.TipoMedicion)
                    .WithMany(tm => tm.Mediciones)
                    .HasForeignKey(m => m.IdTipoMedicion)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // --- CONFIGURACIÓN DE PRECISIONES DECIMALES ---
            modelBuilder.Entity<Medicion>()
                .Property(m => m.ValorMedicion).HasPrecision(10, 2);

            modelBuilder.Entity<TipoMedicion>(entity =>
            {
                entity.Property(tm => tm.ValorMaximo).HasPrecision(10, 2);
                entity.Property(tm => tm.ValorMinimo).HasPrecision(10, 2);
            });

            modelBuilder.Entity<InfoClinicaPaciente>()
                .Property(i => i.PesoActual).HasPrecision(5, 2);
        }
    }
}
