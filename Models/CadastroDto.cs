namespace Api_dotnet.Models
{
    public class CadastroDto
    {
        public string Email { get; set; }
        public string Nome { get; set; }
        public string Password { get; set; }
        public string Telefone { get; set; } 
        public string Endereco { get; set; } 
        public string Modelo { get; set; } 
        public string Placa { get; set; }
        public bool IsCliente { get; set; } 
    }
}