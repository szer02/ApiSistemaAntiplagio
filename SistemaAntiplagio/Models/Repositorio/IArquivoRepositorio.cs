using SistemaAntiplagio.Models.Repositorio.Entidades;

namespace SistemaAntiplagio.Models.Repositorio
{
    public interface IArquivoRepositorio : IRepositorio<Arquivo>
    {
        IEnumerable<Arquivo> BuscarPorAtividadeId(int atividadeId); // Método para buscar arquivos por atividade
    }
}
