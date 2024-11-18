using System;
using System.Collections.Generic;

namespace Biblioteca
{
    class Program
    {
        static void Main(string[] args)
        {
            Biblioteca biblioteca = new Biblioteca();
            int opcao;

            do
            {
                Console.WriteLine("\n--- Sistema de Gerenciamento de Biblioteca ---");
                Console.WriteLine("1. Adicionar livro");
                Console.WriteLine("2. Listar livros");
                Console.WriteLine("3. Buscar livro por título");
                Console.WriteLine("0. Sair");
                Console.Write("Escolha uma opção: ");
                opcao = int.Parse(Console.ReadLine());

                switch (opcao)
                {
                    case 1:
                        biblioteca.AdicionarLivro();
                        break;
                    case 2:
                        biblioteca.ListarLivros();
                        break;
                    case 3:
                        biblioteca.BuscarLivroPorTitulo();
                        break;
                    case 0:
                        Console.WriteLine("Saindo do sistema...");
                        break;
                    default:
                        Console.WriteLine("Opção inválida. Tente novamente.");
                        break;
                }
            } while (opcao != 0);
        }
    }

    class Biblioteca
    {
        private List<Livro> livros;

        public Biblioteca()
        {
            livros = new List<Livro>();
        }

        public void AdicionarLivro()
        {
            Console.Write("Informe o título do livro: ");
            string titulo = Console.ReadLine();
            Console.Write("Informe o autor do livro: ");
            string autor = Console.ReadLine();
            Console.Write("Informe o ano de publicação: ");
            int anoPublicacao = int.Parse(Console.ReadLine());

            Livro livro = new Livro(titulo, autor, anoPublicacao);
            livros.Add(livro);
            Console.WriteLine("Livro adicionado com sucesso!");
        }

        public void ListarLivros()
        {
            Console.WriteLine("\n--- Lista de Livros ---");
            if (livros.Count == 0)
            {
                Console.WriteLine("Nenhum livro cadastrado.");
            }
            else
            {
                foreach (var livro in livros)
                {
                    Console.WriteLine(livro.ToString());
                }
            }
        }

        public void BuscarLivroPorTitulo()
        {
            Console.Write("Informe o título do livro para busca: ");
            string titulo = Console.ReadLine();
            var livroEncontrado = livros.Find(l => l.Titulo.Equals(titulo, StringComparison.OrdinalIgnoreCase));

            if (livroEncontrado != null)
            {
                Console.WriteLine("\nLivro encontrado:");
                Console.WriteLine(livroEncontrado.ToString());
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

        public override string ToString()
        {
            return $"Título: {Titulo} | Autor: {Autor} | Ano: {AnoPublicacao}";
        }
    }
}
