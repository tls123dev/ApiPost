using Microsoft.EntityFrameworkCore;
using uwu.Data;
using uwu.Entities;
using uwu.Interfaces;
using Mapster;
using uwu.DTOs.Auth;
using uwu.DTOs.Users.ChangePassword;
using uwu.DTOs.Users.ChangeEmail;

namespace uwu.Repositories
{
    public class UserRepository : IUserRepository
    {
        
        // INJECCION DE DEPENDENCIAS
        private readonly Context _context;
        public UserRepository(Context context)
        {
            _context = context;
        }

        // METODOS ASINCRONOS PARA OPERACIONES CRUD

        // METODO PARA AGREGAR UN USUARIO
        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        // METODO PARA OBTENER TODOS LOS USUARIOS
        public async Task<List<User>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return users;
        }

        // METODO PARA OBTENER UN USUARIO POR ID
        public async Task<User?> GetUserByIdAsync(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
            return user;
        }

        // METODO PARA ELIMINAR UN USUARIO POR ID
        public async Task<bool> DeleteUserAsync(int id)
        {
            var userToDelete = await GetUserByIdAsync(id);
            if (userToDelete == null)
            {
                return false;
            }
            _context.Users.Remove(userToDelete);
            await _context.SaveChangesAsync();

            return true;
        }

        // METODO PARA GUARDAR CAMBIOS
        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }


        // METODO PARA VERIFICAR SI UN EMAIL YA EXISTE
        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }


        // METODO PARA INICIAR SESION
        public async Task<UserResponse?> Login(UserRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            
            if (user == null)
            {
                return null;
            }

            bool okPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);

            if (!okPassword)
            {
                return null;
            }

            var response = user.Adapt<UserResponse>();

            return response;
        }


        // METODO PARA CAMBIAR CONTRASEÑA
        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request)
        {
            var user = await GetUserByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            bool okPassword = BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.Password);
            if (!okPassword)
            {
                return false;
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _context.SaveChangesAsync();
            return true;
        }

        // METODO PARA CAMBIAR EMAIL
        public async Task<bool> ChangeEmailAsync(int userId, ChangeEmailRequest request)
        {
            var user = await GetUserByIdAsync(userId);
            if (user == null)
            {
                return false;
            }
            user.Email = request.NewEmail;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
