using System.Collections.Generic;

namespace SistemaAntiplagio.Models.Repositorio
{
    public interface IRepositorio<TEntidade>
    {
        IEnumerable<TEntidade> BuscarTodos();
        TEntidade Buscar(int id);
        void Inserir(TEntidade model);
        void Alterar(TEntidade model);
        void Excluir(int id);
    }
}
