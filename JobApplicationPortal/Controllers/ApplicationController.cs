using Microsoft.AspNetCore.Mvc;
using JobApplicationPortal.Service.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using JobApplicationPortal.DataModels.Dtos.RequestDtos;

namespace JobApplicationPortal.Controllers;

[ApiController]
[Route("api/")]
public class ApplicationController : ControllerBase
{
    private readonly IApplicationService _applicationService;
    public ApplicationController(IApplicationService applicationService)
    {
        _applicationService = applicationService;
    }

    [HttpPost("job/application")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Candidate")]
    public async Task<IActionResult> JobApplication([FromForm] ApplicationDto applicationDto)
    {
        var result = await _applicationService.JobApplication(applicationDto);
        if (result.StatusCode == 201)
        {
            return Ok(result.Data);
        }
        else if (result.StatusCode == 400)
        {
            return BadRequest(result.Message);
        }
        else if (result.StatusCode == 401)
        {
            return Unauthorized(result.Message);
        }
        else if (result.StatusCode == 403)
        {
            return Forbid(result.Message);
        }
        else
        {
            return StatusCode(StatusCodes.Status500InternalServerError, result.Message);
        }
    }

    [HttpGet("get/applications")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin")]
    public IActionResult GetApplications()
    {
        var result = _applicationService.GetApplications();
        if (result.StatusCode == 200)
        {
            return Ok(result.Data);
        }
        else if (result.StatusCode == 400)
        {
            return BadRequest(result.Message);
        }
        else
        {
            return StatusCode(StatusCodes.Status500InternalServerError, result.Message);
        }
    }

    [HttpGet("get/applications-by-candidate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Candidate")]
    public IActionResult GetApplicationsByCandidate()
    {
        var result = _applicationService.GetApplicationsByCandidate();
        if (result.StatusCode == 200)
        {
            return Ok(result.Data);
        }
        else if (result.StatusCode == 400)
        {
            return BadRequest(result.Message);
        }
        else if (result.StatusCode == 401)
        {
            return Unauthorized(result.Message);
        }
        else if (result.StatusCode == 403)
        {
            return Forbid(result.Message);
        }
        else
        {
            return StatusCode(StatusCodes.Status500InternalServerError, result.Message);
        }
    }

    [HttpGet("get/applications-by-job")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Employer")]
    public IActionResult GetApplicationsByJob(int jobId)
    {
        var result = _applicationService.GetApplicationsByJob(jobId);
        if (result.StatusCode == 200)
        {
            return Ok(result.Data);
        }
        else if (result.StatusCode == 400)
        {
            return BadRequest(result.Message);
        }
        else if (result.StatusCode == 401)
        {
            return Unauthorized(result.Message);
        }
        else if (result.StatusCode == 403)
        {
            return Forbid(result.Message);
        }
        else
        {
            return StatusCode(StatusCodes.Status500InternalServerError, result.Message);
        }
    }

    [HttpPut("application/change-status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Employer")]
    public async Task<IActionResult> UpdateStatus(int applicationId, int statusId)
    {
        var result = await _applicationService.UpdateStatus(applicationId, statusId);
        if (result.StatusCode == 200)
        {
            return Ok(result.Message);
        }
        else if (result.StatusCode == 400)
        {
            return BadRequest(result.Message);
        }
        else if (result.StatusCode == 401)
        {
            return Unauthorized(result.Message);
        }
        else if (result.StatusCode == 403)
        {
            return Forbid(result.Message);
        }
        else
        {
            return StatusCode(StatusCodes.Status500InternalServerError, result.Message);
        }
    }

    [HttpGet("get/statuses")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin, Employer")]
    public IActionResult GetStatuses()
    {
        var result = _applicationService.GetStatuses();
        if (result.StatusCode == 200)
        {
            return Ok(result.Data);
        }
        else
        {
            return StatusCode(StatusCodes.Status500InternalServerError, result.Message);
        }
    }

    [HttpGet("get/job/total-applications/")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin, Employer")]
    public IActionResult GetTotalApplicationByJob(int jobId)
    {
        var result = _applicationService.GetTotalApplicationByJob(jobId);
        if (result.StatusCode == 200)
        {
            return Ok(result.Data);
        }
        else if (result.StatusCode == 400)
        {
            return BadRequest(result.Message);
        }
        else if (result.StatusCode == 401)
        {
            return Unauthorized(result.Message);
        }
        else if (result.StatusCode == 403)
        {
            return Forbid(result.Message);
        }
        else
        {
            return StatusCode(StatusCodes.Status500InternalServerError, result.Message);
        }
    }
}
