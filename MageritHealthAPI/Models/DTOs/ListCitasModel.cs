namespace MageritHealthAPI.Models.DTOs
{
    public class ListCitasModel
    {
        public int IdCita { get; set; }

        public string Motivo { get; set; }

        public DateTime FechaHora { get; set; }

        public string Estado { get; set; } // "programada", "completada", "cancelada", "progreso"

        // -- Datos que usará la vista del Paciente --
        public string NombreDoctor { get; set; }
        public string EspecialidadDoctor { get; set; }

        // -- Datos que usará la vista del Doctor --
        public string NombrePaciente { get; set; }
        public string DniPaciente { get; set; }

        public string? UrlCita { get; set; }
    }
}