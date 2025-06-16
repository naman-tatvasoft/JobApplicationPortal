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
        return Ok(result.Data);

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
        return StatusCode(StatusCodes.Status201Created, result.Data);

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
        return Ok(result.Data);

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
        return Ok(result.Data);

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
        return Ok(result.Message);

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
        return Ok(result.Data);

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
        return Ok(result.Data);
    }

    [HttpPut("application/withdraw-application")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Candidate")]
    public async Task<IActionResult> WithdrawApplication(int applicationId)
    {
        var result = await _applicationService.WithdrawApplication(applicationId);
        return Ok(result.Message);
    }
}
