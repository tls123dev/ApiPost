using Mapster;
using Microsoft.AspNetCore.Authorization;
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

            // VALIDAR SI EXISTEN POST
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

            // VALIDAR SI EL USUARIO EXISTE
            if (user == null)
            {
                return NotFound($"Usuario con ID {userId} no encontrado");
            }

            // OBTENER POSTS POR USUARIO
            var posts = await _postRepository.GetPostByUserAsync(userId);

            // VALIDAR SI EL USUARIO TIENE POST O SI EXISTEN
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

            // CREAR POST
            await _postRepository.AddPostAsync(post);

            // MAPEAR ENTIDAD -> RESPONSE
            var response = post.Adapt<CreatePostResponse>();

            return CreatedAtAction(nameof(GetPostById), new { id = post.PostId }, response);
        }

        // DELETE PARA ELIMINAR POST
        [HttpDelete("{id}/user/{userId}")]
        public async Task<ActionResult> DeletePost(int id, int userId)
        {
            // VERIFICAR SI EL USER EXISTE
            var existingUser = await _userRepository.GetUserByIdAsync(userId);

            // VALIDACION PARA USER EXISTENTE
            if (existingUser == null)
            {
                return NotFound($"Usuario con ID {id} no existe o no coincide con el id del post indicado.");
            }

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

            // VALIDACION PARA POST EXISTENTES
            if (existingPost == null)
            {
                return NotFound($"Post con ID {id} no encontrado para actualizar.");
            }

            // MAPEAR REQUEST -> ENTIDAD
            request.Adapt(existingPost);

            // ACTUALIZAR FECHA DE ACTUALIZACION
            existingPost.UpdatedAt = DateTime.UtcNow;

            // ACTUALIZAR LOS CAMPOS DEL POST EXISTENTE
            var updatedPost = await _postRepository.SaveAllAsync();

            // VALIDACION PARA ACTUALIZACION
            if (!updatedPost)
            {
                return BadRequest("No se pudo actualizar el post.");
            }

            // MAPEAR ENTIDAD -> RESPONSE
            var response = existingPost.Adapt<UpdatePostResponse>();

            return Ok(response);
        }

        // GET PARA OBTENER POST POR ID
        [HttpGet("{id}")]
        public async Task<ActionResult<ReadPostResponse>> GetPostById(int id)
        {
            
            // OBTENER POST POR ID
            var post = await _postRepository.GetPostByIdAsync(id);

            // VALIDAR SI EL POST EXISTE
            if (post == null)
            {
                return NotFound($"Post con ID {id} no encontrado");
            }

            // MAPEAR ENTIDAD -> RESPONSE
            var response = post.Adapt<ReadPostResponse>();

            return Ok(response);
        }
    }
}
