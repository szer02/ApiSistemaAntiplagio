using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaAntiplagio.Models.Repositorio;
using SistemaAntiplagio.Models.Repositorio.Entidades;
using Swashbuckle.AspNetCore.Annotations;
using System.IO;
using System.Security.Claims;

namespace SistemaAntiplagio.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AtividadeController : ControllerBase
    {
        private readonly IAtividadeRepositorio _atividadeRepositorio;
        private readonly IArquivoRepositorio _arquivoRepositorio;

        public AtividadeController(IAtividadeRepositorio atividadeRepositorio, IArquivoRepositorio arquivoRepositorio)
        {
            _atividadeRepositorio = atividadeRepositorio;
            _arquivoRepositorio = arquivoRepositorio;
        }

        // GET: api/atividade
        [HttpGet]
        [SwaggerOperation(Summary = "Lista todas as atividades criadas")]
        public IActionResult Get()
        {
            var atividades = _atividadeRepositorio.BuscarTodos();
            return Ok(atividades);
        }

        // GET: api/atividade/{id}
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Busca uma atividade específica através de um ID")]
        public IActionResult Get(int id)
        {
            var atividade = _atividadeRepositorio.Buscar(id);
            if (atividade == null)
            {
                return NotFound();
            }
            return Ok(atividade);
        }

        // POST: api/atividade
        [HttpPost]
        [SwaggerOperation(Summary = "Cadastra uma atividade")]
        public IActionResult Post([FromBody] Atividade atividade)
        {
            // Recupera o Id do usuário autenticado do token JWT
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (usuarioIdClaim == null)
            {
                return Unauthorized("Usuário não autenticado");
            }

            // Converte o Id recuperado para int e associa à atividade
            atividade.UsuarioId = int.Parse(usuarioIdClaim.Value);

            // Insere a atividade com o UsuarioId obtido do token
            _atividadeRepositorio.Inserir(atividade);
            return CreatedAtAction(nameof(Get), new { id = atividade.Id }, atividade);
        }

        // PUT: api/atividade/{id}
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Atualiza uma atividade específica através de um ID")]
        public IActionResult Put(int id, [FromBody] Atividade atividade)
        {
            var atividadeExistente = _atividadeRepositorio.Buscar(id);
            if (atividadeExistente == null)
            {
                return NotFound();
            }
            atividade.Id = id; // Garante que o ID está correto para a atualização
            _atividadeRepositorio.Alterar(atividade);
            return NoContent();
        }

        // DELETE: api/atividade/{id}
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Exclusão de uma atividade pelo ID")]
        public IActionResult Delete(int id)
        {
            var atividade = _atividadeRepositorio.Buscar(id);
            if (atividade == null)
            {
                return NotFound();
            }

            // Busca todos os arquivos associados à atividade
            var arquivos = _arquivoRepositorio.BuscarPorAtividadeId(id);
            if (arquivos != null)
            {
                // Deleta cada arquivo físico e remove o registro do banco
                foreach (var arquivo in arquivos)
                {
                    if (System.IO.File.Exists(arquivo.CaminhoArquivo))
                    {
                        System.IO.File.Delete(arquivo.CaminhoArquivo);
                    }
                    _arquivoRepositorio.Excluir(arquivo.Id);
                }

                // Remove a pasta da atividade se estiver vazia
                var caminhoPasta = Path.Combine("Uploads", "Atividades", id.ToString());
                if (Directory.Exists(caminhoPasta) && Directory.GetFiles(caminhoPasta).Length == 0)
                {
                    Directory.Delete(caminhoPasta);
                }
            }

            // Exclui a atividade do banco de dados
            _atividadeRepositorio.Excluir(id);
            return NoContent();
        }
    }
}
