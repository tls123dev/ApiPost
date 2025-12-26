using uwu.Entities;

namespace uwu.Interfaces
{
    public interface IPostRepository
    {
        // METODOS CRUD

        // METODO PARA OBTENER TODOS LOS POSTS
        Task<List<Post>> GetPostsAsync();

        // METODO PARA OBTENER POST POR USUARIO
        Task<List<Post>> GetPostByUserAsync(int userId);

        // METODO PARA OBTENER POST POR USUARIO
        Task<Post?> GetPostByIdAsync(int id);

        // METODO PARA ELIMINAR POST
        Task<bool> DeletePostAsync(int id, int userId);

        // METODO PARA AÑADIR POSTs
        Task AddPostAsync(Post post);

        // METODO PARA GUARDAR CAMBIOS
        Task<bool> SaveAllAsync();
    }
}
