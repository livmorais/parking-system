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

    public class EstacionamentoController : ControllerBase
    {
        private readonly IProprietarioService _proprietarioService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EstacionamentoController(IProprietarioService proprietarioService, IHttpContextAccessor httpContextAccessor)
        {
            _proprietarioService = proprietarioService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("estacionamentos")]
        public IActionResult ObterInformacoesEstacionamentos()
        {
            try
            {
                var informacoesEstacionamentos = _proprietarioService.ObterInformacoesEstacionamentos();

                if (informacoesEstacionamentos != null && informacoesEstacionamentos.Any())
                {
                    return Ok(informacoesEstacionamentos);
                }
                else
                {
                    return NotFound("Nenhuma informação de estacionamento disponível.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}