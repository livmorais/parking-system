using Api_dotnet.Models;

namespace Api_dotnet.Services
{
    public interface IUserService
    {
        Cliente RegisterCliente(Cliente cliente);
        Proprietario RegisterProprietario(Proprietario proprietario);
        Cliente GetClienteByName(string Email);
        Proprietario GetProprietarioByName(string email);
    }
}