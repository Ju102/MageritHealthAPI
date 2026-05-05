namespace MageritHealthAPI.Models.DTOs
{
    public class CreateUpdateTipoMedicionModel
    {
        public string NombreMedicion { get; set; } = null!;
        public string UnidadMedicion { get; set; } = null!;
        public decimal ValorMaximo { get; set; }
        public decimal ValorMinimo { get; set; }
    }
}
