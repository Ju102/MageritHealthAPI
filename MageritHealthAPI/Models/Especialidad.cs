using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MageritHealthAPI.Models
{
    [Table("MG_ESPECIALIDADES")]
    public class Especialidad
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdEspecialidad { get; set; }

        [MaxLength(50)]
        public string NombreEspecialidad { get; set; }

        [MaxLength(10)]
        public string Tipo { get; set; }

        [JsonIgnore]
        public List<Usuario> Doctores { get; set; } = new List<Usuario>();
    }
}