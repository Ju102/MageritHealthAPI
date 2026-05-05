namespace MageritHealthAPI.Models.DTOs
{
    public class DetailsInfoClinicaModel
    {
        public int IdPaciente { get; set; }
        public string GrupoSanguineo { get; set; }
        public decimal? PesoActual { get; set; }
        public decimal? EstaturaActual { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}
