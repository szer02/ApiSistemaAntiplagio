class Livro:
    def __init__(self, titulo, autor, ano_publicacao):
        self.titulo = titulo
        self.autor = autor
        self.ano_publicacao = ano_publicacao

    def detalhes(self):
        return f"Título: {self.titulo} | Autor: {self.autor} | Ano: {self.ano_publicacao}"


class GerenciadorBiblioteca:
    def __init__(self):
        self.livros = []

    def exibir_menu(self):
        while True:
            print("\n--- Sistema de Biblioteca ---")
            print("1. Adicionar Livro")
            print("2. Listar Livros")
            print("3. Buscar Livro")
            print("0. Sair")
            opcao = input("Escolha uma opção: ")

            if opcao == "1":
                self.adicionar_livro()
            elif opcao == "2":
                self.listar_livros()
            elif opcao == "3":
                self.buscar_livro()
            elif opcao == "0":
                print("Encerrando o sistema...")
                break
            else:
                print("Opção inválida. Tente novamente.")

    def adicionar_livro(self):
        titulo = input("Digite o título do livro: ")
        autor = input("Digite o autor do livro: ")
        try:
            ano_publicacao = int(input("Digite o ano de publicação: "))
            livro = Livro(titulo, autor, ano_publicacao)
            self.livros.append(livro)
            print("Livro adicionado com sucesso!")
        except ValueError:
            print("Ano inválido. Operação cancelada.")

    def listar_livros(self):
        print("\n--- Lista de Livros ---")
        if not self.livros:
            print("Nenhum livro cadastrado.")
        else:
            for livro in self.livros:
                print(livro.detalhes())

    def buscar_livro(self):
        titulo_busca = input("Digite o título do livro que deseja buscar: ")
        livro_encontrado = next((livro for livro in self.livros if livro.titulo.lower() == titulo_busca.lower()), None)

        if livro_encontrado:
            print("\nLivro encontrado:")
            print(livro_encontrado.detalhes())
        else:
            print("Livro não encontrado.")


if __name__ == "__main__":
    gerenciador = GerenciadorBiblioteca()
    gerenciador.exibir_menu()
