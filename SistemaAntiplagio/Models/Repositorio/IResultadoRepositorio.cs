using SistemaAntiplagio.Models.Repositorio.Entidades;
using SistemaAntiplagio.Models.ViewModel.DTOs;
using System.Collections.Generic;

namespace SistemaAntiplagio.Models.Repositorio
{
    public interface IResultadoRepositorio : IRepositorio<Resultado>
    {
        IEnumerable<ResultadoDto> BuscarPorAtividade(int atividadeId); // Método específico para resultados de uma atividade
       // ResultadoDto BuscarResultadoRecente(int atividadeId); // Método para resultado mais recente
       
    }
}
