namespace MageritHealthAPI.Models.DTOs
{
    public class CreateUpdateInfoClinicaModel
    {
        public int IdPaciente { get; set; }
        public string GrupoSanguineo { get; set; }
        public decimal? PesoActual { get; set; }
        public decimal? EstaturaActual { get; set; }
    }
}
