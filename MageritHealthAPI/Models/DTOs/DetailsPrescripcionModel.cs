namespace MageritHealthAPI.Models.DTOs
{
    public class DetailsPrescripcionModel
    {
        public int IdPrescripcion { get; set; }
        public int IdCita { get; set; }
        public int IdMedicamento { get; set; }

        public string NombreComercial { get; set; }
        public string PrincipioActivo { get; set; }
        public string Concentracion { get; set; }
        public string Instrucciones { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }
}
