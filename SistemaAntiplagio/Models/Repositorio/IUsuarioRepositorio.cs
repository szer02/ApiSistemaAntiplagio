using SistemaAntiplagio.Models.Repositorio.Entidades;

namespace SistemaAntiplagio.Models.Repositorio
{
    public interface IUsuarioRepositorio : IRepositorio<Usuario>
    {
        Usuario BuscarPorEmail(string email); // Método para buscar o usuário pelo e-mail
    }
}
