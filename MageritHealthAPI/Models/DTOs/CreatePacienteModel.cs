using System.ComponentModel.DataAnnotations;

namespace MageritHealthAPI.Models.DTOs
{
    public class CreatePacienteModel : CreateUsuarioModel
    {
        [Required]
        public string NumeroAsegurado { get; set; }
    }
}
