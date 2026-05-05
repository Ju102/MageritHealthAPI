using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MageritHealthAPI.Models
{
    [Table("MG_USUARIOS")]
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdUsuario { get; set; }

        [MaxLength(50)]
        public string Nombre { get; set; }

        [MaxLength(50)]
        public string Apellido1 { get; set; }

        [MaxLength(50)]
        public string? Apellido2 { get; set; }

        [MaxLength(2048)]
        public string? UrlImagen { get; set; }

        [MaxLength(20)]
        public string Dni { get; set; }

        public DateTime FechaNacimiento { get; set; }

        [MaxLength(12)]
        public string Telefono { get; set; }

        [MaxLength(20)]
        public string Genero { get; set; }

        [MaxLength(100)]
        public string Direccion { get; set; }

        [MaxLength(100)]
        public string Email { get; set; }

        [MaxLength(100)]
        public string Pass { get; set; }

        [MaxLength(10)]
        public string Rol { get; set; }

        [MaxLength(100)]
        public string? ContactoEmergenciaNombre { get; set; }

        [MaxLength(20)]
        public string? ContactoEmergenciaTelefono { get; set; }

        public int? IdEspecialidad { get; set; }

        [MaxLength(20)]
        public string? NumeroColegiado { get; set; }

        [MaxLength(20)]
        public string? NumeroAsegurado { get; set; }

        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        [ForeignKey("IdEspecialidad")]
        public Especialidad? Especialidad { get; set; }

        [JsonIgnore]
        public InfoClinicaPaciente? InfoClinica { get; set; }

        [JsonIgnore]
        public List<AntecedenteMedico> Antecedentes { get; set; } = new List<AntecedenteMedico>();

        [JsonIgnore]
        public Credencial? Credencial { get; set; }

        [JsonIgnore]
        [InverseProperty("Paciente")]
        public List<Cita> CitasComoPaciente { get; set; } = new List<Cita>();

        [JsonIgnore]
        [InverseProperty("Doctor")]
        public List<Cita> CitasComoDoctor { get; set; } = new List<Cita>();
    }
}