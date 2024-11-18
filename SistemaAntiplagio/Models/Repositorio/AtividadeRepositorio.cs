using System;
using System.Collections.Generic;
using Dapper;
using SistemaAntiplagio.Models.Repositorio.Entidades;

namespace SistemaAntiplagio.Models.Repositorio
{
    public class AtividadeRepositorio : IAtividadeRepositorio
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        public AtividadeRepositorio(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IEnumerable<Atividade> BuscarTodos()
        {
            using var connection = _connectionFactory.CreateConnection();
            return connection.Query<Atividade>("SELECT * FROM Atividade");
        }

        public Atividade Buscar(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            return connection.QuerySingleOrDefault<Atividade>("SELECT * FROM Atividade WHERE Id = @Id", new { Id = id });
        }

        public void Inserir(Atividade model)
        {
            model.Guid = Guid.NewGuid(); // Gera o GUID automaticamente
            model.DataCriacao = DateTime.Now; // Define a data de criação atual

            var sql = "INSERT INTO Atividade (Guid, UsuarioId, Nome, Descricao, DataCriacao) VALUES (@Guid, @UsuarioId, @Nome, @Descricao, @DataCriacao)";
            using var connection = _connectionFactory.CreateConnection();
            connection.Execute(sql, model);

            var id = connection.QuerySingleOrDefault<int >("SELECT Id FROM Atividade WHERE Guid = @Guid", new { Guid = model.Guid  });
            model.Id = id;
        }

        public void Alterar(Atividade model)
        {
            var sql = "UPDATE Atividade SET Nome = @Nome, Descricao = @Descricao WHERE Id = @Id";
            using var connection = _connectionFactory.CreateConnection();
            connection.Execute(sql, model);
        }

        public void Excluir(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Execute("DELETE FROM Atividade WHERE Id = @Id", new { Id = id });
        }
    }
}
