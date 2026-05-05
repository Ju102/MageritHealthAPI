namespace MageritHealthAPI.Models.DTOs
{
    public class DetailsCitaModel
    {
        public int IdCita { get; set; }
        public string Motivo { get; set; }
        public DateTime FechaHora { get; set; }
        public string? Notas { get; set; }
        public string Estado { get; set; }

        // Datos del Doctor
        public string NombreDoctor { get; set; }
        public string Especialidad { get; set; }
        public string? NumeroColegiado { get; set; }

        // Datos del Paciente
        public int IdPaciente { get; set; }
        public string NombrePaciente { get; set; }
        public string DniPaciente { get; set; }
    }
}