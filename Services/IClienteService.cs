using Api_dotnet.Models;

namespace Api_dotnet.Services 
{
    public interface IClienteService
    {
        bool FazerReserva(Guid estacionamentoId, TipoVaga tipoVaga);
        List<Vaga> ObterVagasReservadasCliente();
        bool CancelarReservaCliente(Guid vagaId);
    }
}