using Api_dotnet.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace Api_dotnet.Services
{
    public class ClienteService : IClienteService
    {
        private static readonly List<Estacionamento> _estacionamentos = new List<Estacionamento>();
        private readonly IMemoryCache _memoryCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static readonly Dictionary<Guid, List<Vaga>> _vagasPorEstacionamento = new Dictionary<Guid, List<Vaga>>();

    
        public ClienteService(IMemoryCache memoryCache, IHttpContextAccessor httpContextAccessor)
        {
            _memoryCache = memoryCache;
            _httpContextAccessor = httpContextAccessor;
        }

    
        public bool FazerReserva(Guid estacionamentoId, TipoVaga tipoVaga)
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim != null && !string.IsNullOrEmpty(userIdClaim.Value) && Guid.TryParse(userIdClaim.Value, out var userGuid))
            {
                var estacionamento = _estacionamentos.FirstOrDefault(e => e.EstacionamentoId == estacionamentoId);

                if (estacionamento != null)
                {
                    var vagasDisponiveis = estacionamento.Vagas
                        .Where(v => v.Status == StatusVaga.Disponivel && v.Tipo == tipoVaga)
                        .ToList();

                    if (vagasDisponiveis.Any())
                    {
                        var random = new Random();
                        var vagaEscolhida = vagasDisponiveis[random.Next(vagasDisponiveis.Count)];

                        vagaEscolhida.ClienteId = userGuid;

                        var modeloClaim = _httpContextAccessor.HttpContext.User.FindFirst("Modelo");
                        var placaClaim = _httpContextAccessor.HttpContext.User.FindFirst("Placa");

                        vagaEscolhida.Modelo = modeloClaim?.Value;
                        vagaEscolhida.Placa = placaClaim?.Value;

                        vagaEscolhida.Status = StatusVaga.Ocupada;

                        return true;
                    }
                }
            }

            return false;
        }

        public List<Vaga> ObterVagasReservadasCliente()
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim != null && !string.IsNullOrEmpty(userIdClaim.Value) && Guid.TryParse(userIdClaim.Value, out var clienteId))
            {
                var vagasReservadas = _vagasPorEstacionamento.Values
                    .SelectMany(list => list)
                    .Where(v => v.ClienteId == clienteId)
                    .ToList();

                return vagasReservadas;
            }
            else
            {
                throw new ApplicationException("ID do cliente não encontrado no token JWT.");
            }
        }

        public bool CancelarReservaCliente(Guid vagaId)
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim != null && !string.IsNullOrEmpty(userIdClaim.Value) && Guid.TryParse(userIdClaim.Value, out var clienteId))
            {
                var vagaParaCancelar = _vagasPorEstacionamento.Values
                    .SelectMany(list => list)
                    .FirstOrDefault(v => v.VagaId == vagaId && v.ClienteId == clienteId);

                if (vagaParaCancelar != null)
                {
                    vagaParaCancelar.Status = StatusVaga.Disponivel;

                    vagaParaCancelar.ClienteId = Guid.Empty;
                    vagaParaCancelar.Modelo = String.Empty;
                    vagaParaCancelar.Placa = String.Empty;

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                throw new ApplicationException("ID do cliente não encontrado no token JWT.");
            }
        }
    }
}