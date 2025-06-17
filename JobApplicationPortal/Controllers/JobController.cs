using System.Data;
using JobApplicationPortal.DataModels.Dtos.RequestDtos;
using JobApplicationPortal.Service.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobApplicationPortal.Controllers;

[ApiController]
[Route("api/")]
public class JobController : ControllerBase
{
    private readonly IJobService _jobService;
    public JobController(IJobService jobService)
    {
        _jobService = jobService;
    }

    [HttpPost("create/job")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Employer")]
    public async Task<IActionResult> CreateJob([FromBody] JobDto createJobDto)
    {
        var result = await _jobService.CreateJob(createJobDto);
        return StatusCode(StatusCodes.Status201Created, result.Message);
    }

    [HttpGet("get/job-by-id")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Employer")]
    public IActionResult GetJobsById(int jobId)
    {
        var result = _jobService.GetJobById(jobId);
        return Ok(result.Data);
    }

    [HttpPut("update/job")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Employer")]
    public async Task<IActionResult> UpdateJob([FromBody] JobDto updateJobDto)
    {
        var result = await _jobService.UpdateJob(updateJobDto);
        return Ok(result.Data);
    }

    [HttpGet("get/jobs")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Candidate, Admin")]
    public IActionResult GetJobs([FromQuery] string search = "", [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 2,
                                    [FromQuery] string skill = "", [FromQuery] string location = "", [FromQuery] int experience = 0, [FromQuery] string category = "")
    {
        var result = _jobService.GetJobs(search, pageNumber, pageSize, skill, location, experience, category);
        return Ok(result.Data);
    }

    [HttpGet("get/created-jobs")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Employer")]
    public IActionResult GetCreatedJobs()
    {
        var result = _jobService.GetCreatedJobs();
        return Ok(result.Data);
    }


    [HttpGet("get/jobs-by-employer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin")]
    public IActionResult GetJobsByEmployer(int employerId)
    {
        var result = _jobService.GetJobsByEmployer(employerId);
        return Ok(result.Data);

    }

    [HttpPut("delete/job")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Employer")]
    public async Task<IActionResult> DeleteJob(int jobId)
    {
        var result = await _jobService.DeleteJob(jobId);
        return StatusCode(StatusCodes.Status201Created, result.Message);
    }

    [HttpGet("get/skills")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetSkills()
    {
        var result = _jobService.GetSkills();
        return Ok(result.Data);
    }

    [HttpGet("get/categories")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetCategories()
    {
        var result = _jobService.GetCategories();
        return Ok(result.Data);
    }

    [HttpPost("create/job-preference")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Candidate")]
    public async Task<IActionResult> CreateJobPreference([FromBody] JobPreferenceDto jobPreferenceDto)
    {
        var result = await _jobService.CreateJobPreference(jobPreferenceDto);
        return StatusCode(StatusCodes.Status201Created, result.Data);
    }

    [HttpPut("update/job-preference")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Candidate")]
    public async Task<IActionResult> UpdateJobPreference([FromBody] JobPreferenceDto jobPreferenceDto)
    {
        var result = await _jobService.UpdateJobPreference(jobPreferenceDto);
        return StatusCode(StatusCodes.Status200OK, result.Data);
    }

    [HttpDelete("delete/job-preference")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Candidate")]
    public async Task<IActionResult> DeleteJobPreference(int jobPreferenceId)
    {
        var result = await _jobService.DeleteJobPreference(jobPreferenceId);
        return StatusCode(StatusCodes.Status200OK, result.Message);
    }

}
