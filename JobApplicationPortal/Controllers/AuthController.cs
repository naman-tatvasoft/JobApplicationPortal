using JobApplicationPortal.Dto;
using JobApplicationPortal.Helper;
using JobApplicationPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationPortal.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{

    private readonly JobApplicationPortalContext _context;
    private readonly JwtService _jwtService;

    public AuthController(JobApplicationPortalContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    
    [HttpPost("register/employer")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RegisterEmployer([FromBody] RegisterEmployerDto registerEmployerDto)
    {
        if (registerEmployerDto == null || !ModelState.IsValid)
        {
            return BadRequest("Invalid registration data.");
        }
        if (_context.Users.Any(u => u.Email == registerEmployerDto.Email))
        {
            return BadRequest("Email already exists.");
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var hashedPassword = PasswordHasher.Hash(registerEmployerDto.Password);

            var user = new User
            {
                Email = registerEmployerDto.Email,
                Password = hashedPassword,
                RoleId = _context.Roles.FirstOrDefault(r => r.Name == "Employer")?.Id ?? 2
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var employer = new Employer
            {
                Name = registerEmployerDto.Name,
                CompanyName = registerEmployerDto.CompanyName,
                UserId = user.Id
            };
            _context.Employers.Add(employer);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            return StatusCode(StatusCodes.Status201Created);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost("register/candidate")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RegisterCandidate([FromBody] RegisterCandidateDto registerCandidateDto)
    {
        if (registerCandidateDto == null || !ModelState.IsValid)
        {
            return BadRequest("Invalid registration data.");
        }
        if (_context.Users.Any(u => u.Email == registerCandidateDto.Email))
        {
            return BadRequest("Email already exists.");
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var hashedPassword = PasswordHasher.Hash(registerCandidateDto.Password);

            var user = new User
            {
                Email = registerCandidateDto.Email,
                Password = hashedPassword,
                RoleId =  _context.Roles.FirstOrDefault(r => r.Name == "Candidate")?.Id ?? 3
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var candidate = new Candidate
            {
                Name = registerCandidateDto.Name,
                UserId = user.Id
            };
            _context.Candidates.Add(candidate);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            return StatusCode(StatusCodes.Status201Created);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
    {
        if (loginDto == null || !ModelState.IsValid)
        {
            return BadRequest("Invalid registration data.");
        }

        try
        {
            var user = _context.Users
            .Include(u => u.Role)
            .FirstOrDefault(u => u.Email == loginDto.Email);

            if (user == null)
            {
                return BadRequest("Email does not exists.");
            }

            if (!PasswordHasher.Verify(user.Password, loginDto.Password))
            {
                return Unauthorized("Invalid password.");
            }

            var token = _jwtService.GenerateToken(user.Email, user.Role.Name);

            return Ok(new { token });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
        }
    }

}
