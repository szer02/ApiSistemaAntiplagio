using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaAntiplagio.Models.Repositorio;
using SistemaAntiplagio.Models.Repositorio.Entidades;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.IO;

namespace SistemaAntiplagio.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ArquivoController : ControllerBase
    {
        private readonly IArquivoRepositorio _arquivoRepositorio;
        private readonly IAtividadeRepositorio _atividadeRepositorio;

        public ArquivoController(IArquivoRepositorio arquivoRepositorio, IAtividadeRepositorio atividadeRepositorio)
        {
            _arquivoRepositorio = arquivoRepositorio;
            _atividadeRepositorio = atividadeRepositorio;
        }

        // GET: api/arquivo
        [HttpGet]
        [SwaggerOperation(Summary = "Lista todos os arquivos anexados")]
        public IActionResult Get()
        {
            var arquivos = _arquivoRepositorio.BuscarTodos();
            return Ok(arquivos);
        }

        // GET: api/arquivo/{id}
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Busca um arquivo específico através de um ID")]
        public IActionResult Get(int id)
        {
            var arquivo = _arquivoRepositorio.Buscar(id);
            if (arquivo == null)
            {
                return NotFound();
            }
            return Ok(arquivo);
        }

        // POST: api/upload
        [HttpPost("upload")]
        [SwaggerOperation(Summary = "Anexa um arquivo")]
        public IActionResult Upload(int atividadeId, IFormFile arquivo)
        {
            var atividade = _atividadeRepositorio.Buscar(atividadeId);
            if (atividade == null)
            {
                return BadRequest("Atividade não encontrada.");
            }

            var caminhoPasta = Path.Combine("Uploads", "Atividades", atividadeId.ToString());
            Directory.CreateDirectory(caminhoPasta);

            // Gera um nome único para o arquivo no sistema
            var nomeUnico = Guid.NewGuid() + Path.GetExtension(arquivo.FileName);
            var caminhoArquivo = Path.Combine(caminhoPasta, nomeUnico);

            using (var stream = new FileStream(caminhoArquivo, FileMode.Create))
            {
                arquivo.CopyTo(stream);
            }

            var novoArquivo = new Arquivo
            {
                AtividadeId = atividadeId,
                NomeOriginal = arquivo.FileName, // Armazena o nome original
                CaminhoArquivo = caminhoArquivo,  // Caminho onde o arquivo foi salvo
                DataUpload = DateTime.Now
            };

            _arquivoRepositorio.Inserir(novoArquivo);
            return CreatedAtAction(nameof(Get), new { id = novoArquivo.Id }, novoArquivo);
        }



        // DELETE: api/arquivo/{id}
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Exclusão de um arquivo pelo ID")]
        public IActionResult Delete(int id)
        {
            var arquivo = _arquivoRepositorio.Buscar(id);
            if (arquivo == null)
            {
                return NotFound();
            }

            if (System.IO.File.Exists(arquivo.CaminhoArquivo))
            {
                System.IO.File.Delete(arquivo.CaminhoArquivo);
            }

            _arquivoRepositorio.Excluir(id);
            return NoContent();
        }
    }
}
