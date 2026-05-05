namespace MageritHealthAPI.Models.DTOs
{
    public class DetailsTipoMedicionModel
    {
        public int IdTipoMedicion { get; set; }
        public string NombreMedicion { get; set; } = null!;
        public string UnidadMedicion { get; set; } = null!;
        public decimal ValorMaximo { get; set; }
        public decimal ValorMinimo { get; set; }
        public bool Activo { get; set; }
    }
}
