namespace MageritHealthAPI.Models.DTOs
{
    public class UpdatePerfilModel
    {
        public int IdUsuario { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public string? OldPassword { get; set; }
        public string? NewPassword { get; set; }
    }
}
