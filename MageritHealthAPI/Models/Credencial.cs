using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MageritHealthAPI.Models
{
    [Table("MG_CREDENCIALES")]
    public class Credencial
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdCredencial { get; set; }

        public int IdUsuario { get; set; }

        public byte[] PasswordHash { get; set; }

        [MaxLength(50)]
        public string Salt { get; set; }

        [JsonIgnore]
        [ForeignKey("IdUsuario")]
        public Usuario? Usuario { get; set; }
    }
}