using Api_dotnet.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api_dotnet.Models 
{
    public class Estacionamento
    {
        public Guid EstacionamentoId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public Guid ProprietarioId { get; set; }
        public ICollection<Vaga> Vagas { get; set; } = new List<Vaga>();
        [NotMapped]
        public Dictionary<TipoVaga, int> VagasDisponiveisPorTipo { get; set; }
    }
}