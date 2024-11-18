using Dapper;
using SistemaAntiplagio.Models.Repositorio.Entidades;
using SistemaAntiplagio.Models.ViewModel.DTOs;
using System.Collections.Generic;
using System.Data;

namespace SistemaAntiplagio.Models.Repositorio
{
    public class ResultadoRepositorio : IResultadoRepositorio
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        public ResultadoRepositorio(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IEnumerable<Resultado> BuscarTodos()
        {
            using var connection = _connectionFactory.CreateConnection();
            return connection.Query<Resultado>("SELECT * FROM Resultado");
        }

        public Resultado Buscar(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            return connection.QuerySingleOrDefault<Resultado>("SELECT * FROM Resultado WHERE Id = @Id", new { Id = id });
        }

        public void Inserir(Resultado resultado)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "INSERT INTO Resultado (AtividadeId, Arquivo1Id, Arquivo2Id, Relatorio, PercentualSimilaridade, DataAnalise) " +
                      "VALUES (@AtividadeId, @Arquivo1Id, @Arquivo2Id, @Relatorio, @PercentualSimilaridade, @DataAnalise)";
            connection.Execute(sql, resultado);
        }

        public void Alterar(Resultado resultado)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "UPDATE Resultado SET AtividadeId = @AtividadeId, Arquivo1Id = @Arquivo1Id, Arquivo2Id = @Arquivo2Id, Relatorio = @Relatorio, " +
                      "PercentualSimilaridade = @PercentualSimilaridade, DataAnalise = @DataAnalise WHERE Id = @Id";
            connection.Execute(sql, resultado);
        }

        public void Excluir(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Execute("DELETE FROM Resultado WHERE Id = @Id", new { Id = id });
        }

        // public IEnumerable<Resultado> BuscarPorAtividade(int atividadeId)
        // {
        //     using var connection = _connectionFactory.CreateConnection();
        //     return connection.Query<Resultado>("SELECT * FROM Resultado WHERE AtividadeId = @AtividadeId", new { AtividadeId = atividadeId });
        // }

        public IEnumerable<ResultadoDto> BuscarPorAtividade(int atividadeId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"
                SELECT r.Id, r.AtividadeId, r.PercentualSimilaridade, r.DataAnalise,
                    a1.NomeOriginal AS NomeArquivo1, a2.NomeOriginal AS NomeArquivo2
                FROM Resultado r
                JOIN Arquivo a1 ON r.Arquivo1Id = a1.Id
                JOIN Arquivo a2 ON r.Arquivo2Id = a2.Id
                WHERE r.AtividadeId = @AtividadeId";

            return connection.Query<ResultadoDto>(sql, new { AtividadeId = atividadeId });
        }

    }
}