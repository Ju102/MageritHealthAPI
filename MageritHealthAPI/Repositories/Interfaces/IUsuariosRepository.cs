using MageritHealthAPI.Models;
using MageritHealthAPI.Models.DTOs;

namespace MageritHealthAPI.Repositories.Interfaces
{
    public interface IUsuariosRepository
    {
        // -- GETs --
        Task<List<Usuario>> GetUsuariosAsync(string? dni = null, string? rol = null, bool activos = true);
        Task<Usuario> FindUsuarioByIdAsync(int idUsuario);
        Task<Usuario> FindUsuarioByEmailAsync(string email);

        // Doctor
        Task<List<Usuario>> GetPacientesByIdDoctorAsync(int idDoctor);
        Task<List<Usuario>> GetDoctoresByEspecialidadAsync(int idEspecialidad);

        // -- Login --
        Task<Usuario> LoginUsuarioAsync(string email, string password);

        // -- INSERTs --
        Task<bool> InsertUsuarioAsync(Usuario usuario);

        // -- UPDATEs --
        Task<bool> UpdatePerfilUsuarioAsync(UpdatePerfilModel model);
        Task<bool> UpdateUsuarioAsync(Usuario user);

        Task<bool> UpdateUrlImagenUsuarioAsync(int idUsuario, string urlImagen);

        Task<bool> UpdatePasswordUsuarioAsync(int idUsuario, string oldPassword, string newPassword);
        Task<bool> ResetPasswordUsuarioAsync(int idUsuario);

        // -- DELETEs (logicos) --
        Task<bool> UpdateEstadoUsuarioAsync(int idUsuario, bool activo);
    }
}
