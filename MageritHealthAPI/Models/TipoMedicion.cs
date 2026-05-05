using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MageritHealthAPI.Models
{
    [Table("MG_TIPOS_MEDICIONES")]
    public class TipoMedicion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdTipoMedicion { get; set; }

        [MaxLength(50)]
        public string NombreMedicion { get; set; }

        [MaxLength(10)]
        public string UnidadMedicion { get; set; }

        [Column("ValorMaximo", TypeName = "decimal(10, 2)")]
        public decimal ValorMaximo { get; set; }

        [Column("ValorMinimo", TypeName = "decimal(10, 2)")]
        public decimal ValorMinimo { get; set; }

        public bool Activo { get; set; } = true;

        [JsonIgnore]
        public List<Medicion> Mediciones { get; set; } = new List<Medicion>();
    }
}