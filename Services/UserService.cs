using Microsoft.Extensions.Caching.Memory;
using Api_dotnet.Models;
using Api_dotnet.Data;

namespace Api_dotnet.Services
{
    public class UserService : IUserService
    { 
        private readonly List<Cliente> _registeredClientes = new List<Cliente>();
        private readonly List<Proprietario> _registeredProprietarios = new List<Proprietario>();
        private readonly IMemoryCache _memoryCache;
        private readonly DataContext _context; 

        public UserService(IMemoryCache memoryCache, DataContext context)
        {
            _memoryCache = memoryCache;
            _context = context;
        }

        public Proprietario RegisterProprietario(Proprietario proprietario)
        {

            // _registeredProprietarios.Add(proprietario);
            _context.Proprietarios.Add(proprietario);
            _context.SaveChanges();
            _memoryCache.Set(proprietario.Email, proprietario);
    

            return proprietario;
        }

        public Proprietario GetProprietarioByName(string email)
        {
            if (_memoryCache.TryGetValue(email, out Proprietario proprietario))
            {
                return proprietario;
            }
            // return null;
            return _context.Proprietarios.FirstOrDefault(p => p.Email == email);
        }


        public Cliente RegisterCliente(Cliente cliente)
        {

            // _registeredClientes.Add(cliente);
            _context.Clientes.Add(cliente);
            _context.SaveChanges();
            _memoryCache.Set(cliente.Email, cliente);

            return cliente;
        }
        

        public Cliente GetClienteByName(string email)
        {
            if (_memoryCache.TryGetValue(email, out Cliente cliente))
            {
                return cliente;
            }
            // return null;
            return _context.Clientes.FirstOrDefault(c => c.Email == email);
        }
    }
}