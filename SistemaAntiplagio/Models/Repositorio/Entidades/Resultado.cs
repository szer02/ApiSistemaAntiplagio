using System;

namespace SistemaAntiplagio.Models.Repositorio.Entidades
{
    public class Resultado
    {
        public virtual int Id { get; set; }
        public virtual int AtividadeId { get; set; }
        public virtual int Arquivo1Id { get; set; }
        public virtual int Arquivo2Id { get; set; }
        public virtual string Relatorio { get; set; }
        public virtual decimal PercentualSimilaridade { get; set; }
        public virtual DateTime DataAnalise { get; set; }
    }
}
