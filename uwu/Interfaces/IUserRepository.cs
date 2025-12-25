using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using uwu.DTOs.Auth;
using uwu.DTOs.Users.ChangeEmail;
using uwu.DTOs.Users.ChangePassword;
using uwu.Entities;

namespace uwu.Interfaces
{
    public interface IUserRepository
    {
        // METODOS CRUD

        // METODO PARA OBTENER TODOS LOS USUARIOS
        Task<List<User>> GetUsers();

        // METODO PARA OBTENER USUARIO POR ID
        Task<User?> GetUserByIdAsync(int id);

        // METODO PARA ELIMINAR USUARIO
        Task<bool> DeleteUserAsync(int id);

        // METODO PARA ACTUALIZAR USUARIO
        Task<User> UpdateUserAsync(User user);

        // METODO PARA AÑADIR USUARIO
        Task AddUserAsync(User user);

        // METODO PARA GUARDAR CAMBIOS
        Task<bool> SaveAllAsync();

        // METODO PARA INICIAR SESION
        Task<UserResponse?> Login(UserRequest request);
        
        // METODO PARA VERIFICAR EMAIL
        Task<bool> EmailExistsAsync(string email);

        // METODO PARA CAMBIAR CONTRASEÑA
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request);

        // METODO PARA CAMBIAR EMAIL
        Task<bool> ChangeEmailAsync(int userId, ChangeEmailRequest request);
    }
}
