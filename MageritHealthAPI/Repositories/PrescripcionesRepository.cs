using MageritHealthAPI.Data;
using MageritHealthAPI.Models;
using MageritHealthAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MageritHealthAPI.Repositories
{
    public class PrescripcionesRepository : IPrescripcionesRepository
    {
        private readonly MageritHealthDbContext context;

        public PrescripcionesRepository(MageritHealthDbContext context)
        {
            this.context = context;
        }

        public async Task<Prescripcion> GetPrescripcionByIdAsync(int id)
        {
            return await this.context.Prescripciones
                .Include(p => p.Cita)
                .Include(p => p.Medicamento)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.IdPrescripcion == id);
        }

        public async Task<List<Prescripcion>> GetPrescripcionesByPacienteIdAsync(int pacienteId)
        {
            return await this.context.Prescripciones
                .Include(p => p.Cita)
                .Include(p => p.Medicamento)
                .AsNoTracking()
                .Where(p => p.Cita.IdPaciente == pacienteId)
                .ToListAsync();
        }

        public async Task<bool> CreatePrescripcionAsync(Prescripcion prescripcion)
        {
            int maxId = await this.context.Prescripciones.MaxAsync(p => (int?)p.IdPrescripcion) ?? 0;

            prescripcion.IdPrescripcion = maxId + 1;
            prescripcion.FechaCreacion = DateTime.UtcNow;
            prescripcion.Activa = true; // Por defecto al crear

            await this.context.Prescripciones.AddAsync(prescripcion);
            return await this.context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdatePrescripcionAsync(Prescripcion prescripcion)
        {
            Prescripcion storedPrescripcion = await this.context.Prescripciones.FirstOrDefaultAsync(p => p.IdPrescripcion == prescripcion.IdPrescripcion);

            if (storedPrescripcion != null)
            {
                storedPrescripcion.IdCita = prescripcion.IdCita;
                storedPrescripcion.IdMedicamento = prescripcion.IdMedicamento;
                storedPrescripcion.Instrucciones = prescripcion.Instrucciones;
                storedPrescripcion.FechaInicio = prescripcion.FechaInicio;
                storedPrescripcion.FechaFin = prescripcion.FechaFin;

                return await this.context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<bool> UpdateEstadoPrescripcionAsync(int id, bool nuevoEstado)
        {
            Prescripcion storedPrescripcion = await this.context.Prescripciones.FirstOrDefaultAsync(p => p.IdPrescripcion == id);

            if (storedPrescripcion != null)
            {
                storedPrescripcion.Activa = nuevoEstado;
                return await this.context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<bool> DeletePrescripcionAsync(int id)
        {
            Prescripcion prescripcion = await this.context.Prescripciones.FirstOrDefaultAsync(p => p.IdPrescripcion == id);

            if (prescripcion != null)
            {
                this.context.Prescripciones.Remove(prescripcion);
                return await this.context.SaveChangesAsync() > 0;
            }
            return false;
        }
    }
}