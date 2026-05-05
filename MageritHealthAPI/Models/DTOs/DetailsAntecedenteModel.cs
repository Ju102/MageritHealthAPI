namespace MageritHealthAPI.Models.DTOs
{
    public class DetailsAntecedenteModel
    {
        public int IdAntecedente { get; set; }
        public string Tipo { get; set; }
        public string Nombre { get; set; }
        public string? Severidad { get; set; }
        public DateTime? FechaDiagnostico { get; set; }
        public string? Notas { get; set; }
        public bool Activo { get; set; }
    }
}
