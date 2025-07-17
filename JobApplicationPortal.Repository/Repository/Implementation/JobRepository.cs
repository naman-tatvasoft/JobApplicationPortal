using JobApplicationPortal.DataModels.Models;
using JobApplicationPortal.Repository.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationPortal.Repository.Repository.Implementation;

public class JobRepository : IJobRepository
{
    private readonly JobApplicationPortalContext _context;
    public JobRepository(JobApplicationPortalContext context)
    {
        _context = context;
    }

    public IQueryable<Job> GetJobs()
    {
        return _context.Jobs.Where(job => !job.IsDeleted && (bool)job.IsActive).AsQueryable();
    }

    public IQueryable<Job> GetJobsByEmployer(int employerId)
    {
        return _context.Jobs.Where(job => job.EmployerId == employerId && !job.IsDeleted).AsQueryable();
    }

    public Job GetJobById(int jobId)
    {
        return _context.Jobs.Include(job => job.JobSkills).ThenInclude(js => js.Skill).Include(job => job.Category).FirstOrDefault(job => job.Id == jobId  && (bool)!job.IsDeleted);
    }

    public async Task<Job> UpdateJob(Job job)
    {
        var existingJob = GetJobById(job.Id);

        existingJob.Title = job.Title;
        existingJob.Description = job.Description;
        existingJob.Location = job.Location;
        existingJob.ExperienceRequired = job.ExperienceRequired;
        existingJob.OpenFrom = job.OpenFrom;
        existingJob.Vacancy = job.Vacancy;
        existingJob.CategoryId = job.CategoryId;

        _context.Jobs.Update(existingJob);
        await _context.SaveChangesAsync();

        return existingJob;
    }

    public bool IsJobByEmployer(int jobId, Employer employer)
    {
        return _context.Jobs.Any(j => j.Id == jobId && j.EmployerId == employer.Id);
    }

    public bool IsJobAlreadyDeleted(int jobId)
    {
        return _context.Jobs.Any(job => job.Id == jobId && (bool)job.IsDeleted);
    }

    public async Task DeleteJob(int jobId)
    {
        var job = GetJobById(jobId);

        job.IsDeleted = true;

        _context.Jobs.Update(job);
        await _context.SaveChangesAsync();
    }

    public bool JobTitleByEmployerAlreadyExists(string title, int employerId)
    {
        return _context.Jobs.Any(j => j.Title == title && j.EmployerId == employerId && (bool)!j.IsDeleted);
    }

    public async Task<Job> CreateJob(Job job)
    {
        job.CreatedAt = DateTime.Now;
        job.IsActive = true;
        job.IsDeleted = false;

        _context.Jobs.Add(job);
        await _context.SaveChangesAsync();

        return job;
    }

    public bool CheckJobById(int jobId)
    {
        return _context.Jobs.Any(job => job.Id == jobId && (bool)!job.IsDeleted && (bool)job.IsActive);
    }

    public bool CheckExperience(int jobId, int experience)
    {
        var job = _context.Jobs.Any(j => j.Id == jobId && (bool)!j.IsDeleted && (bool)j.IsActive && j.ExperienceRequired <= experience);
        return job;
    }

    public string GetEmployerEmailByJobId(int jobId)
    {
        var job = _context.Jobs.Include(j => j.Employer).ThenInclude(e => e.User).FirstOrDefault(j => j.Id == jobId && (bool)!j.IsDeleted);
        return job?.Employer?.User?.Email ?? string.Empty;
    }

    public async Task ReduceVacancy(int jobId)
    {
        var job = GetJobById(jobId);
        if (job != null && job.Vacancy > 0)
        {
            job.Vacancy -= 1;
            if (job.Vacancy == 0)
            {
                job.IsActive = false;
            }
            
            _context.Jobs.Update(job);
            await _context.SaveChangesAsync();
        }
    }

    
}
