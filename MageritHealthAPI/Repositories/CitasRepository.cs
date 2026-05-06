using MageritHealthAPI.Data;
using MageritHealthAPI.Models;
using MageritHealthAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MageritHealthAPI.Repositories
{
    public class CitasRepository : ICitasRepository
    {
        private readonly MageritHealthDbContext context;

        public CitasRepository(MageritHealthDbContext context)
        {
            this.context = context;
        }

        public async Task<List<Cita>> GetCitasAsync()
        {
            return await this.context.Citas.ToListAsync();
        }

        public async Task<Cita> FindCitaByIdAsync(int idCita)
        {
            return await this.context.Citas.Include(c => c.Paciente).Include(c => c.Doctor).FirstOrDefaultAsync(c => c.IdCita == idCita);
        }

        public async Task<List<Cita>> GetHistorialCitasByIdDoctorAsync(int idDoctor)
        {
            return await this.context.Citas
                .Include(c => c.Paciente).Include(c => c.Doctor)
                .AsNoTracking()
                .Where(c => c.IdDoctor == idDoctor && c.FechaHora <= DateTime.UtcNow && !c.Activa)
                .OrderByDescending(c => c.FechaHora).ToListAsync();
        }

        public async Task<List<Cita>> GetProximasCitasByIdDoctorAsync(int idDoctor, DateTime? fecha)
        {
            var query = this.context.Citas
                .Include(c => c.Paciente)
                .Include(c => c.Doctor)
                .AsNoTracking()
                .Where(c => c.IdDoctor == idDoctor && c.Activa == true);

            if (fecha.HasValue)
            {
                query = query.Where(c => c.FechaHora.Date == fecha.Value.Date);
            }
            else
            {
                query = query.Where(c => c.FechaHora >= DateTime.Today);
            }

            return await query.OrderBy(c => c.FechaHora).ToListAsync();
        }

        public async Task<List<Cita>> GetHistorialCitasByIdPacienteAsync(int idPaciente)
        {
            return await this.context.Citas
                .Include(c => c.Paciente).Include(c => c.Doctor)
                .AsNoTracking()
                .Where(c => c.IdPaciente == idPaciente && c.FechaHora <= DateTime.UtcNow && !c.Activa)
                .OrderByDescending(c => c.FechaHora).ToListAsync();
        }

        public async Task<List<Cita>> GetProximasCitasByIdPacienteAsync(int idPaciente, DateTime? fecha)
        {
            var query = this.context.Citas
                .Include(c => c.Paciente)
                .Include(c => c.Doctor)
                .AsNoTracking()
                .Where(c => c.IdPaciente == idPaciente && c.Activa == true);

            if (fecha.HasValue)
            {
                query = query.Where(c => c.FechaHora.Date == fecha.Value.Date);
            }
            else
            {
                query = query.Where(c => c.FechaHora >= DateTime.Today);
            }

            return await query.OrderBy(c => c.FechaHora).ToListAsync();
        }

        // --- ESCRITURA (POST / PUT) ---

        public async Task<bool> CreateCitaAsync(Cita cita)
        {
            // Prevenir el Double-Booking
            bool huecoOcupado = await this.context.Citas.AnyAsync(c =>
                c.IdDoctor == cita.IdDoctor &&
                c.FechaHora == cita.FechaHora &&
                c.Activa == true);

            // Cambiado a InvalidOperationException para capturarlo mejor en el controller
            if (huecoOcupado) throw new InvalidOperationException("El horario seleccionado ya no está disponible.");

            int maxIdCita = await this.context.Citas.MaxAsync(c => (int?)c.IdCita) ?? 0;
            cita.IdCita = maxIdCita + 1;
            cita.FechaCreacion = DateTime.UtcNow;

            await this.context.Citas.AddAsync(cita);
            return await this.context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateCitaAsync(Cita updatedCita)
        {
            Cita cita = await this.context.Citas.FirstOrDefaultAsync(c => c.IdCita == updatedCita.IdCita);

            if (cita != null)
            {
                cita.FechaHora = updatedCita.FechaHora;
                cita.Motivo = updatedCita.Motivo;
                cita.Notas = updatedCita.Notas;

                return await this.context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<bool> UpdateEstadoCitaAsync(int idCita, string estado, bool activa)
        {
            Cita cita = await this.context.Citas.FirstOrDefaultAsync(c => c.IdCita == idCita);

            if (cita != null)
            {
                cita.Estado = estado;
                cita.Activa = activa;

                return await this.context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<List<string>> GetHorasDisponiblesDoctorAsync(int idDoctor, DateTime fechaElegida)
        {
            var citasOcupadas = await this.context.Citas
                .Where(c => c.IdDoctor == idDoctor
                         && c.FechaHora.Date == fechaElegida.Date
                         && c.Estado != "cancelada")
                .Select(c => c.FechaHora.ToString("HH:mm"))
                .ToListAsync();

            List<string> todasLasHoras = new List<string>
            {
                "09:00", "09:30", "10:00", "10:30",
                "11:00", "11:30", "12:00", "12:30",
                "13:00", "13:30", "14:00", "14:30"
            };

            if (fechaElegida.Date == DateTime.Now.Date)
            {
                string horaActual = DateTime.Now.ToString("HH:mm");
                todasLasHoras = todasLasHoras.Where(h => string.Compare(h, horaActual) > 0).ToList();
            }

            return todasLasHoras.Except(citasOcupadas).ToList();
        }
    }
}