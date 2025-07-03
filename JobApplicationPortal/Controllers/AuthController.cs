using JobApplicationPortal.DataModels.Dtos.RequestDtos;
using JobApplicationPortal.DataModels.Dtos.ResponseDtos;
using JobApplicationPortal.Service.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobApplicationPortal.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{

    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("employer")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterEmployer([FromBody] RegisterEmployerDto registerEmployerDto)
    {
        var result = await _authService.RegisterEmployer(registerEmployerDto);
        return StatusCode(StatusCodes.Status201Created, result.Message);
    }

    [HttpPost("candidate")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterCandidate([FromBody] RegisterCandidateDto registerCandidateDto)
    {
        var result = await _authService.RegisterCandidate(registerCandidateDto);
        return StatusCode(StatusCodes.Status201Created, result.Message);

    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
    {
        var result = await _authService.Login(loginDto);
        return Ok(new { data = result.Data });
    }

    [HttpGet("test/token")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [AllowAnonymous]
    public async Task<IActionResult> TestToken([FromQuery] string token)
    {
        var result = await _authService.TestToken(token);
        return Ok(new { data = result });
    }

}
