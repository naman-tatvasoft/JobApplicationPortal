using JobApplicationPortal.Dto;
using JobApplicationPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationPortal.Controllers;

[ApiController]
[Route("api/")]
public class UserController : ControllerBase
{

    private readonly JobApplicationPortalContext _context;

    public UserController(JobApplicationPortalContext context){
        _context = context;
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Authorize(Roles = "Admin")]
    [HttpGet("get/employers")]
    public IActionResult GetEmployers(){
        var employersInfoList = _context.Employers.Include(e => e.User).Select(c => new EmployerInfoDto{
            Id = c.Id,
            Name = c.Name,
            CompanyName = c.CompanyName,
            Email = c.User.Email
        }).ToList();

        return Ok(employersInfoList);
    }

    
    [HttpGet("get/candidates")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Authorize(Roles = "Admin")]
    public IActionResult GetCandidates(){
        var candidatesInfoList = _context.Candidates.Include(e => e.User).Select(c => new CandidateInfoDto{
            Id = c.Id,
            Name = c.Name,
            Email = c.User.Email
        }).ToList();

        return Ok(candidatesInfoList);
    }

}
