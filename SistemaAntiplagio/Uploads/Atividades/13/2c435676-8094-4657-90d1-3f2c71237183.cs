using System;
using System.Collections.Generic;

namespace BibliotecaRefatorada
{
    class Program
    {
        static void Main(string[] args)
        {
            GerenciadorBiblioteca gerenciador = new GerenciadorBiblioteca();
            gerenciador.ExibirMenu();
        }
    }

    class GerenciadorBiblioteca
    {
        private List<Livro> livros = new List<Livro>();

        public void ExibirMenu()
        {
            while (true)
            {
                Console.WriteLine("\n--- Sistema de Biblioteca ---");
                Console.WriteLine("1. Adicionar Livro");
                Console.WriteLine("2. Listar Livros");
                Console.WriteLine("3. Buscar Livro");
                Console.WriteLine("0. Sair");
                Console.Write("Escolha uma opção: ");

                string escolha = Console.ReadLine();

                switch (escolha)
                {
                    case "1":
                        AdicionarLivro();
                        break;
                    case "2":
                        ListarLivros();
                        break;
                    case "3":
                        BuscarLivro();
                        break;
                    case "0":
                        Console.WriteLine("Encerrando o sistema...");
                        return;
                    default:
                        Console.WriteLine("Opção inválida. Tente novamente.");
                        break;
                }
            }
        }

        private void AdicionarLivro()
        {
            Console.Write("Digite o título do livro: ");
            string titulo = Console.ReadLine();

            Console.Write("Digite o autor do livro: ");
            string autor = Console.ReadLine();

            Console.Write("Digite o ano de publicação: ");
            if (!int.TryParse(Console.ReadLine(), out int anoPublicacao))
            {
                Console.WriteLine("Ano inválido. Operação cancelada.");
                return;
            }

            livros.Add(new Livro(titulo, autor, anoPublicacao));
            Console.WriteLine("Livro adicionado com sucesso!");
        }

        private void ListarLivros()
        {
            Console.WriteLine("\n--- Lista de Livros ---");

            if (livros.Count == 0)
            {
                Console.WriteLine("Nenhum livro cadastrado.");
                return;
            }

            livros.ForEach(livro => Console.WriteLine(livro.ObterDetalhes()));
        }

        private void BuscarLivro()
        {
            Console.Write("Digite o título do livro que deseja buscar: ");
            string tituloBusca = Console.ReadLine();

            Livro livroEncontrado = livros.Find(livro => livro.Titulo.Equals(tituloBusca, StringComparison.OrdinalIgnoreCase));

            if (livroEncontrado != null)
            {
                Console.WriteLine("\nLivro encontrado:");
                Console.WriteLine(livroEncontrado.ObterDetalhes());
            }
            else
            {
                Console.WriteLine("Livro não encontrado.");
            }
        }
    }

    class Livro
    {
        public string Titulo { get; }
        public string Autor { get; }
        public int AnoPublicacao { get; }

        public Livro(string titulo, string autor, int anoPublicacao)
        {
            Titulo = titulo;
            Autor = autor;
            AnoPublicacao = anoPublicacao;
        }

        public string ObterDetalhes()
        {
            return $"Título: {Titulo} | Autor: {Autor} | Ano: {AnoPublicacao}";
        }
    }
}
