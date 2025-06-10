using System.Security.Claims;
using JobApplicationPortal.Dto;
using Microsoft.AspNetCore.Authorization;
using JobApplicationPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationPortal.Controllers;

[ApiController]
[Route("api/")]
public class ApplicationController : ControllerBase
{
    private readonly JobApplicationPortalContext _context;
    public ApplicationController(JobApplicationPortalContext context)
    {
        _context = context;
    }

    [HttpPost("job/application")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Candidate")]
    public async Task<IActionResult> JobApplication([FromForm] ApplicationDto applicationDto)
    {
        try
        {
            var email = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized("User is not authenticated.");
            }

            var candidate = _context.Candidates
                .Include(e => e.User)
                .FirstOrDefault(e => e.User.Email == email);
            if (candidate == null)
            {
                return BadRequest("Candidate not found.");
            }

            var jobIdCheck = _context.Jobs.FirstOrDefault(j => j.Id == applicationDto.JobId && (bool)j.IsActive && (bool)!j.IsDeleted);
            if (jobIdCheck == null)
            {
                return BadRequest("Job not found or not active or deleted or not still open to apply.");
            }

            var isAlreadyApplied = _context.Applications.FirstOrDefault(a => a.JobId == applicationDto.JobId && a.CandidateId == candidate.Id);
            if (isAlreadyApplied != null)
            {
                return BadRequest("You have already applied for this job");
            }

            var coverLetterFileName = string.Empty;
            if (applicationDto.CoverLetter != null && applicationDto.CoverLetter.Length > 0)
            {
                var coverLetterPath = Path.Combine("uploads", "CoverLetters");
                if (!Directory.Exists(coverLetterPath))
                {
                    Directory.CreateDirectory(coverLetterPath);
                }

                coverLetterFileName = $"{Guid.NewGuid()}_{applicationDto.CoverLetter.FileName}";
                var filePath = Path.Combine(coverLetterPath, coverLetterFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await applicationDto.CoverLetter.CopyToAsync(stream);
                }
            }

            var resumeFileName = string.Empty;
            if (applicationDto.Resume != null && applicationDto.Resume.Length > 0)
            {

                var resumePath = Path.Combine("uploads", "Resumes");
                if (!Directory.Exists(resumePath))
                {
                    Directory.CreateDirectory(resumePath);
                }
                resumeFileName = $"{Guid.NewGuid()}_{applicationDto.Resume.FileName}";
                var filePath = Path.Combine(resumePath, resumeFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await applicationDto.Resume.CopyToAsync(stream);
                }
            }

            var application = new Application
            {
                CandidateId = candidate.Id,
                JobId = applicationDto.JobId,
                Experience = applicationDto.Experience,
                NoteForEmployer = applicationDto.NoteForEmployer,
                CoverLetter = coverLetterFileName,
                Resume = resumeFileName,
                StatusId = _context.Statuses.FirstOrDefault(s => s.Name == "Applied")?.Id ?? 4
            };
            _context.Applications.Add(application);
            await _context.SaveChangesAsync();

            return StatusCode(StatusCodes.Status201Created, "Job Applied successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("get/applications")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin")]
    public IActionResult GetApplications()
    {
        try
        {
            var applicationInfo = _context.Applications
                .Include(j => j.Candidate)
                .ThenInclude(c => c.User)
                .Include(j => j.Job)
                .ThenInclude(js => js.Employer)
                .Select(j => new ApplicationInfoDto
                {
                    Id = j.Id,
                    JobTitle = j.Job.Title,
                    CompanyName = j.Job.Employer.CompanyName,
                    jobLocation = j.Job.Location,
                    CandidateId = j.Candidate.Id,
                    CandidateName = j.Candidate.Name,
                    CandidateEmail = j.Candidate.User.Email,
                    Experience = j.Experience,
                    NoteForEmployer = j.NoteForEmployer,
                    ResumeName = j.Resume,
                    CoverLetterName = j.CoverLetter,
                    Status = _context.Statuses.FirstOrDefault(s => s.Id == j.StatusId).Name,
                    ApplicationDate = (DateTime)j.AppliedDate
                }).ToList();



            var email = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var candidate = _context.Candidates
                .Include(e => e.User)
                .FirstOrDefault(e => e.User.Email == email);

            if (candidate != null)
            {
                applicationInfo = applicationInfo.Where(j => j.CandidateId == candidate.Id).ToList();
            }

            return Ok(applicationInfo);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("get/applications-by-candidate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Candidate")]
    public IActionResult GetApplicationsByCandidate()
    {
        try
        {
            var email = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            var candidate = _context.Candidates
                .Include(e => e.User)
                .FirstOrDefault(e => e.User.Email == email);
            if (candidate == null)
            {
                return BadRequest("Candidate not found.");
            }

            var applicationInfo = _context.Applications
                .Include(j => j.Job)
                .ThenInclude(js => js.Employer)
                .Where(j => j.CandidateId == candidate.Id)
                .Select(j => new ApplicationInfoDto
                {
                    Id = j.Id,
                    JobTitle = j.Job.Title,
                    CompanyName = j.Job.Employer.CompanyName,
                    jobLocation = j.Job.Location,
                    Experience = j.Experience,
                    NoteForEmployer = j.NoteForEmployer,
                    ResumeName = j.Resume,
                    CoverLetterName = j.CoverLetter,
                    Status = _context.Statuses.FirstOrDefault(s => s.Id == j.StatusId).Name,
                    ApplicationDate = (DateTime)j.AppliedDate
                }).ToList();

            return Ok(applicationInfo);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("get/applications-by-job")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Employer")]
    public IActionResult GetApplicationsByJob(int jobId)
    {
        try
        {
            var email = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            var employer = _context.Employers
                .Include(e => e.User)
                .FirstOrDefault(e => e.User.Email == email);
            if (employer == null)
            {
                return BadRequest("Employer not found.");
            }

            var jobIdCheck = _context.Jobs.FirstOrDefault(j => j.Id == jobId);
            if (jobIdCheck == null)
            {
                return BadRequest("Job not found.");
            }

            if (_context.Jobs.Any(j => j.Id == jobId && j.EmployerId != employer.Id))
            {
                return BadRequest("Job is not created by the employer.");
            }

            var applicationInfo = _context.Applications
                .Include(j => j.Job)
                .ThenInclude(js => js.Employer)
                .Include(j => j.Candidate)
                .ThenInclude(c => c.User)
                .Where(j => j.JobId == jobId)
                .Select(j => new ApplicationInfoDto
                {
                    Id = j.Id,
                    CandidateId = j.Candidate.Id,
                    CandidateName = j.Candidate.Name,
                    CandidateEmail = j.Candidate.User.Email,
                    Experience = j.Experience,
                    NoteForEmployer = j.NoteForEmployer,
                    ResumeName = j.Resume,
                    CoverLetterName = j.CoverLetter,
                    Status = _context.Statuses.FirstOrDefault(s => s.Id == j.StatusId).Name,
                    ApplicationDate = (DateTime)j.AppliedDate
                }).ToList();

            return Ok(applicationInfo);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPut("application/change-status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Employer")]
    public async Task<IActionResult> UpdateStatus(int applicationId, int statusId)
    {
        try
        {
            var email = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            var employer = _context.Employers
                .Include(e => e.User)
                .FirstOrDefault(e => e.User.Email == email);
            if (employer == null)
            {
                return BadRequest("Employer not found.");
            }

            var applicationIdCheck = _context.Applications.FirstOrDefault(a => a.Id == applicationId);
            if (applicationIdCheck == null)
            {
                return BadRequest("application not found.");
            }

            if (_context.Applications.Include(a => a.Job).Any(a => a.Id == applicationId && a.Job.EmployerId != employer.Id))
            {
                return BadRequest("Application is not created for a job created by the employer.");
            }

            var application = _context.Applications.FirstOrDefault(a => a.Id == applicationId);
            application.StatusId = statusId;

            _context.Applications.Update(application);
            await _context.SaveChangesAsync();

            return Ok("Application status updated successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("get/statuses")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin, Employer")]
    public IActionResult GetStatuses(){
        try{
            var statuses = _context.Statuses
                .Select(s => new StatusDto
                {
                    Id = s.Id,
                    Name = s.Name
                }).ToList();

            return Ok(statuses);
        }catch(Exception ex){
            return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
        }
    }
}
