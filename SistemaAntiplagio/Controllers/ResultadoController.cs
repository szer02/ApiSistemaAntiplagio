using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaAntiplagio.Models.Repositorio;
using SistemaAntiplagio.Models.ViewModel.DTOs;
using SistemaAntiplagio.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace SistemaAntiplagio.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ResultadoController : ControllerBase
    {
        private readonly DeteccaoPlagioService _deteccaoPlagioService;
        private readonly IResultadoRepositorio _resultadoRepositorio;

        public ResultadoController(DeteccaoPlagioService deteccaoPlagioService, IResultadoRepositorio resultadoRepositorio)
        {
            _deteccaoPlagioService = deteccaoPlagioService;
            _resultadoRepositorio = resultadoRepositorio;
        }

        // Endpoint para iniciar a detecção de plágio
        [HttpPost("detectar")]
        [SwaggerOperation(Summary = "Iniciar a detecção através do ID de uma atividade")]
        public async Task<IActionResult> DetectarPlagio(int atividadeId)
        {
            await _deteccaoPlagioService.ExecutarDeteccaoPlagio(atividadeId);
            return Ok("Detecção de plágio iniciada com sucesso para a atividade.");
        }

        // Endpoint para visualizar os resultados de uma atividade específica com limite de similaridade
        [HttpGet("atividade/{atividadeId}")]
        [SwaggerOperation(
            Summary = "Obtém os resultados de plágio de uma atividade",
            Description = "Retorna os resultados de plágio para uma atividade específica. Use o parâmetro `limiteSimilaridade` para filtrar os resultados com base em um limite de similaridade. Recomenda-se definir limites como 50, 75 ou 100. (Em consideração a sintaxes de cada linguagem)"
        )]
        public IActionResult ObterResultados(int atividadeId, [FromQuery] decimal? limiteSimilaridade)
        {
            // Busca os resultados no formato DTO para incluir os nomes dos arquivos
            var resultados = _resultadoRepositorio.BuscarPorAtividade(atividadeId);
            if (resultados == null || !resultados.Any())
            {
                return NotFound("Nenhum resultado encontrado para esta atividade.");
            }

            // Aplica a filtragem se o limite de similaridade for fornecido
            if (limiteSimilaridade.HasValue)
            {
                resultados = resultados.Where(r => r.PercentualSimilaridade >= limiteSimilaridade.Value);
            }

            return Ok(resultados); // Retorna os dados completos com os nomes dos arquivos
        }


          // Endpoint para deletar o histórico de resultados de uma atividade específica
        [HttpDelete("atividade/{atividadeId}")]
        [SwaggerOperation(Summary = "Limpa o histórico de resultados de uma atividade")]
        public IActionResult DeletarResultados(int atividadeId)
        {
            // Busca os resultados da atividade
            var resultados = _resultadoRepositorio.BuscarPorAtividade(atividadeId);
            if (resultados == null || !resultados.Any())
            {
                return NotFound("Nenhum resultado encontrado para esta atividade.");
            }

            // Remove todos os resultados da atividade
            foreach (var resultado in resultados)
            {
                _resultadoRepositorio.Excluir(resultado.Id);
            }

            return Ok("Histórico de resultados da atividade excluído com sucesso.");
        }

    }
}
