using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SistemaAntiplagio.Models.Repositorio;
using SistemaAntiplagio.Models.Repositorio.Entidades;

namespace SistemaAntiplagio.Services
{
    public class DeteccaoPlagioService
    {
        private readonly IArquivoRepositorio _arquivoRepositorio;
        private readonly IResultadoRepositorio _resultadoRepositorio;

        public DeteccaoPlagioService(IArquivoRepositorio arquivoRepositorio, IResultadoRepositorio resultadoRepositorio)
        {
            _arquivoRepositorio = arquivoRepositorio;
            _resultadoRepositorio = resultadoRepositorio;
        }

        // Método principal que executa a detecção de plágio em uma lista de arquivos
        public async Task ExecutarDeteccaoPlagio(int atividadeId, decimal limiteSimilaridade = 0)
        {
            // Carrega todos os arquivos associados à atividade
            var arquivos = _arquivoRepositorio.BuscarPorAtividadeId(atividadeId).ToList();
            int totalArquivos = arquivos.Count;

            if (totalArquivos < 2)
            {
                Console.WriteLine("Não há arquivos suficientes para verificar plágio.");
                return;
            }

            // Converte o conteúdo dos arquivos para uma lista de strings
            var conteudosArquivos = arquivos.Select(arquivo => File.ReadAllText(arquivo.CaminhoArquivo)).ToList();
            var headers = arquivos.Select(a => a.NomeOriginal).ToArray();

            // Matriz de similaridade
            double[][] matrizSimilaridade = new double[totalArquivos][];
            for (int i = 0; i < totalArquivos; i++)
            {
                matrizSimilaridade[i] = new double[totalArquivos];
            }

            // Executa a comparação de similaridade
            await Task.Run(() => SimilaridadeCos(conteudosArquivos, matrizSimilaridade));

            // Salva os resultados da comparação no banco de dados
            for (int i = 0; i < totalArquivos; i++)
            {
                for (int j = i + 1; j < totalArquivos; j++)
                {
                    decimal percentual = (decimal)(matrizSimilaridade[i][j] * 100); // Conversão explícita para decimal
                    if (percentual >= limiteSimilaridade)
                    {
                        var resultado = new Resultado
                        {
                            AtividadeId = atividadeId,
                            Arquivo1Id = arquivos[i].Id,
                            Arquivo2Id = arquivos[j].Id,
                            PercentualSimilaridade = percentual,
                            DataAnalise = DateTime.Now
                        };

                        _resultadoRepositorio.Inserir(resultado);
                    }
                }
            }
        }

        // Método de similaridade usando cosseno
        private void SimilaridadeCos(List<string> conteudos, double[][] matriz)
        {
            for (int i = 0; i < conteudos.Count; i++)
            {
                for (int j = 0; j < conteudos.Count; j++)
                {
                    if (i == j) continue;
                    matriz[i][j] = CalcularSimilaridade(conteudos[i], conteudos[j]);
                }
            }
        }

        // Calcula a similaridade entre dois textos
        private double CalcularSimilaridade(string texto1, string texto2)
        {
            var tokens = Tokenizar(texto1, texto2);
            var vetor1 = Vetorizar(texto1, tokens);
            var vetor2 = Vetorizar(texto2, tokens);

            double produtoEscalar = 0;
            double norma1 = 0;
            double norma2 = 0;

            for (int i = 0; i < tokens.Count; i++)
            {
                produtoEscalar += vetor1[i] * vetor2[i];
                norma1 += Math.Pow(vetor1[i], 2);
                norma2 += Math.Pow(vetor2[i], 2);
            }

            return produtoEscalar / (Math.Sqrt(norma1) * Math.Sqrt(norma2));
        }

        // Converte o texto em um vetor de contagem de palavras
        private List<double> Vetorizar(string texto, List<string> tokens)
        {
            var vetor = new List<double>(new double[tokens.Count]);

            var palavras = Regex.Split(texto, @"\W+");
            foreach (var palavra in palavras)
            {
                int indice = tokens.IndexOf(palavra);
                if (indice >= 0)
                {
                    vetor[indice]++;
                }
            }

            return vetor;
        }

        // Método que extrai tokens únicos dos textos
        private List<string> Tokenizar(string texto1, string texto2)
        {
            var tokens = new HashSet<string>();

            var palavras1 = Regex.Split(texto1, @"\W+");
            var palavras2 = Regex.Split(texto2, @"\W+");

            foreach (var palavra in palavras1.Concat(palavras2))
            {
                if (!string.IsNullOrWhiteSpace(palavra))
                {
                    tokens.Add(palavra);
                }
            }

            return tokens.ToList();
        }
    }
}
