namespace Api_dotnet.Models
{
    public class RefreshToken
    {
        public required string Token { get; set; }

        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Expires { get; set; } 
    }
}