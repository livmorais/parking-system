using Api_dotnet.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using Api_dotnet.Data;
using Microsoft.EntityFrameworkCore;
using System.Transactions;



namespace Api_dotnet.Services
{
    public class ProprietarioService : IProprietarioService 
    {
        private static readonly List<Estacionamento> _estacionamentos = new List<Estacionamento>();
        private readonly IMemoryCache _memoryCache;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static readonly Dictionary<Guid, List<Estacionamento>> _estacionamentosPorUsuario = new Dictionary<Guid, List<Estacionamento>>();
        private static readonly Dictionary<Guid, List<Vaga>> _vagasPorEstacionamento = new Dictionary<Guid, List<Vaga>>();
        
        public ProprietarioService(IMemoryCache memoryCache, IHttpContextAccessor httpContextAccessor, DataContext context)
        {
            _memoryCache = memoryCache;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }


        public async Task<Estacionamento> CriarEstacionamento(EstacionamentoDTO estacionamentoDto)
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim != null && !string.IsNullOrEmpty(userIdClaim.Value) && Guid.TryParse(userIdClaim.Value, out var userGuid))
            {

                var estacionamento = new Estacionamento
                {
                    EstacionamentoId = Guid.NewGuid(),
                    ProprietarioId = userGuid,
                    Nome = estacionamentoDto.Nome,
                    Endereco = estacionamentoDto.Endereco,
                    Telefone = estacionamentoDto.Telefone
                };

                // _estacionamentos.Add(estacionamento);

                // if (!_estacionamentosPorUsuario.ContainsKey(userGuid))
                // {
                //     _estacionamentosPorUsuario[userGuid] = new List<Estacionamento>();
                // }
                // _estacionamentosPorUsuario[userGuid].Add(estacionamento);

                // return estacionamento;
                _context.Estacionamentos.Add(estacionamento);
                await _context.SaveChangesAsync();

                // Consulta ao banco de dados para obter estacionamentos associados ao usuário
                var estacionamentosDoUsuario = await _context.Estacionamentos
                    .Where(e => e.ProprietarioId == userGuid)
                    .ToListAsync();

                // Retornar o estacionamento recém-criado
                return estacionamento;
            }
            else
            {
                throw new ApplicationException("ID do usuário não encontrado no token JWT.");
            }
        }

        public List<Estacionamento> ObterEstacionamentoDoUsuarioLogado()
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim != null && !string.IsNullOrEmpty(userIdClaim.Value) && Guid.TryParse(userIdClaim.Value, out var userGuid))
            {
                var estacionamentosDoUsuario = _context.Estacionamentos
                .Where(e => e.ProprietarioId == userGuid)
                .Include(e => e.Vagas)
                .ToList();

                return estacionamentosDoUsuario;
                // if (_estacionamentosPorUsuario.ContainsKey(userGuid))
                //     {
                //         return _estacionamentosPorUsuario[userGuid];
                //     }
                // else
                // {
                //     return new List<Estacionamento>();
                // }
            }
            else
            {
                throw new ApplicationException("ID do usuário não encontrado no token JWT.");
            }
        }

        public async Task<Estacionamento> EditarEstacionamento(Guid estacionamentoId, EstacionamentoDTO estacionamentoDTO)
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim != null && !string.IsNullOrEmpty(userIdClaim.Value) && Guid.TryParse(userIdClaim.Value, out var userGuid))
            {
                // var estacionamentoDoUsuario = _estacionamentos.FirstOrDefault(e => e.ProprietarioId == userGuid && e.EstacionamentoId == estacionamentoId);
                var estacionamentoDoUsuario = _context.Estacionamentos
                .Include(e => e.Vagas) // Inclui as vagas associadas ao estacionamento
                .FirstOrDefault(e => e.ProprietarioId == userGuid && e.EstacionamentoId == estacionamentoId);

                if (estacionamentoDoUsuario != null)
                {
                    estacionamentoDoUsuario.Nome = estacionamentoDTO.Nome;
                    estacionamentoDoUsuario.Endereco = estacionamentoDTO.Endereco;
                    estacionamentoDoUsuario.Telefone = estacionamentoDTO.Telefone;
                    
                    foreach (var vaga in estacionamentoDoUsuario.Vagas)
                    {
                        vaga.Nome = estacionamentoDTO.Nome;
                        vaga.Telefone = estacionamentoDTO.Telefone;
                    }

                    await _context.SaveChangesAsync();

                    return estacionamentoDoUsuario; 
                }
                else
                {
                    throw new ApplicationException("Estacionamento não encontrado para o usuário logado.");
                }
            }
            else
            {
                throw new ApplicationException("ID do usuário não encontrado no token JWT.");
            }
        }

        // public async Task<Vaga> CriarVaga(Guid estacionamentoId, VagaDTO vagaDto)
        // {
        //     var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

        //     if (userIdClaim != null && !string.IsNullOrEmpty(userIdClaim.Value) && Guid.TryParse(userIdClaim.Value, out var userGuid))
        //     {
        //         // var estacionamentoDoUsuario = _estacionamentos.FirstOrDefault(e => e.ProprietarioId == userGuid && e.EstacionamentoId == estacionamentoId);
        //         var estacionamentoDoUsuario = await _context.Estacionamentos
        //         .Include(e => e.Vagas) 
        //         .FirstOrDefaultAsync(e => e.ProprietarioId == userGuid && e.EstacionamentoId == estacionamentoId);

        //         if (estacionamentoDoUsuario != null)
        //         {
        //             var vaga = new Vaga
        //             {
        //                 VagaId = Guid.NewGuid(),
        //                 EstacionamentoId = estacionamentoId,
        //                 Status = StatusVaga.Disponivel, 
        //                 Tipo = vagaDto.Tipo, 
        //                 Nome = estacionamentoDoUsuario.Nome,
        //                 Telefone = estacionamentoDoUsuario.Telefone
        //             };

        //             // estacionamentoDoUsuario.Vagas.Add(vaga);

        //             // if (!_vagasPorEstacionamento.ContainsKey(estacionamentoId))
        //             // {
        //             //     _vagasPorEstacionamento[estacionamentoId] = new List<Vaga>();
        //             // }
        //             // _vagasPorEstacionamento[estacionamentoId].Add(vaga);
        //             estacionamentoDoUsuario.Vagas.Add(vaga);
        //             await _context.SaveChangesAsync();


        //             return vaga;
        //         }
        //         else
        //         {
        //             throw new ApplicationException("Estacionamento não encontrado para o usuário logado.");
        //         }
        //     }
        //     else
        //     {
        //         throw new ApplicationException("ID do usuário não encontrado no token JWT.");
        //     }
        // }

      
// public async Task<Vaga> CriarVaga(Guid estacionamentoId, VagaDTO vagaDto)
// {
//     var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

//     if (userIdClaim != null && !string.IsNullOrEmpty(userIdClaim.Value) && Guid.TryParse(userIdClaim.Value, out var userGuid))
//     {
//         var estacionamentoDoUsuario = await _context.Estacionamentos
//             .Include(e => e.Vagas)
//             .FirstOrDefaultAsync(e => e.ProprietarioId == userGuid && e.EstacionamentoId == estacionamentoId);

//         if (estacionamentoDoUsuario != null)
//         {
//             // Verifica se a lista de Vagas é nula e a inicializa se for o caso
//             if (estacionamentoDoUsuario.Vagas == null)
//             {
//                 estacionamentoDoUsuario.Vagas = new List<Vaga>();
//             }

//             var vaga = new Vaga
//             {
//                 VagaId = Guid.NewGuid(),
//                 EstacionamentoId = estacionamentoId,
//                 Status = StatusVaga.Disponivel,
//                 Tipo = vagaDto.Tipo,
//                 Nome = estacionamentoDoUsuario.Nome ?? "Nome padrão",
//                 Telefone = estacionamentoDoUsuario.Telefone ?? "Telefone padrão"
//             };

//             estacionamentoDoUsuario.Vagas.Add(vaga);

//             try
//             {
//                 await _context.SaveChangesAsync();
//             }
//             catch (DbUpdateConcurrencyException ex)
//             {
//                 foreach (var entry in ex.Entries)
//                 {
//                     if (entry.Entity is Vaga)
//                     {
//                         var databaseValues = entry.GetDatabaseValues();

//                         if (databaseValues == null)
//                         {
//                             // A entidade foi excluída no banco de dados
//                             // Lide com a situação conforme necessário (por exemplo, lance uma exceção ou registre uma mensagem)
//                         }
//                         else
//                         {
//                             // A entidade foi modificada no banco de dados
//                             entry.OriginalValues.SetValues(databaseValues);

//                             // Aplica suas alterações à entidade recarregada
//                             var vagaAtualizada = (Vaga)entry.Entity;
//                             vagaAtualizada.Tipo = vagaDto.Tipo;
//                             // Atualize outros campos conforme necessário

//                             // Salve as alterações novamente
//                             await _context.SaveChangesAsync();
//                         }
//                     }
//                     else
//                     {
//                         throw new NotSupportedException("Não sei como lidar com conflitos de concorrência para " + entry.Metadata.Name);
//                     }
//                 }
//             }

//             return vaga;
//         }
//         else
//         {
//             throw new ApplicationException("Estacionamento não encontrado para o usuário logado.");
//         }
//     }
//     else
//     {
//         throw new ApplicationException("ID do usuário não encontrado no token JWT.");
//     }
// }

public async Task<Vaga> CriarVaga(Guid estacionamentoId, VagaDTO vagaDto)
{
    var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

    if (userIdClaim != null && !string.IsNullOrEmpty(userIdClaim.Value) && Guid.TryParse(userIdClaim.Value, out var userGuid))
    {
        var estacionamentoDoUsuario = await _context.Estacionamentos
            .FirstOrDefaultAsync(e => e.ProprietarioId == userGuid && e.EstacionamentoId == estacionamentoId);

        if (estacionamentoDoUsuario != null)
        {
            var vaga = new Vaga
            {
                VagaId = Guid.NewGuid(),
                EstacionamentoId = estacionamentoId,
                Status = StatusVaga.Disponivel,
                Tipo = vagaDto.Tipo,
                Nome = estacionamentoDoUsuario.Nome ?? "Nome padrão",
                Telefone = estacionamentoDoUsuario.Telefone ?? "Telefone padrão"
            };

            _context.Vagas.Add(vaga);
            await _context.SaveChangesAsync();

            return vaga;
        }
        else
        {
            throw new ApplicationException("Estacionamento não encontrado para o usuário logado.");
        }
    }
    else
    {
        throw new ApplicationException("ID do usuário não encontrado no token JWT.");
    }
}






        // public bool ExcluirVaga(Guid vagaId)
        public async Task<bool> ExcluirVaga(Guid vagaId)
        {
            var vaga = _vagasPorEstacionamento.Values.SelectMany(list => list)
                .FirstOrDefault(v => v.VagaId == vagaId);

            if (vaga != null)
            {
                var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim != null && !string.IsNullOrEmpty(userIdClaim.Value) && Guid.TryParse(userIdClaim.Value, out var proprietarioId))
                {
                    // var estacionamento = _estacionamentos.FirstOrDefault(e => e.EstacionamentoId == vaga.EstacionamentoId && e.ProprietarioId == proprietarioId);
                    var estacionamento = await _context.Estacionamentos
                    .FirstOrDefaultAsync(e => e.EstacionamentoId == vaga.EstacionamentoId && e.ProprietarioId == proprietarioId);

                    if (estacionamento != null)
                    {
                        if (vaga.Status == StatusVaga.Disponivel)
                        {
                            // estacionamento.Vagas.Remove(vaga);

                            // if (_vagasPorEstacionamento.ContainsKey(vaga.EstacionamentoId))
                            // {
                            //     _vagasPorEstacionamento[vaga.EstacionamentoId].Remove(vaga);
                            // }
                            _context.Vagas.Remove(vaga);
                            await _context.SaveChangesAsync();

                            return true; 
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        throw new ApplicationException("O proprietário não é dono do estacionamento associado à vaga.");
                    }
                }
                else
                {
                    throw new ApplicationException("ID do usuário não encontrado ou inválido no token JWT.");
                }
            }
            else
            {
                return false;
            }
        }

        // public List<Vaga> ObterVagasEstacionamento(Guid estacionamentoId)
        // {
        //     if (_vagasPorEstacionamento.ContainsKey(estacionamentoId))
        //     {
        //         return _vagasPorEstacionamento[estacionamentoId];
        //     }
        //     else
        //     {
        //         throw new ApplicationException("Nenhuma vaga encontrada.");
        //     }
        // }
        public async Task<List<Vaga>> ObterVagasEstacionamento(Guid estacionamentoId)
        {
            var estacionamento = await _context.Estacionamentos
                .Include(e => e.Vagas)
                .FirstOrDefaultAsync(e => e.EstacionamentoId == estacionamentoId);

            if (estacionamento != null)
            {
                return estacionamento.Vagas.ToList();
            }
            else
            {
                throw new ApplicationException("Nenhuma vaga encontrada.");
            }
        }


        public IEnumerable<Estacionamento> ObterInformacoesEstacionamentos()
        {
            // var informacoesEstacionamentos = new List<Estacionamento>();

            // foreach (var estacionamento in _estacionamentos)
            // {
            //     var informacoesEstacionamento = new Estacionamento
            //     {
            //         EstacionamentoId = estacionamento.EstacionamentoId,
            //         Nome = estacionamento.Nome,
            //         Telefone = estacionamento.Telefone,
            //         Endereco = estacionamento.Endereco,
            //         VagasDisponiveisPorTipo = ObterContagemVagasDisponiveisPorTipo(estacionamento.EstacionamentoId)
            //     };

            //     informacoesEstacionamentos.Add(informacoesEstacionamento);
            // }
            var estacionamentos = _context.Estacionamentos
                .Include(e => e.Vagas) 
                .ToList();

            var informacoesEstacionamentos = estacionamentos.Select(estacionamento => new Estacionamento
            {
                EstacionamentoId = estacionamento.EstacionamentoId,
                Nome = estacionamento.Nome,
                Telefone = estacionamento.Telefone,
                Endereco = estacionamento.Endereco,
                VagasDisponiveisPorTipo = ObterContagemVagasDisponiveisPorTipo(estacionamento.Vagas)
            });

            return informacoesEstacionamentos;
        }

        private Dictionary<TipoVaga, int> ObterContagemVagasDisponiveisPorTipo(IEnumerable<Vaga> vagas)
        {
            var contagemPorTipo = new Dictionary<TipoVaga, int>
            {
                { TipoVaga.Comum, 0 },
                { TipoVaga.Pcd, 0 },
                { TipoVaga.CargaDescarga, 0 }
            };

            foreach (var vaga in vagas)
            {
                if (vaga.Status == StatusVaga.Disponivel)
                {
                    contagemPorTipo[vaga.Tipo]++;
                }
            }

            return contagemPorTipo;
        }
        // private Dictionary<TipoVaga, int> ObterContagemVagasDisponiveisPorTipo(Guid estacionamentoId)
        // {
        //     if (_vagasPorEstacionamento.ContainsKey(estacionamentoId))
        //     {
        //         var vagas = _vagasPorEstacionamento[estacionamentoId];
        //         var contagemPorTipo = new Dictionary<TipoVaga, int>
        //         {
        //             { TipoVaga.Comum, 0 },
        //             { TipoVaga.Pcd, 0 },
        //             { TipoVaga.CargaDescarga, 0 }
        //         };

        //         foreach (var vaga in vagas)
        //         {
        //             if (vaga.Status == StatusVaga.Disponivel)
        //             {
        //                 contagemPorTipo[vaga.Tipo]++;
        //             }
        //         }

        //         return contagemPorTipo;
        //     }

        //     return new Dictionary<TipoVaga, int>
        //     {
        //         { TipoVaga.Comum, 0 },
        //         { TipoVaga.Pcd, 0 },
        //         { TipoVaga.CargaDescarga, 0 }
        //     };
        // }

        public bool FazerReserva(Guid estacionamentoId, TipoVaga tipoVaga)
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim != null && !string.IsNullOrEmpty(userIdClaim.Value) && Guid.TryParse(userIdClaim.Value, out var userGuid))
            {
                // var estacionamento = _estacionamentos.FirstOrDefault(e => e.EstacionamentoId == estacionamentoId);
                var estacionamento = _context.Estacionamentos
                .Include(e => e.Vagas) 
                .FirstOrDefault(e => e.EstacionamentoId == estacionamentoId);

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

                         _context.SaveChanges();

                        return true;
                    }
                }
            }

            return false;
        }

        public bool CancelarReserva(Guid vagaId)
        {
            // var vaga = _vagasPorEstacionamento.Values.SelectMany(list => list)
            //     .FirstOrDefault(v => v.VagaId == vagaId);
            var vaga = _context.Vagas.FirstOrDefault(v => v.VagaId == vagaId);

            if (vaga != null)
            {
                if (vaga.Status == StatusVaga.Ocupada)
                {
                    vaga.Status = StatusVaga.Disponivel;

                    // vaga.ClienteId = Guid.Empty;
                    // vaga.Modelo = String.Empty;
                    // vaga.Placa = String.Empty;
                    vaga.ClienteId = null;
                    vaga.Modelo = null;
                    vaga.Placa = null;

                    _context.SaveChanges();

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public List<Vaga> ObterVagasReservadasCliente()
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim != null && !string.IsNullOrEmpty(userIdClaim.Value) && Guid.TryParse(userIdClaim.Value, out var clienteId))
            {
                // var vagasReservadas = _vagasPorEstacionamento.Values
                //     .SelectMany(list => list)
                //     .Where(v => v.ClienteId == clienteId)
                //     .ToList();
                var vagasReservadas = _context.Vagas
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
                // var vagaParaCancelar = _vagasPorEstacionamento.Values
                //     .SelectMany(list => list)
                //     .FirstOrDefault(v => v.VagaId == vagaId && v.ClienteId == clienteId);
                var vagaParaCancelar = _context.Vagas.FirstOrDefault(v => v.VagaId == vagaId && v.ClienteId == clienteId);

                if (vagaParaCancelar != null)
                {
                    vagaParaCancelar.Status = StatusVaga.Disponivel;

                    // vagaParaCancelar.ClienteId = Guid.Empty;
                    // vagaParaCancelar.Modelo = String.Empty;
                    // vagaParaCancelar.Placa = String.Empty;
                    vagaParaCancelar.ClienteId = null;
                    vagaParaCancelar.Modelo = null;
                    vagaParaCancelar.Placa = null;

                    _context.SaveChanges();

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

        public List<Vaga> ObterVagasOcupadasPorEstacionamento(Guid estacionamentoId)
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var proprietarioId))
            {
                // var estacionamento = _estacionamentos.FirstOrDefault(e => e.EstacionamentoId == estacionamentoId && e.ProprietarioId == proprietarioId);
                var estacionamento = _context.Estacionamentos
                .Include(e => e.Vagas)
                .FirstOrDefault(e => e.EstacionamentoId == estacionamentoId && e.ProprietarioId == proprietarioId);

                if (estacionamento != null)
                {
                    var vagasOcupadas = estacionamento.Vagas.Where(v => v.Status == StatusVaga.Ocupada).ToList();

                    return vagasOcupadas;
                }
                else
                {
                    throw new ApplicationException("Estacionamento não encontrado para o proprietário logado.");
                }
            }
            else
            {
                throw new ApplicationException("ID do usuário não encontrado ou inválido no token JWT.");
            }
        }

    }
}
