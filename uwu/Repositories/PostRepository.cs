using Microsoft.EntityFrameworkCore;
using uwu.Data;
using uwu.Entities;
using uwu.Interfaces;
using Mapster;

namespace uwu.Repositories
{
    public class PostRepository : IPostRepository
    {
        // INJECCION DE DEPENDENCIAS
        private readonly Context _context;
        public PostRepository(Context context)
        {
            _context = context;
        }

        //------------------------------------------//
        // METODOS ASINCRONOS PARA OPERACIONES CRUD //
        //------------------------------------------//

        // METODO PARA AGREGAR UN POST
        public async Task AddPostAsync(Post post)
        {
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
        }

        // METODO PARA OBTENER TODOS LOS POSTS
        public async Task<List<Post>> GetPostsAsync()
        {
            var posts = await _context.Posts.Include(p => p.User).OrderByDescending(p => p.CreatedAt).ToListAsync();
            return posts;
        }

        // METODO PARA OBTENER POSTS POR USUARIO
        public async Task<List<Post>> GetPostByUserAsync(int userId)
        {
            var posts = await _context.Posts.Where(p => p.UserId == userId).Include(p => p.User).OrderByDescending(p => p.CreatedAt).ToListAsync();
            return posts;
        }

        // METODO PARA OBTENER POST POR ID
        public async Task<Post?> GetPostByIdAsync(int id)
        {
            return await _context.Posts.Include(p => p.User).FirstOrDefaultAsync(p => p.PostId == id);
        }

        // METODO PARA ELIMINAR POST
        public async Task<bool> DeletePostAsync(int id, int userId)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.PostId == id && p.UserId == userId);
            if (post == null)
            {
                return false;
            }
            _context.Posts.Remove(post);
            return await _context.SaveChangesAsync() > 0;
        }

        // METODO PARA GUARDAR CAMBIOS
        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
