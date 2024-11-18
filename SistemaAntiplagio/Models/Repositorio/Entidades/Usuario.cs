using System;

namespace SistemaAntiplagio.Models.Repositorio.Entidades
{
    public class Usuario
    {
        public virtual int Id { get; set; }
        public virtual string Nome { get; set; }
        public virtual string Email { get; set; }
        public virtual string Senha { get; set; }
        public virtual DateTime DataCadastro { get; set; }
        public virtual bool Status { get; set; }
    }
}
