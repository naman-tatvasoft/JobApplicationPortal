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
        if (result.StatusCode == 201)
        {
            return StatusCode(StatusCodes.Status201Created, result.Message);
        }
        else if (result.StatusCode == 400)
        {
            return BadRequest(result.Message);
        }
        else if (result.StatusCode == 401)
        {
            return Unauthorized(result.Message);
        }
        else
        {
            return StatusCode(result.StatusCode, result.Message);
        }
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
        
        if (result.StatusCode == 200)
        {
            return Ok(result.Data);
        }
        else if (result.StatusCode == 401){
            return Unauthorized(result.Message);
        }
        else
        {
            return StatusCode(result.StatusCode, result.Message);
        }
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
        
        if (result.StatusCode == 200)
        {
            return Ok(result.Data);
        }
        else if(result.StatusCode == 400){
            return BadRequest(result.Message);
        }
        else if (result.StatusCode == 401){
            return Unauthorized(result.Message);
        }
        else
        {
            return StatusCode(result.StatusCode, result.Message);
        }
    }

    [HttpGet("get/jobs")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Candidate, Admin")]
    public IActionResult GetJobs([FromQuery] string search = "", [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 2,
                                    [FromQuery]  string skill = "", [FromQuery]  string location = "", [FromQuery]  int experience = 0)
    {
        var result = _jobService.GetJobs(search, pageNumber, pageSize, skill, location, experience);
        
        if (result.StatusCode == 200)
        {
            return Ok(result.Data);
        }
        else if (result.StatusCode == 401){
            return Unauthorized(result.Message);
        }
        else
        {
            return StatusCode(result.StatusCode, result.Message);
        }
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
     
        if (result.StatusCode == 200)
        {
            return Ok(result.Data);
        }
        else if (result.StatusCode == 400){
            return BadRequest(result.Message);
        }
        else if(result.StatusCode == 401){
            return Unauthorized(result.Message);
        }
        else
        {
            return StatusCode(result.StatusCode, result.Message);
        }
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
        if (result.StatusCode == 200)
        {
            return Ok(result.Data);
        }
        else if (result.StatusCode == 400){
            return BadRequest(result.Message);
        }
        else
        {
            return StatusCode(result.StatusCode, result.Message);
        }
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
        if (result.StatusCode == 201)
        {
            return StatusCode(StatusCodes.Status201Created, result.Message);
        }
        else if (result.StatusCode == 400)
        {
            return BadRequest(result.Message);
        }
        else
        {
            return StatusCode(result.StatusCode, result.Message);
        }
    }

    [HttpGet("get/skills")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin, Employer")]
    public IActionResult GetSkills()
    {
        var result = _jobService.GetSkills();
        if (result.StatusCode == 200)
        {
            return Ok(result.Data);
        }
        else
        {
            return StatusCode(result.StatusCode, result.Message);
        }
    }
}
