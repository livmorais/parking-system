namespace Api_dotnet.Models
{
    public class Vaga
    {
        public Guid VagaId { get; set; }
        public Guid EstacionamentoId { get; set; }
        public StatusVaga Status { get; set; }
        public TipoVaga Tipo { get; set; }
        public Guid? ClienteId { get; set; }
        public string? Modelo { get; set; } 
        public string? Placa { get; set; }
        public string Nome { get; set; }
        public string Telefone { get; set; }
    }

    public enum StatusVaga
    {
        Disponivel,
        Ocupada
    }

    public enum TipoVaga
    {
        Comum,
        Pcd,
        CargaDescarga
    }
}