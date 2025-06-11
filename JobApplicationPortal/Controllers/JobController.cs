using System.Security.Claims;
using JobApplicationPortal.Dto;
using JobApplicationPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationPortal.Controllers;

[ApiController]
[Route("api/")]
public class JobController : ControllerBase
{
    private readonly JobApplicationPortalContext _context;
    public JobController(JobApplicationPortalContext context)
    {
        _context = context;

    }
 
    [HttpPost("create/job")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Employer")]
    public async Task<IActionResult> CreateJob([FromBody] JobDto createJobDto)
    {
        if (createJobDto == null || !ModelState.IsValid)
        {
            return BadRequest("Invalid job data.");
        }

        var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var email = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized("User is not authenticated.");
            }

            var employer = _context.Employers
                .Include(e => e.User)
                .FirstOrDefault(e => e.User.Email == email);

            if (_context.Jobs.Any(j => j.Title == createJobDto.Title && j.EmployerId == employer.Id && (bool)!j.IsDeleted))
            {
                return BadRequest("Job with the same title already exists for this employer.");
            }

            var job = new Job
            {
                Title = createJobDto.Title,
                Description = createJobDto.Description,
                Location = createJobDto.Location,
                ExperienceRequired = createJobDto.ExperienceRequired,
                EmployerId = employer.Id,
                OpenFrom = createJobDto.OpenFrom,
                CreatedAt = DateTime.Now,
                IsActive = true,
                IsDeleted = false,
            };
            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();

            if (createJobDto.skillsRequiredList != null && createJobDto.skillsRequiredList.Any())
            {
                foreach (var skill in createJobDto.skillsRequiredList)
                {
                    if (_context.Skills.Any(s => s.Name == skill.Name))
                    {
                        var jobSkill = new JobSkill
                        {
                            JobId = job.Id,
                            SkillId = _context.Skills.FirstOrDefault(s => s.Name == skill.Name)?.Id ?? skill.Id,
                        };
                        _context.JobSkills.Add(jobSkill);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        return BadRequest("Such skill is not present");
                    }
                }
            }
            await transaction.CommitAsync();
            return StatusCode(StatusCodes.Status201Created, "Job created successfully.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("get/jobs")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Candidate, Admin")]
    public IActionResult GetJobs()
    {
        try
        {
            var jobs = _context.Jobs
                .Include(j => j.Employer)
                .Include(j => j.JobSkills)
                .ThenInclude(js => js.Skill)
                .Where(j => (bool)!j.IsDeleted && (bool)j.IsActive)
                .Select(j => new JobDto
                {
                    Title = j.Title,
                    Description = j.Description,
                    Location = j.Location,
                    ExperienceRequired = j.ExperienceRequired,
                    OpenFrom = j.OpenFrom,
                    skillsRequiredList = j.JobSkills.Select(js => new SkillDto
                    {
                        Id = js.Skill.Id,
                        Name = js.Skill.Name
                    }).ToList()
                }).ToList();


            var email = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var candidate = _context.Candidates
                .Include(e => e.User)
                .FirstOrDefault(e => e.User.Email == email);
            
            if(candidate != null){
                jobs = jobs.Where(j => j.OpenFrom <= DateOnly.FromDateTime(DateTime.Now)).ToList();
            }

            return Ok(jobs);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("get/created-jobs")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Employer")]
    public IActionResult GetCreatedJobs()
    {

        try
        {
            var email = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized("User is not authenticated.");
            }
            var employer = _context.Employers
                .Include(e => e.User)
                .FirstOrDefault(e => e.User.Email == email);
            if (employer == null)
            {
                return NotFound("Employer not found.");
            }

            var jobs = _context.Jobs
                .Include(j => j.Employer)
                .Include(j => j.JobSkills)
                .ThenInclude(js => js.Skill)
                .Where(j => (bool)!j.IsDeleted && (bool)j.IsActive && j.EmployerId == employer.Id)
                .Select(j => new JobDto
                {
                    Title = j.Title,
                    Description = j.Description,
                    Location = j.Location,
                    ExperienceRequired = j.ExperienceRequired,
                    OpenFrom = j.OpenFrom,
                    skillsRequiredList = j.JobSkills.Select(js => new SkillDto
                    {
                        Id = js.Skill.Id,
                        Name = js.Skill.Name
                    }).ToList()
                }).ToList();

            return Ok(jobs);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
        }
    }


    [HttpGet("get/jobs-by-employer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin")]
    public IActionResult GetJobsByEmployer(int employerId)
    {
        if (!_context.Employers.Any(e => e.Id == employerId))
        {
            return BadRequest("Employer not found.");
        }

        try
        {
            var jobs = _context.Jobs
                .Include(j => j.Employer)
                .Include(j => j.JobSkills)
                .ThenInclude(js => js.Skill)
                .Where(j => (bool)!j.IsDeleted && (bool)j.IsActive && j.EmployerId == employerId)
                .Select(j => new JobDto
                {
                    Title = j.Title,
                    Description = j.Description,
                    Location = j.Location,
                    ExperienceRequired = j.ExperienceRequired,
                    OpenFrom = j.OpenFrom,
                    skillsRequiredList = j.JobSkills.Select(js => new SkillDto
                    {
                        Id = js.Skill.Id,
                        Name = js.Skill.Name
                    }).ToList()
                }).ToList();

            return Ok(jobs);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPut("delete/job")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Employer")]
    public async Task<IActionResult> DeleteJob(int jobId)
    {
        try
        {
            var email = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized("User is not authenticated.");
            }

            var employer = _context.Employers
                .Include(e => e.User)
                .FirstOrDefault(e => e.User.Email == email);

            if(employer == null)
            {
                return BadRequest("Employer not found.");
            }

            var jobIdCheck = _context.Jobs.FirstOrDefault(j => j.Id == jobId);
            if(jobIdCheck == null){
                return BadRequest("Job not found.");
            }

            if (_context.Jobs.Any(j => j.Id == jobId && j.EmployerId != employer.Id))
            {
                return BadRequest("Job is not created by the employer.");
            }

            if (_context.Jobs.Any(j => j.Id == jobId && j.EmployerId == employer.Id && (bool)j.IsDeleted))
            {
                return BadRequest("Job already deleted.");
            }

            var job = _context.Jobs.FirstOrDefault(j => j.Id == jobId && j.EmployerId == employer.Id);
            job.IsDeleted = true;

            _context.Jobs.Update(job);
            await _context.SaveChangesAsync();

            return StatusCode(StatusCodes.Status201Created, "Job Deleted successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("get/skills")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin, Employer")]
    public IActionResult GetSkills(){
        try{
            var skills = _context.Skills
                .Select(s => new SkillDto
                {
                    Id = s.Id,
                    Name = s.Name
                }).ToList();

            return Ok(skills);
        }catch(Exception ex){
            return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
        }
    }
}
