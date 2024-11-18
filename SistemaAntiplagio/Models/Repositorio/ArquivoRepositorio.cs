using System;
using System.Collections.Generic;
using Dapper;
using SistemaAntiplagio.Models.Repositorio.Entidades;

namespace SistemaAntiplagio.Models.Repositorio
{
    public class ArquivoRepositorio : IArquivoRepositorio
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        public ArquivoRepositorio(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IEnumerable<Arquivo> BuscarPorAtividadeId(int atividadeId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT * FROM Arquivo WHERE AtividadeId = @AtividadeId";
            return connection.Query<Arquivo>(sql, new { AtividadeId = atividadeId });
        }

        public IEnumerable<Arquivo> BuscarTodos()
        {
            using var connection = _connectionFactory.CreateConnection();
            return connection.Query<Arquivo>("SELECT * FROM Arquivo");
        }

        public Arquivo Buscar(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            return connection.QuerySingleOrDefault<Arquivo>("SELECT * FROM Arquivo WHERE Id = @Id", new { Id = id });
        }

        public void Inserir(Arquivo model)
        {
            model.Guid = Guid.NewGuid(); // Gera o GUID automaticamente para o arquivo
            model.DataUpload = DateTime.Now; // Define a data de upload atual

            var sql = "INSERT INTO Arquivo (Guid, NomeOriginal, AtividadeId, CaminhoArquivo, DataUpload) VALUES (@Guid, @NomeOriginal, @AtividadeId, @CaminhoArquivo, @DataUpload)";
            using var connection = _connectionFactory.CreateConnection();
            connection.Execute(sql, model);

            var id = connection.QuerySingleOrDefault<int>("SELECT Id FROM Arquivo WHERE Guid = @Guid", new { Guid = model.Guid });
            model.Id = id;
        }


        public void Alterar(Arquivo model)
        {
            var sql = "UPDATE Arquivo SET CaminhoArquivo = @CaminhoArquivo WHERE Id = @Id";
            using var connection = _connectionFactory.CreateConnection();
            connection.Execute(sql, model);
        }

        public void Excluir(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Execute("DELETE FROM Arquivo WHERE Id = @Id", new { Id = id });
        }
    }
}
