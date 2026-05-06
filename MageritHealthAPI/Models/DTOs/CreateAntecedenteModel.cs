namespace MageritHealthAPI.Models.DTOs
{
    public class CreateAntecedenteModel
    {
        public int? IdCita { get; set; }
        public int IdPaciente { get; set; }
        public string Tipo { get; set; }
        public string Nombre { get; set; }
        public string? Severidad { get; set; }
        public DateTime? FechaDiagnostico { get; set; }
        public string? Notas { get; set; }
    }
}
