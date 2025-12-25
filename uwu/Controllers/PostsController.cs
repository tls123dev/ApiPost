using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using uwu.DTOs.Posts.CreatePosts;
using uwu.Interfaces;
using uwu.Entities;
using uwu.DTOs.Posts.UpdatePosts;
using uwu.DTOs.Posts.ReadPosts;

namespace uwu.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PostsController : Controller
    {

        private readonly IPostRepository _postRepository;
        private readonly IUserRepository _userRepository;
        public PostsController(IPostRepository postRepository, IUserRepository userRepository)
        {
            _postRepository = postRepository;
            _userRepository = userRepository;
        }

        // GET PARA OBTENER TODOS LOS POSTS
        [HttpGet]
        public async Task<ActionResult<List<ReadPostResponse>>> GetPosts()
        {
            // OBTENER TODOS LOS POSTS
            var posts = await _postRepository.GetPostsAsync();

            if (posts == null || posts.Count == 0)
            {
                return NotFound("No se encontraron posts");
            }

            // MAPEAR ENTIDAD -> RESPONSE
            var response = posts.Adapt<List<ReadPostResponse>>();

            return Ok(response);
        }

        // GET PARA OBTENER POSTS POR USUARIO
        [HttpGet]
        [Route("user/{userId}")]
        public async Task<ActionResult<List<ReadPostResponse>>> GetPostsByUser(int userId)
        {
            // VERIFICAR SI EL USUARIO EXISTE
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                return NotFound($"Usuario con ID {userId} no encontrado");
            }

            // OBTENER POSTS POR USUARIO
            var posts = await _postRepository.GetPostByUserAsync(userId);

            if (posts == null || posts.Count == 0)
            {
                return NotFound($"No se encontraron posts para el usuario con ID {userId}");
            }

            var response = posts.Adapt<List<ReadPostResponse>>();

            return Ok(response);
        }

        // POST PARA AGREGAR POST
        [HttpPost]
        public async Task<ActionResult<CreatePostRequest>> CreatePost([FromBody] CreatePostRequest request)
        {
            // OBTENER EL ID DEL USUARIO DESDE EL TOKEN
            var user = User.FindFirst("UserId")?.Value;

            // VALIDAR SI EL USUARIO ESTA REGISTRADO
            if (user == null)
            {
                return Unauthorized("Usuario no registrado");
            }

            // CONVERTIR EL ID DEL USUARIO A ENTERO
            int userId = int.Parse(user);

            // MAPEAR REQUEST -> ENTIDAD
            var post = request.Adapt<Post>();

            // ASIGNAR EL ID DEL USUARIO AL POST
            post.UserId = userId;
            // ASIGNAR FECHA DE CREACION
            post.CreatedAt = DateTime.UtcNow;
            // INICIALIZAR FECHA DE ACTUALIZACION COMO NULL
            post.UpdatedAt = DateTime.UtcNow;

            // GUARDAR POST
            await _postRepository.AddPostAsync(post);

            // MAPEAR ENTIDAD -> RESPONSE
            var response = post.Adapt<CreatePostResponse>();

            // ASIGNAR NOMBRE DE USUARIO AL RESPONSE
            response.UserName = User.FindFirst("Name")?.Value;

            return CreatedAtAction(nameof(GetPostById), new { id = post.PostId }, response);
        }

        // DELETE PARA ELIMINAR POST
        [HttpDelete("{id}/user/{userId}")]
        public async Task<ActionResult> DeletePost(int id, int userId)
        {
            // ELIMINA POST POR ID Y USUARIO
            var deletePost = await _postRepository.DeletePostAsync(id, userId);

            // VALIDAR SI EL POST FUE ENCONTRADO Y ELIMINADO
            if (!deletePost)
            {
                return NotFound($"Post con id {id} no existe o ya se ha eliminado");
            }

            return Ok("Post eliminado correctamente");
        }

        // PUT PARA ACTUALIZAR POST
        [HttpPut("{id}")]
        public async Task<ActionResult<UpdatePostResponse>> UpdatePost(int id, [FromBody] UpdatePostRequest request)
        {
            // VERIFICAR SI EL POST EXISTE
            var existingPost = await _postRepository.GetPostByIdAsync(id);
            if (existingPost == null)
            {
                return NotFound($"Post con ID {id} no encontrado para actualizar.");
            }

            // ACTUALIZAR LOS CAMPOS DEL POST EXISTENTE
            var updatedPost = await _postRepository.UpdatePostAsync(existingPost);
            if (updatedPost == null)
            {
                return BadRequest("No se pudo actualizar el post.");
            }

            // MAPEAR REQUEST -> ENTIDAD
            request.Adapt(updatedPost);

            await _postRepository.SaveAllAsync();

            // MAPEAR ENTIDAD -> RESPONSE
            var response = updatedPost.Adapt<UpdatePostResponse>();

            return Ok(response);
        }

        // GET PARA OBTENER POST POR ID
        [HttpGet("{id}")]
        public async Task<ActionResult<ReadPostResponse>> GetPostById(int id)
        {
            
            // OBTENER POST POR ID
            var post = await _postRepository.GetPostByIdAsync(id);

            if (post == null)
            {
                return NotFound($"Post con ID {id} no encontrado");
            }

            var response = post.Adapt<ReadPostResponse>();

            response.UserName = post.User?.Name;
            response.UserEmail = post.User?.Email;

            return Ok(response);
        }
    }
}
