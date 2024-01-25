namespace Api_dotnet.Models
{
    public class Cliente
    {
        public Guid ClienteId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public string Placa{ get; set; } = string.Empty;
        public List<Vaga> Reservas { get; set; } = new List<Vaga>();
        public string PasswordHash { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime TokenCreated { get; set; } 
        public DateTime TokenExpires { get; set; }
        public string Role { get; set; }
    }
}