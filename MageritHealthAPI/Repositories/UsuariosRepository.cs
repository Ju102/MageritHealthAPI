using MageritHealthAPI.Models;
using MageritHealthAPI.Data;
using MageritHealthAPI.Helpers;
using MageritHealthAPI.Models.DTOs;
using MageritHealthAPI.Repositories.Interfaces;
using MageritHealthAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MageritHealthAPI.Repositories
{
    public class UsuariosRepository : IUsuariosRepository
    {
        private readonly MageritHealthDbContext context;
        private readonly IEmailingService emailingService;

        public UsuariosRepository(MageritHealthDbContext context, IEmailingService emailingService)
        {
            this.context = context;
            this.emailingService = emailingService;
        }

        public async Task<List<Usuario>> GetUsuariosAsync(string? dni = null, string? rol = null, bool activos = true)
        {
            var query = this.context.Usuarios
                .Include(u => u.Especialidad)
                .AsNoTracking()
                .AsQueryable();

            if (activos) query = query.Where(u => u.Activo == true);
            if (!string.IsNullOrWhiteSpace(dni)) query = query.Where(u => u.Dni.Contains(dni));
            if (!string.IsNullOrWhiteSpace(rol)) query = query.Where(u => u.Rol == rol);

            return await query.ToListAsync();
        }

        public async Task<Usuario> FindUsuarioByIdAsync(int idUsuario)
        {
            return await this.context.Usuarios
                .Include(u => u.Especialidad)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.IdUsuario == idUsuario);
        }

        public async Task<List<Usuario>> GetPacientesByIdDoctorAsync(int idDoctor)
        {
            return await this.context.Usuarios
                .AsNoTracking()
                .Where(u => u.Rol == "paciente" && u.CitasComoPaciente.Any(c => c.IdDoctor == idDoctor))
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<Usuario>> GetDoctoresByEspecialidadAsync(int idEspecialidad)
        {
            return await this.context.Usuarios
                .AsNoTracking()
                .Where(u => u.IdEspecialidad == idEspecialidad)
                .ToListAsync();
        }

        public async Task<Usuario> LoginUsuarioAsync(string email, string password)
        {
            Usuario user = await this.context.Usuarios
                .Include(u => u.Especialidad)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null || !user.Activo) return null;

            Credencial credencial = await this.context.Credenciales.FirstOrDefaultAsync(c => c.IdUsuario == user.IdUsuario);
            if (credencial == null) return null;

            byte[] hashedPassword = CryptographyHelper.EncryptPassword(password, credencial.Salt);

            if (ToolsHelper.CompareArrays(hashedPassword, credencial.PasswordHash))
                return user;

            return null;
        }

        public async Task<bool> InsertUsuarioAsync(Usuario usuario)
        {
            int lastIdUsuario = await this.context.Usuarios.MaxAsync(u => (int?)u.IdUsuario) ?? 0;
            usuario.IdUsuario = lastIdUsuario + 1;

            int lastIdCredencial = await this.context.Credenciales.MaxAsync(c => (int?)c.IdCredencial) ?? 0;

            string password = ToolsHelper.GenerateRandomPassword();
            string salt = ToolsHelper.GenerateSalt();
            byte[] passwordHash = CryptographyHelper.EncryptPassword(password, salt);

            Credencial credencial = new Credencial()
            {
                IdCredencial = lastIdCredencial + 1,
                IdUsuario = usuario.IdUsuario,
                PasswordHash = passwordHash,
                Salt = salt
            };

            usuario.Pass = password; // Temporal

            await this.context.Usuarios.AddAsync(usuario);
            await this.context.Credenciales.AddAsync(credencial);

            bool success = await this.context.SaveChangesAsync() > 0;

            if (success)
            {
                await this.emailingService.SendEmailBienvenidaAsync(usuario.Email, password, usuario.Nombre, usuario.Rol);
            }

            return success;
        }

        public async Task<bool> UpdateUsuarioAsync(Usuario user)
        {
            Usuario storedUsuario = await this.context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == user.IdUsuario);

            if (storedUsuario == null) return false;

            storedUsuario.Nombre = user.Nombre;
            storedUsuario.Apellido1 = user.Apellido1;
            storedUsuario.Apellido2 = user.Apellido2;
            storedUsuario.UrlImagen = user.UrlImagen;
            storedUsuario.Dni = user.Dni;
            storedUsuario.FechaNacimiento = user.FechaNacimiento;
            storedUsuario.Telefono = user.Telefono;
            storedUsuario.Genero = user.Genero;
            storedUsuario.Direccion = user.Direccion;
            storedUsuario.Email = user.Email;
            storedUsuario.Rol = user.Rol;
            storedUsuario.ContactoEmergenciaNombre = user.ContactoEmergenciaNombre;
            storedUsuario.ContactoEmergenciaTelefono = user.ContactoEmergenciaTelefono;
            storedUsuario.IdEspecialidad = user.IdEspecialidad;
            storedUsuario.NumeroColegiado = user.NumeroColegiado;
            storedUsuario.NumeroAsegurado = user.NumeroAsegurado;

            return await this.context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdatePerfilUsuarioAsync(UpdatePerfilModel model)
        {
            Usuario usuario = await this.context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == model.IdUsuario);
            if (usuario == null) return false;

            bool hasChanges = false;

            if (!string.IsNullOrEmpty(model.OldPassword) && !string.IsNullOrEmpty(model.NewPassword))
            {
                bool passResult = await this.UpdatePasswordUsuarioAsync(model.IdUsuario, model.OldPassword, model.NewPassword);

                if (!passResult)
                {
                    return false;
                }
                hasChanges = true;
            }

            if (!string.IsNullOrWhiteSpace(model.Telefono) && usuario.Telefono != model.Telefono)
            {
                usuario.Telefono = model.Telefono;
                hasChanges = true;
            }

            if (!string.IsNullOrWhiteSpace(model.Email) && usuario.Email != model.Email)
            {
                usuario.Email = model.Email;
                hasChanges = true;
            }

            if (!hasChanges) return true;

            return await this.context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateUrlImagenUsuarioAsync(int idUsuario, string urlImagen)
        {
            Usuario usuario = await this.context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == idUsuario);

            if (usuario != null)
            {
                usuario.UrlImagen = urlImagen;
                return await this.context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<bool> UpdatePasswordUsuarioAsync(int idUsuario, string oldPassword, string newPassword)
        {
            Credencial credencial = await this.context.Credenciales.FirstOrDefaultAsync(c => c.IdUsuario == idUsuario);
            if (credencial == null) return false;

            byte[] oldHash = CryptographyHelper.EncryptPassword(oldPassword, credencial.Salt);

            if (ToolsHelper.CompareArrays(oldHash, credencial.PasswordHash))
            {
                string newSalt = ToolsHelper.GenerateSalt();
                credencial.PasswordHash = CryptographyHelper.EncryptPassword(newPassword, newSalt);
                credencial.Salt = newSalt;

                Usuario usuario = await this.context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == idUsuario);
                if (usuario != null) usuario.Pass = newPassword;

                return true;
            }
            return false;
        }

        public async Task<bool> ResetPasswordUsuarioAsync(int idUsuario)
        {
            Credencial credencial = await this.context.Credenciales.FirstOrDefaultAsync(c => c.IdUsuario == idUsuario);
            Usuario usuario = await this.context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == idUsuario);

            if (credencial == null || usuario == null) return false;

            string newPassword = ToolsHelper.GenerateRandomPassword();
            usuario.Pass = newPassword; // Temporal

            string salt = ToolsHelper.GenerateSalt();
            credencial.PasswordHash = CryptographyHelper.EncryptPassword(newPassword, salt);
            credencial.Salt = salt;

            bool success = await this.context.SaveChangesAsync() > 0;

            if (success)
            {
                await this.emailingService.SendEmailRecuperacionAsync(usuario.Email, newPassword, usuario.Nombre);
            }

            return success;
        }

        public async Task<bool> UpdateEstadoUsuarioAsync(int idUsuario, bool activo)
        {
            Usuario usuario = await this.context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == idUsuario);
            if (usuario == null) return false;

            usuario.Activo = activo;
            return await this.context.SaveChangesAsync() > 0;
        }

        public async Task<Usuario> FindUsuarioByEmailAsync(string email)
        {
            return await this.context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}