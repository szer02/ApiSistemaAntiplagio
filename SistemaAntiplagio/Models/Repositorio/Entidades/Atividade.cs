using System;

namespace SistemaAntiplagio.Models.Repositorio.Entidades
{
    public class Atividade
    {
        public virtual int Id { get; set; } // Chave primária gerada automaticamente
        public virtual Guid Guid { get; set; } // Identificador único para a atividade
        public virtual string Nome { get; set; }
        public virtual string Descricao { get; set; }
        public virtual DateTime DataCriacao { get; set; } 
        public virtual int UsuarioId { get; set; } // Chave estrangeira que referencia o usuário
    }
}
