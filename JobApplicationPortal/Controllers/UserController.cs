using JobApplicationPortal.DataModels.Dtos.RequestDtos;
using JobApplicationPortal.Service.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobApplicationPortal.Controllers;

[ApiController]
[Route("api/")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("employers")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Authorize(Roles = "Admin")]
    public IActionResult GetEmployers()
    {
        var result = _userService.GetEmployers();
        return Ok(result.Data);

    }


    [HttpGet("candidates")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Authorize(Roles = "Admin")]
    public IActionResult GetCandidates()
    {
        var result = _userService.GetCandidates();
        return Ok(result.Data);
    }

    [HttpGet("profile")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Authorize(Roles = "Candidate,Employer")]
    public IActionResult GetProfile()
    {
        var result = _userService.GetProfile();
        return Ok(result.Data);
    }

    [HttpPut("employer-profile")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Authorize(Roles = "Employer")]
    public async Task<IActionResult> UpdateEmployerProfile([FromBody] UpdateEmployerProfileDto updateEmployerProfileDto)
    {
        var result = await _userService.UpdateEmployerProfile(updateEmployerProfileDto);
        return Ok(result.Data);
    }

    [HttpPut("candidate-profile")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Authorize(Roles = "Candidate")]
    public async Task<IActionResult> UpdateCandidateProfile([FromBody] UpdateCandidateProfileDto updateCandidateProfileDto)
    {
        var result = await _userService.UpdateCandidateProfile(updateCandidateProfileDto);
        return Ok(result.Data);
    }



}
