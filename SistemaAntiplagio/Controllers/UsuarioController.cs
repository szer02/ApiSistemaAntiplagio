using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaAntiplagio.Models.ViewModel;
using SistemaAntiplagio.Models.Repositorio;
using SistemaAntiplagio.Models.Repositorio.Entidades;
using System;
using System.Security.Claims;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.JsonPatch;
using Swashbuckle.AspNetCore.Annotations;


namespace SistemaAntiplagio.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly string _secretKey = "sistema_antiplagio_tccsantiago2024!#20181021"; // Substitua por uma chave segura

        public UsuarioController(IUsuarioRepositorio usuarioRepositorio)
        {
            _usuarioRepositorio = usuarioRepositorio;
        }

        // Método auxiliar para verificar se o usuário é administrador
        private bool UsuarioEhAdmin()
        {
            var emailUsuario = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
            var emailsAutorizados = new List<string> { "admin@gmail.com" }; // Lista de emails de administradores
            return emailsAutorizados.Contains(emailUsuario);
        }

        // GET: api/usuario
        [HttpGet]
        [SwaggerOperation(Summary = "Lista todo os usuários cadastrados")]
        public IActionResult Get()
        {   
            // Este trecho foi usado para verificar o Type do Email, pois "email" não dava certo kkkkkk então tivemos que colocar "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress".
            // var claims = User.Claims.Select(c => $"{c.Type}: {c.Value}").ToList();
            // Console.WriteLine("Claims do usuário autenticado:");
            // claims.ForEach(Console.WriteLine);

            if (!UsuarioEhAdmin())
            {
                return StatusCode(StatusCodes.Status403Forbidden, "Acesso restrito aos administradores.");
            }

            var usuarios = _usuarioRepositorio.BuscarTodos();
            return Ok(usuarios);
        }

        // GET: api/usuario/{id}
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Busca um usuário espcífico atráves de um ID")]
        public IActionResult Get(int id)
        {
            if (!UsuarioEhAdmin())
            {
                return StatusCode(StatusCodes.Status403Forbidden, "Acesso restrito aos administradores.");
            }

            var usuario = _usuarioRepositorio.Buscar(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return Ok(usuario);
        }

        // POST: api/usuario (não restrito, pois é para criação de usuários)
        [AllowAnonymous]
        [HttpPost]
        [SwaggerOperation(Summary = "Cadastro de usuário para acesso ao Sistema")]
        // POST: api/usuario (não restrito, pois é para criação de usuários)
        public IActionResult Post([FromBody] Usuario usuario)
        {
            if(usuario.Senha == null || usuario.Senha.Length < 8)
            {
                return BadRequest("A senha deve ter no mínimo 8 caracteres");
            }

            try
            {
                _usuarioRepositorio.Inserir(usuario);
                return CreatedAtAction(nameof(Get), new { id = usuario.Id }, usuario);
            }
            catch (Exception ex)
            {
                // Verifica se a exceção é causada por uma violação de chave única
                if (ex.InnerException != null && ex.InnerException.Message.Contains("Violação da restrição UNIQUE KEY"))
                {
                    return Conflict("E-mail inválido!");
                }

                // Retorna uma mensagem genérica para outras exceções
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao cadastrar, Verifique os campos!");
            }
        }


        // PUT: api/usuario/{id}
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Atualiza um usuário específico através de um ID")]
        public IActionResult Put(int id, [FromBody] Usuario usuario)
        {
            if (!UsuarioEhAdmin())
            {
                return StatusCode(StatusCodes.Status403Forbidden, "Acesso restrito aos administradores.");
            }

            var existingUser = _usuarioRepositorio.Buscar(id);
            if (existingUser == null)
            {
                return NotFound();
            }
            usuario.Id = id;  // Certifica que o ID está correto para a atualização
            _usuarioRepositorio.Alterar(usuario);
            return NoContent();
        }

        // DELETE: api/usuario/{id}
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Exclusão de um usuário pelo ID")]
        public IActionResult Delete(int id)
        {
            if (!UsuarioEhAdmin())
            {
                return StatusCode(StatusCodes.Status403Forbidden, "Acesso restrito aos administradores.");
            }

            var usuario = _usuarioRepositorio.Buscar(id);
            if (usuario == null)
            {
                return NotFound();
            }
            _usuarioRepositorio.Excluir(id);
            return NoContent();
        }

        // Endpoint de Login - POST: api/usuario/login (não restrito)
        [AllowAnonymous]
        [HttpPost("login")]
        [SwaggerOperation(Summary = "Login para acesso ao Sistema")]
        public IActionResult Login([FromBody] Login login)
        {
            // Busca o usuário no repositório pelo e-mail
            var usuarioExistente = _usuarioRepositorio.BuscarPorEmail(login.Email);

            // Verifica se o usuário existe e se a senha está correta
            if (usuarioExistente == null || usuarioExistente.Senha != login.Senha)
            {
                return Unauthorized("E-mail ou senha incorretos!");
            }

            // Configura o token handler para gerar o JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            // Define as propriedades do token, como o ID do usuário e o tempo de expiração
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, usuarioExistente.Id.ToString()),
                    new Claim("email", usuarioExistente.Email) // Adiciona o email como claim
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            // Gera o token usando o token handler
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Converte o token para string para ser retornado ao cliente
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString });
        }

    }
}
