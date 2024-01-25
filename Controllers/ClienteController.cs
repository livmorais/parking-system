using Api_dotnet.Models;
using Api_dotnet.Services;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api_dotnet.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class ClienteController : ControllerBase 
    {
        
        private readonly IProprietarioService _proprietarioService;
        private readonly IClienteService _clienteService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        public ClienteController(IClienteService clienteService, IHttpContextAccessor httpContextAccessor, IProprietarioService proprietarioService)
        {
            _clienteService = clienteService;
            _httpContextAccessor = httpContextAccessor;
            _proprietarioService = proprietarioService;
        }

        [HttpPost("/reservar/{estacionamentoId}")]
        public IActionResult FazerReserva(Guid estacionamentoId, [FromBody] VagaDTO vagaDto)
        {
            try
            {

                TipoVaga tipoVaga = vagaDto.Tipo;

                bool reservaSucesso = _proprietarioService.FazerReserva(estacionamentoId, tipoVaga);

                if (reservaSucesso)
                {
                    return Ok("Reserva realizada com sucesso!");
                }
                else
                {
                    return BadRequest("Não foi possível realizar a reserva. Verifique se há vagas disponíveis.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("reservas")]
        public IActionResult ObterVagasReservadasCliente()
        {
            try
            {
                var vagasReservadas = _proprietarioService.ObterVagasReservadasCliente();

                return Ok(vagasReservadas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("cancelar-reserva/{vagaId}")]
        public IActionResult CancelarReserva(Guid vagaId)
        {
            try
            {
                bool cancelamentoSucesso = _proprietarioService.CancelarReservaCliente(vagaId);

                if (cancelamentoSucesso)
                {
                    return Ok("Reserva cancelada com sucesso!");
                }
                else
                {
                    return BadRequest("Não foi possível cancelar a reserva. Verifique se a vaga pertence ao cliente.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
