using System.ComponentModel.DataAnnotations;

namespace MageritHealthAPI.Models.DTOs
{
    public class CreateDoctorModel : CreateUsuarioModel
    {
        [Required]
        public int IdEspecialidad { get; set; }

        [Required]
        public string NumeroColegiado { get; set; }
    }
}
