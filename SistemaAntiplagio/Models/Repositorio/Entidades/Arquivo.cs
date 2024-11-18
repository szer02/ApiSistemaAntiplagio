using System;

namespace SistemaAntiplagio.Models.Repositorio.Entidades;
public class Arquivo
{
    public virtual int Id { get; set; } // Chave primária do arquivo
    public virtual Guid Guid { get; set; } // Identificador único para o arquivo
    public virtual string NomeOriginal { get; set; } // Nome original do arquivo enviado pelo usuário
    public virtual string CaminhoArquivo { get; set; } // Caminho de armazenamento do arquivo
    public virtual DateTime DataUpload { get; set; } // Data de upload do arquivo
    public virtual int AtividadeId { get; set; } // Chave estrangeira que referencia a atividade
}
