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

    [HttpPost("application")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Candidate")]
    public async Task<IActionResult> JobApplication([FromForm] ApplicationDto applicationDto)
    {
        var result = await _applicationService.JobApplication(applicationDto);
        return StatusCode(StatusCodes.Status201Created, result.Data);
    }

    [HttpGet("applications")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin")]
    public IActionResult GetApplications([FromQuery] string search = "", [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5,
                                    [FromQuery] string status = "")
    {
        var result = _applicationService.GetApplications(search, pageNumber, pageSize, status);
        return StatusCode(StatusCodes.Status201Created, result.Data);
    }

    [HttpGet("applications-by-candidate")]
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

    [HttpGet("applications-by-job/{jobId}")]
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

    [HttpPut("application/{applicationId}/change-status/{statusId}")]
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

    [HttpGet("statuses")]
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

    [HttpGet("job/{jobId}/total-applications/")]
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

    [HttpPut("application/withdraw-application/{applicationId}")]
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


    [HttpGet("application/{applicationId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Employer, Admin")]
    public IActionResult GetApplicationById(int applicationId)
    {
        var result = _applicationService.GetApplicationById(applicationId);
        return Ok(result.Data);
    }


}
