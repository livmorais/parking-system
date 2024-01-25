using Api_dotnet.Models;

namespace Api_dotnet.Services 
{
    public interface IProprietarioService
    {
        Task<Estacionamento> CriarEstacionamento(EstacionamentoDTO estacionamentoDTO);
        List<Estacionamento> ObterEstacionamentoDoUsuarioLogado();
        Task<Estacionamento> EditarEstacionamento(Guid estacionamentoId, EstacionamentoDTO estacionamentoDTO);
        Task<Vaga> CriarVaga(Guid estacionamentoId, VagaDTO vagaDto);
        // List<Vaga> ObterVagasEstacionamento(Guid estacionamentoId);
        Task<List<Vaga>> ObterVagasEstacionamento(Guid estacionamentoId);

        IEnumerable<Estacionamento> ObterInformacoesEstacionamentos();
        bool FazerReserva(Guid estacionamentoId, TipoVaga tipoVaga);
        bool CancelarReserva(Guid vagaId);
        List<Vaga> ObterVagasReservadasCliente();
        bool CancelarReservaCliente(Guid vagaId);
        List<Vaga> ObterVagasOcupadasPorEstacionamento(Guid estacionamentoId);
        // bool ExcluirVaga(Guid vagaId);
        Task<bool> ExcluirVaga(Guid vagaId);
    }
}