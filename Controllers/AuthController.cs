using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Api_dotnet.Models;
using Api_dotnet.Services;
using Microsoft.AspNetCore.Http;


namespace Api_dotnet.Controllers
{   
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public static Proprietario proprietario = new Proprietario();
        public static Cliente cliente = new Cliente();
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public AuthController(IConfiguration configuration, IUserService userService)
        {
            _configuration = configuration;
            _userService = userService;
        }

[HttpPost("cadastro")]
public ActionResult<object> Cadastro(CadastroDto request)
{
    try
    {
        if (request.IsCliente)
        {
            var existingUser = _userService.GetClienteByName(request.Email);
            if (existingUser != null)
            {
                return BadRequest("User already exists.");
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            Guid clienteId = Guid.NewGuid();

            var newCliente = new Cliente
            {
                ClienteId = clienteId,
                Email = request.Email,
                Nome = request.Nome,
                Endereco = request.Endereco,
                Modelo = request.Modelo,
                Placa = request.Placa,
                Telefone = request.Telefone,
                PasswordHash = passwordHash,
                Role = "User"
            };

            _userService.RegisterCliente(newCliente);

            return Ok(newCliente);
        }
        else
        {
            var existingUser = _userService.GetProprietarioByName(request.Email);
            if (existingUser != null)
            {
                return BadRequest("User already exists.");
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            Guid proprietarioId = Guid.NewGuid();

            var newProprietario = new Proprietario
            {
                ProprietarioId = proprietarioId,
                Email = request.Email,
                Nome = request.Nome,
                Telefone = request.Telefone,
                Endereco = request.Endereco,
                PasswordHash = passwordHash,
                Role = "Admin"
            };

            _userService.RegisterProprietario(newProprietario);

            return Ok(newProprietario);
        }
    }
    catch (Exception ex)
    {
        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
    }
}




        [HttpPost("login")]
public ActionResult<string> Login(LoginDto request)
{
    try
    {
        var proprietario = _userService.GetProprietarioByName(request.Email);
        var cliente = _userService.GetClienteByName(request.Email);

        if (proprietario != null && cliente != null)
        {
            return BadRequest("Duplicate email. Please specify the user type.");
        }
        else if (proprietario != null)
        {
            if (!BCrypt.Net.BCrypt.Verify(request.Password, proprietario.PasswordHash))
            {
                return BadRequest("Wrong password.");
            }

            var token = CreateToken(proprietario);

            return Ok(token);
        }
        else if (cliente != null)
        {
            if (!BCrypt.Net.BCrypt.Verify(request.Password, cliente.PasswordHash))
            {
                return BadRequest("Wrong password.");
            }

            var token = CreateTokenCliente(cliente);

            return Ok(token);
        }
        else
        {
            return BadRequest("User not found.");
        }
    }
    catch (Exception ex)
    {
        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
    }
}


        [HttpPost("refresh-token")]
        public async Task<ActionResult<string>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if(!proprietario.RefreshToken.Equals(refreshToken)) {
                return Unauthorized("Invalid Refresh Token");
            }
            else if(proprietario.TokenExpires < DateTime.Now) {
                return Unauthorized("Token expired.");
            }

            string token = CreateToken(proprietario);
            var newRefreshToken = GenerateRefreshToken();
            SetRefreshToken(newRefreshToken);

            return Ok(token);
        }

        private RefreshToken GenerateRefreshToken(){
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddDays(7)
            };
            return refreshToken;
        }

        private void SetRefreshToken(RefreshToken newRefreshToken)
        {
            var cookieOptions = new CookieOptions 
            {
                HttpOnly = true,
                Expires = newRefreshToken.Expires,
            };
            Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);

            proprietario.RefreshToken = newRefreshToken.Token;
            proprietario.TokenCreated = newRefreshToken.Created;
            proprietario.TokenExpires = newRefreshToken.Expires;
        }

        private string CreateToken(Proprietario proprietario) {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, proprietario.Email),
                new Claim("Nome", proprietario.Nome),
                new Claim("Telefone", proprietario.Telefone),
                new Claim("Endereco", proprietario.Endereco),
                new Claim(ClaimTypes.NameIdentifier, proprietario.ProprietarioId.ToString())
            };

            if (proprietario.Role == "Admin")
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }
            else
            {
                claims.Add(new Claim(ClaimTypes.Role, "User"));
            }


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value!
            ));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }




        private string CreateTokenCliente(Cliente cliente) {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, cliente.Email),
                new Claim("Nome", cliente.Nome),
                new Claim("Telefone", cliente.Telefone),
                new Claim("Endereco", cliente.Endereco),
                new Claim("Modelo", cliente.Modelo),
                new Claim("Placa", cliente.Placa),
                new Claim(ClaimTypes.NameIdentifier, cliente.ClienteId.ToString())
                
            };

            if (cliente.Role == "User")
            {
                claims.Add(new Claim(ClaimTypes.Role, "User"));
            }
            else
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value!
            ));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }

}