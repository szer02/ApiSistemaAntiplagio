using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using SistemaAntiplagio.Models.Repositorio.Entidades;

namespace SistemaAntiplagio.Models.Repositorio
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        public UsuarioRepositorio(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IEnumerable<Usuario> BuscarTodos()
        {
            using var connection = _connectionFactory.CreateConnection();
            return connection.Query<Usuario>("SELECT * FROM Usuario");
        }

        public Usuario Buscar(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            return connection.QuerySingleOrDefault<Usuario>("SELECT * FROM Usuario WHERE Id = @Id", new { Id = id });
        }
        public Usuario BuscarPorEmail(string email)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT * FROM Usuario WHERE Email = @Email";
            return connection.QuerySingleOrDefault<Usuario>(sql, new { Email = email });
        }

        public void Inserir(Usuario model)
        {
            model.DataCadastro = DateTime.Now; // Define a data de cadastro atual

            using var connection = _connectionFactory.CreateConnection();
            var sql = "INSERT INTO Usuario (Nome, Email, Senha, DataCadastro, Status) VALUES (@Nome, @Email, @Senha, @DataCadastro, @Status)";
            connection.Execute(sql, model);

            var id = connection.QuerySingleOrDefault<int>("SELECT Id FROM Usuario WHERE Email = @Email", new { Email = model.Email });
            model.Id = id;
        }

        public void Alterar(Usuario model)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "UPDATE Usuario SET Nome = @Nome, Email = @Email, Senha = @Senha, Status = @Status WHERE Id = @Id";
            connection.Execute(sql, model);
        }

        public void Excluir(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Execute("DELETE FROM Usuario WHERE Id = @Id", new { Id = id });
        }
    }
}
