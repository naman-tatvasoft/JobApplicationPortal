using System.Data;
using JobApplicationPortal.DataModels.Dtos.RequestDtos;
using JobApplicationPortal.DataModels.Dtos.ResponseDtos;
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

    [HttpPost("job")]
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

    [HttpGet("job/{jobId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Employer, Admin")]
    public IActionResult GetJobsById(int jobId)
    {
        var result = _jobService.GetJobById(jobId);
        return Ok(result.Data);
    }

    [HttpPut("job/{jobId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Employer")]
    public async Task<IActionResult> UpdateJob(int jobId, [FromBody] JobDto updateJobDto)
    {
        var result = await _jobService.UpdateJob(jobId, updateJobDto);
        return Ok(result.Data);
    }

    [HttpGet("jobs")]
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

    [HttpGet("created-jobs")]
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

    [HttpGet("jobs-by-employer/{employerId}")]
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

    [HttpPut("delete/job/{jobId}")]
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

    [HttpGet("skills")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetSkills()
    {
        var result = _jobService.GetSkills();
        return Ok(result.Data);
    }

    [HttpGet("categories")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetCategories()
    {
        var result = _jobService.GetCategories();
        return Ok(result.Data);
    }

    [HttpPost("job-preference")]
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

    [HttpPut("job-preference/{jobPreferenceId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Candidate")]
    public async Task<IActionResult> UpdateJobPreference(int jobPreferenceId, [FromBody] JobPreferenceDto jobPreferenceDto)
    {
        var result = await _jobService.UpdateJobPreference(jobPreferenceId, jobPreferenceDto);
        return StatusCode(StatusCodes.Status200OK, result.Data);
    }

    [HttpDelete("job-preference/{jobPreferenceId}")]
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

    [HttpPost("category")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateCategory([FromBody] CategoriesDto catDto)
    {
        var result = await _jobService.CreateCategory(catDto);
        return StatusCode(StatusCodes.Status201Created, result.Data);
    }

    [HttpPost("skill")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateSkill([FromBody] SkillDto skillDto)
    {
        var result = await _jobService.CreateSkill(skillDto);
        return StatusCode(StatusCodes.Status201Created, result.Data);
    }

    [HttpGet("skill/{skillId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin")]
    public IActionResult GetSkillById(int skillId)
    {
        var skillName = _jobService.GetSkillNameById(skillId);
        if (string.IsNullOrEmpty(skillName))
        {
            return NotFound("skill not found.");
        }
        return Ok(new { name = skillName });
    }

    [HttpPut("skill/{skillId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateSkill(int skillId, [FromBody] SkillDto skillDto)
    {
        var result = await _jobService.UpdateSkill(skillId, skillDto);
        return Ok(result.Data);
    }

    [HttpGet("category/{categoryId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin")]
    public IActionResult GetCategoryById(int categoryId)
    {
        var categoryName = _jobService.GetCategoryNameById(categoryId);
        if (string.IsNullOrEmpty(categoryName))
        {
            return NotFound("category not found.");
        }
        return Ok(new { name = categoryName });
    }

    [HttpPut("category/{categoryId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateCategory(int categoryId, [FromBody] CategoriesDto categoriesDto)
    {
        var result = await _jobService.UpdateCategory(categoryId, categoriesDto);
        return Ok(result.Data);
    }

    [HttpDelete("category/{categoryId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCategory(int categoryId)
    {
        var result = await _jobService.DeleteCategory(categoryId);
        return Ok(result.Message);
    }

    [HttpDelete("skill/{skillId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteSkill(int skillId)
    {
        var result = await _jobService.DeleteSkill(skillId);
        return Ok(result.Message);
    }
}
