using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MageritHealthAPI.Models
{
    [Table("MG_MEDICIONES")]
    public class Medicion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdMedicion { get; set; }

        public int IdTipoMedicion { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal ValorMedicion { get; set; }

        public int IdAnalitica { get; set; }

        [JsonIgnore]
        [ForeignKey("IdTipoMedicion")]
        public TipoMedicion? TipoMedicion { get; set; }

        [JsonIgnore]
        [ForeignKey("IdAnalitica")]
        public Analitica? Analitica { get; set; }
    }
}