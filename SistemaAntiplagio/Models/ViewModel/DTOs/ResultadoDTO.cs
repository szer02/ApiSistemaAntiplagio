//DTO (Data Transfer Object)

namespace SistemaAntiplagio.Models.ViewModel.DTOs;
public class ResultadoDto
{
    public int Id { get; set; }
    public int AtividadeId { get; set; }
    public string NomeArquivo1 { get; set; }
    public string NomeArquivo2 { get; set; }
    public decimal PercentualSimilaridade { get; set; }
    public DateTime DataAnalise { get; set; }
}
