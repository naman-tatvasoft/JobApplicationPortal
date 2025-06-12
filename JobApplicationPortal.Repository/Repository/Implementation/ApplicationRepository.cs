using JobApplicationPortal.DataModels.Dtos.ResponseDtos;
using JobApplicationPortal.DataModels.Models;
using JobApplicationPortal.Repository.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;

namespace JobApplicationPortal.Repository.Repository.Implementation;

public class ApplicationRepository : IApplicationRepository
{
    private readonly JobApplicationPortalContext _context;
    public ApplicationRepository(JobApplicationPortalContext context)
    {
        _context = context;
    }

    public async Task<bool> CheckAlreadyApplied(int jobId, int candidateId)
    {
        return await _context.Applications.AnyAsync(a => a.JobId == jobId && a.CandidateId == candidateId);
    }

    public async Task<Application> CreateApplication(Application application)
    {
        _context.Applications.Add(application);
        await _context.SaveChangesAsync();
        return application;
    }

    public List<ApplicationInfoDto> GetApplications()
    {
        var applicationInfo = _context.Applications
                .Include(j => j.Job)
                .ThenInclude(js => js.Employer)
                .Include(j => j.Candidate)
                .ThenInclude(c => c.User)
                .Select(j => new ApplicationInfoDto
                {
                    Id = j.Id,
                    Experience = j.Experience,
                    NoteForEmployer = j.NoteForEmployer,
                    ResumeName = j.Resume,
                    CoverLetterName = j.CoverLetter,
                    ApplicationDate = (DateTime)j.AppliedDate,

                    JobTitle = j.Job.Title,
                    CompanyName = j.Job.Employer.CompanyName,
                    jobLocation = j.Job.Location,

                    CandidateId = j.Candidate.Id,
                    CandidateName = j.Candidate.Name,
                    CandidateEmail = j.Candidate.User.Email,

                    Status = _context.Statuses.FirstOrDefault(s => s.Id == j.StatusId).Name,
                }).ToList();
        return applicationInfo;
    }



    public List<ApplicationInfoDto> GetApplicationsByCandidate(int candidateId)
    {
        var applicationInfo = _context.Applications
                .Include(j => j.Job)
                .ThenInclude(js => js.Employer)
                .Include(j => j.Candidate)
                .ThenInclude(c => c.User)
                .Where(j => j.CandidateId == candidateId)
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
                    ApplicationDate = (DateTime)j.AppliedDate,
                    Status = _context.Statuses.FirstOrDefault(s => s.Id == j.StatusId).Name,
                }).ToList();
        return applicationInfo;
    }

    public List<ApplicationInfoDto> GetApplicationsByJob(int jobId)
    {
        var applicationInfo = _context.Applications
                .Include(j => j.Job)
                .ThenInclude(js => js.Employer)
                .Include(j => j.Candidate)
                .ThenInclude(c => c.User)
                .Where(j => j.JobId == jobId)
                .Select(j => new ApplicationInfoDto
                {
                    Id = j.Id,
                    JobTitle = j.Job.Title,
                    Experience = j.Experience,
                    NoteForEmployer = j.NoteForEmployer,
                    ResumeName = j.Resume,
                    CoverLetterName = j.CoverLetter,
                    ApplicationDate = (DateTime)j.AppliedDate,

                    CandidateId = j.Candidate.Id,
                    CandidateName = j.Candidate.Name,
                    CandidateEmail = j.Candidate.User.Email,

                    Status = _context.Statuses.FirstOrDefault(s => s.Id == j.StatusId).Name,
                }).ToList();
        return applicationInfo;
    }

    public Application GetApplicationById(int applicationId)
    {
        return _context.Applications.FirstOrDefault(a => a.Id == applicationId);
    }

    public bool JobByEmployer(int jobId, int employerId)
    {
        return _context.Jobs.Any(j => j.Id == jobId && j.EmployerId == employerId);
    }

    public async Task<Application> UpdateApplicationStatus(int applicationId, int statusId)
    {
        var application = await _context.Applications.FirstOrDefaultAsync(a => a.Id == applicationId);
        if (application != null)
        {
            application.StatusId = statusId;
            _context.Applications.Update(application);
            await _context.SaveChangesAsync();
            return application;
        }
        return null;
    }
}
