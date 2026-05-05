namespace MageritHealthAPI.Models.DTOs
{
    public class DetailsMedicamentoModel
    {
        public int IdMedicamento { get; set; }
        public string NombreComercial { get; set; }
        public string Fabricante { get; set; }
        public string PrincipioActivo { get; set; }
        public string Concentracion { get; set; }
        public string Formato { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool Activo { get; set; }
    }
}
