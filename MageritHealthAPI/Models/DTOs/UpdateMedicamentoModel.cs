namespace MageritHealthAPI.Models.DTOs
{
    public class UpdateMedicamentoModel
    {
        public int IdMedicamento { get; set; }
        public string NombreComercial { get; set; }
        public string PrincipioActivo { get; set; }
        public string Concentracion { get; set; }
        public string Formato { get; set; }
        public string Fabricante { get; set; }
    }
}
