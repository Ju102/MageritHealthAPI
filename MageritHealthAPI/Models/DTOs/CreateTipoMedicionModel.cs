namespace MageritHealthAPI.Models.DTOs
{
    public class CreateTipoMedicionModel
    {
        public string NombreMedicion { get; set; }

        public string UnidadMedicion { get; set; }

        public decimal ValorMaximo { get; set; }

        public decimal ValorMinimo { get; set; }
    }
}
