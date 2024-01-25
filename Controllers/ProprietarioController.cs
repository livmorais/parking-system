
using Api_dotnet.Models;
using Api_dotnet.Services;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Api_dotnet.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize(Roles = "Admin")]

    public class ProprietarioController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IProprietarioService _proprietarioService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProprietarioController(IUserService userService, IProprietarioService proprietarioService, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _proprietarioService = proprietarioService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        public async Task<IActionResult>  CriarEstacionamento(EstacionamentoDTO estacionamentoDto)
        {
            try
            {
                var estacionamentoCriado = await _proprietarioService.CriarEstacionamento(estacionamentoDto);

                return Ok(estacionamentoCriado);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult ObterMeusEstacionamentos()
        {
            try
            {
                var estacionamentoDoUsuario = _proprietarioService.ObterEstacionamentoDoUsuarioLogado();

                if (estacionamentoDoUsuario != null && estacionamentoDoUsuario.Count > 0)
                {
                    return Ok(estacionamentoDoUsuario);
                }
                else
                {
                    return NotFound("Nenhum estacionamento encontrado para o usuário logado.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("{estacionamentoId}")]
        public async Task<IActionResult> EditarEstacionamento(Guid estacionamentoId, EstacionamentoDTO estacionamentoDto)
        {
            try
            {
                var estacionamentoEditado = await _proprietarioService.EditarEstacionamento(estacionamentoId, estacionamentoDto);

                return Ok(estacionamentoEditado);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("{estacionamentoId}/criar-vaga")]
            public async Task<IActionResult> CriarVaga(Guid estacionamentoId, VagaDTO vagaDto)
            {
                try
                {
                    var vagaCriada = await _proprietarioService.CriarVaga(estacionamentoId, vagaDto);

                    return Ok(vagaCriada);
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }

        // [HttpGet("{estacionamentoId}/vagas")]
        // public IActionResult ObterVagasEstacionamento(Guid estacionamentoId)
        // {
        //     try
        //     {
        //         var vagas = _proprietarioService.ObterVagasEstacionamento(estacionamentoId);

        //         return Ok(vagas);
        //     }
        //     catch (Exception ex)
        //     {
        //         return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //     }
        // }
        [HttpGet("{estacionamentoId}/vagas")]
        public async Task<ActionResult<List<Vaga>>> ObterVagasEstacionamento(Guid estacionamentoId)
        {
            try
            {
                var vagas = await _proprietarioService.ObterVagasEstacionamento(estacionamentoId);

                return Ok(vagas);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [HttpDelete("excluir/{vagaId}")]
        public async Task<IActionResult> ExcluirVaga(Guid vagaId)
        {
            try
            {
                var vagaExcluida = await _proprietarioService.ExcluirVaga(vagaId);

                if (vagaExcluida)
                {
                    return Ok("Vaga excluída com sucesso.");
                }
                else
                {
                    return BadRequest("Não foi possível excluir a vaga. Certifique-se de que a vaga está disponível para exclusão e o proprietário está correto.");
                }
            }
            catch (ApplicationException ex)
            {
                return BadRequest($"Erro ao excluir vaga: {ex.Message}");
            }
        }

        [HttpGet("{estacionamentoId}/reservas")]
        public IActionResult ObterVagasOcupadasPorEstacionamento(Guid estacionamentoId)
        {
            try
            {
                var vagasOcupadas = _proprietarioService.ObterVagasOcupadasPorEstacionamento(estacionamentoId);
                return Ok(vagasOcupadas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("liberar-reserva/{vagaId}")]
        public IActionResult CancelarReserva(Guid vagaId)
        {
            try
            {
                bool cancelamentoSucesso = _proprietarioService.CancelarReserva(vagaId);

                if (cancelamentoSucesso)
                {
                    return Ok("Reserva cancelada com sucesso!");
                }
                else
                {
                    return BadRequest("Não foi possível cancelar a reserva. Verifique o status da vaga.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}