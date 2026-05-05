namespace MageritHealthAPI.Models.DTOs
{
    public class CreateCitaModel
    {
        public int IdPaciente { get; set; }
        public int IdEspecialidad { get; set; }
        public int IdDoctor { get; set; }
        public DateTime FechaHora { get; set; }
        public string Motivo { get; set; }
        public string? Notas { get; set; }
    }
}