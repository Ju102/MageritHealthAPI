namespace MageritHealthAPI.Models.DTOs
{
    public class ListAnaliticasModel
    {
        public int IdAnalitica { get; set; }
        
        public DateTime FechaHora { get; set; }

        public string NombrePaciente { get; set; }

        public string NombreDoctor { get; set; }

        public string Estado { get; set; }

        public string UrlAnalitica { get; set; }
    }
}
