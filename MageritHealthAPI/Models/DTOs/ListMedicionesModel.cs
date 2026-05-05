namespace MageritHealthAPI.Models.DTOs
{
    public class ListMedicionesModel
    {
        public string NombreMedicion { get; set; }
        public string UnidadMedicion { get; set; }
        public decimal ValorMaximo { get; set; }
        public decimal ValorMinimo { get; set; }
        public decimal ValorMedicion { get; set; }
    }
}
