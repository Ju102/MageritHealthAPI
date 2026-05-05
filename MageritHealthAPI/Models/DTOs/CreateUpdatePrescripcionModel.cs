namespace MageritHealthAPI.Models.DTOs
{
    public class CreateUpdatePrescripcionModel
    {
        public int IdCita { get; set; }

        public int IdMedicamento { get; set; }

        public string Instrucciones { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }
    }
}
