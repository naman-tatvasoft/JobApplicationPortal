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
        return await _context.Applications.AnyAsync(a => a.JobId == jobId && a.CandidateId == candidateId && a.StatusId != 5);
    }

    public async Task<Application> CreateApplication(Application application)
    {
        _context.Applications.Add(application);
        await _context.SaveChangesAsync();
        return application;
    }

    public IQueryable<Application> GetApplications()
    {
        var applicationInfo = _context.Applications
                .Include(j => j.Job)
                .ThenInclude(js => js.Employer)
                .Include(j => j.Candidate)
                .ThenInclude(c => c.User).AsQueryable();
        return applicationInfo;
    }

    public IQueryable<Application> GetApplicationsByCandidate(int candidateId)
    {
        var applicationInfo = _context.Applications
                .Include(j => j.Job)
                .ThenInclude(js => js.Employer)
                .Include(j => j.Candidate)
                .ThenInclude(c => c.User)
                .Where(j => j.CandidateId == candidateId).AsQueryable();
        return applicationInfo;
    }

    public List<ApplicationInfoDto> GetApplicationsByJob(int jobId)
    {
        var applicationInfo = _context.Applications
                .Include(j => j.Job)
                .ThenInclude(js => js.Employer)
                .Include(j => j.Candidate)
                .ThenInclude(c => c.User)
                .Where(j => j.JobId == jobId && j.StatusId != 5)
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
        return _context.Applications.Include(a => a.Job)
            .ThenInclude(aj => aj.Employer)
            .Include(a => a.Candidate)
            .ThenInclude(c => c.User)
            .Include(a => a.Status)
            .FirstOrDefault(a => a.Id == applicationId);
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

    public string GetCandidateEmailByApplicationId(int applicationId)
    {
        var application = _context.Applications
            .Include(a => a.Candidate)
            .ThenInclude(c => c.User)
            .FirstOrDefault(a => a.Id == applicationId);

        return application?.Candidate?.User?.Email;
    }

    public async Task<bool> WithdrawApplication(int applicationId){
        var application = await _context.Applications.FirstOrDefaultAsync(a => a.Id == applicationId);
        if (application != null)
        {
            var status = _context.Statuses.FirstOrDefault(s => s.Name == "Withdrawn");
            application.StatusId = status != null ? status.Id : 5;
           
            _context.Applications.Update(application);
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }

}
