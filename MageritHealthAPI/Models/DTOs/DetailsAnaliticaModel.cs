namespace MageritHealthAPI.Models.DTOs
{
    public class DetailsAnaliticaModel
    {
        public int IdAnalitica { get; set; }

        public DateTime FechaHora { get; set; }

        public string Notas { get; set; }

        public string Estado { get; set; }

        public string UrlAnalitica { get; set; }

        public int IdCita { get; set; }
        public string MotivoCita { get; set; }

        public string NombrePaciente { get; set; }
        public string DniPaciente { get; set; }

        public string NombreDoctor { get; set; }
        public string NumeroColegiado { get; set; }

    }
}
