using Mapster;
using Microsoft.AspNetCore.Mvc;
using uwu.DTOs.Auth;
using uwu.DTOs.Users.CreateUsers;
using uwu.DTOs.Users.UpdateUsers;
using uwu.DTOs.Users.ChangeEmail;
using uwu.DTOs.Users.ChangePassword;
using uwu.Entities;
using uwu.Interfaces;
using Microsoft.AspNetCore.Authorization;

// USING PARA AUTENTICACION JWT
using Microsoft.IdentityModel.Tokens;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace uwu.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _config;
        public UsersController(IUserRepository userRepository, IConfiguration config)
        {
            _userRepository = userRepository;
            _config = config;
        }

        private string GenerateJwtToken(UserResponse user)
        {
            var jwtSettings = _config.GetSection("JwtSettings");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new System.Security.Claims.Claim("UserId", user.UserId.ToString()),
                new System.Security.Claims.Claim("Email", user.Email ?? string.Empty),
                new System.Security.Claims.Claim("Name", user.Name ?? string.Empty)
            };

            var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(jwtSettings["DurationInMinutes"]!)),
                signingCredentials: creds
            );

            return new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
        }

        // GET PARA OBTENER TODOS LOS USUARIOS
        [HttpGet]
        public async Task<ActionResult<List<User>>> GetUsers()
        {

            // OBTENER TODOS LOS USUARIOS
            var users = await _userRepository.GetUsers();

            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            // OBTENER USUARIO POR ID
            var user = await _userRepository.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound($"Usuario con ID {id} no encontrado");
            }

            return Ok(user);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUsers(int id)
        {
            // ELIMINA USUARIO POR ID
            var deleteUser = await _userRepository.DeleteUserAsync(id);

            // VALIDACION PARA USUARIO EXISTENTES
            if (!deleteUser)
            {
                return NotFound($"Usuario con ID {id} no encontrado");
            }

            return Ok("Usuario eliminado correctamente");
        }


        // PUT PARA ACTUALIZAR USUARIO
        [HttpPut("{id}")]
        public async Task<ActionResult<UpdateUserResponse>> UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {

            // VALIDACION PARA USUARIO EXISTENTES
            var existingUser = await _userRepository.GetUserByIdAsync(id);

            if (existingUser == null)
            {
                return NotFound($"Usuario con ID {id} no encontrado para actualizar.");
            }

            // VALIDACION PARA EMAIL
            if (await _userRepository.EmailExistsAsync(request.Email) && existingUser.Email != request.Email)
            {
                return BadRequest("El correo electrónico ya está en uso.");
            }

            // MAPEAR REQUEST -> ENTIDAD
            request.Adapt(existingUser);

            await _userRepository.SaveAllAsync();

            // MAPEAR ENTIDAD -> RESPONSE
            var response = existingUser.Adapt<UpdateUserResponse>();

            return Ok(response);
        }

        // POST PARA AGREGAR USUARIO
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> AddUser([FromBody] CreateUserRequest request)
        {

            // VALIDACION PARA EMAIL
            if (await _userRepository.EmailExistsAsync(request.Email))
            {
                return BadRequest("El correo electrónico ya está en uso.");
            }

            // MAPEAR REQUEST -> ENTIDAD
            var user = request.Adapt<User>();

            // ENCRIPTADO DE CONTRASEÑA
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            // GUARDADO DE USUARIO
            await _userRepository.AddUserAsync(user);

            // MAPEAR ENTIDAD -> RESPONSE
            var response = user.Adapt<CreateUserResponse>();

            return CreatedAtAction(nameof(GetUserById), new { id = user.UserId }, response);
        }


        // POST PARA LOGIN
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserRequest request)
        {
            //// PROCESO DE LOGIN
            var userLogin = await _userRepository.Login(request);

            //// VALIDACION DE LOGIN
            if (userLogin == null)
            {
                return Unauthorized("Email o contraseña incorrectos");
            }

            var token = GenerateJwtToken(userLogin);
            userLogin.Token = token;

            return Ok(userLogin);
        }

        [HttpPut("change-email/{userId}")]
        public async Task<IActionResult> ChangeEmail(int userId, [FromBody] ChangeEmailRequest request)
        {
            // VALIDACION PARA USUARIO EXISTENTES
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Usuario con ID {userId} no encontrado para cambiar el email.");
            }

            // VALIDACION PARA EMAIL
            if (await _userRepository.EmailExistsAsync(request.NewEmail) && user.Email != request.NewEmail)
            {
                return BadRequest("El correo electrónico ya está en uso.");
            }

            // VALIDACION PARA CAMBIO DE EMAIL
            var changeEmailResult = await _userRepository.ChangeEmailAsync(userId, request);

            if (!changeEmailResult)
            {
                return NotFound("Ha ocurrido un error al cambiar el email");
            }
            return Ok("Email actualizado correctamente");
        }

        [HttpPut("change-password/{userId}")]
        public async Task<IActionResult> ChangePassword(int userId, [FromBody] ChangePasswordRequest request)
        {

            // VALIDACION PARA USUARIO EXISTENTES
            var existingUser = await _userRepository.GetUserByIdAsync(userId);

            if (existingUser == null)
            {
                return NotFound($"Usuario con ID {userId} no encontrado para actualizar.");
            }


            // VALIDACION PARA CONTRASEÑA ACTUAL
            var changePasswordResult = await _userRepository.ChangePasswordAsync(userId, request);

            if (!changePasswordResult)
            {
                return BadRequest("Contraseña actual incorrecta");
            }

            return Ok("Contraseña actualizada correctamente");
        }

        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("No se pudo obtener el usuario");

            int userId = int.Parse(userIdClaim);

            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
                return NotFound("Usuario no encontrado");

            var response = user.Adapt<UserResponse>();

            return Ok(response);
        }
    }
}