using Api_dotnet.Models;

namespace Api_dotnet.Models
{
    public class Proprietario
    {
        public Guid ProprietarioId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime TokenCreated { get; set; } 
        public DateTime TokenExpires { get; set; }
        public string Role { get; set; }
        public ICollection<Estacionamento> Estacionamentos { get; set; } = new List<Estacionamento>();

    }
}