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
        return _context.Jobs.Where(job => (bool)!job.IsDeleted).AsQueryable();
    }

    public IQueryable<Job> GetJobsByEmployer(int employerId)
    {
        return _context.Jobs.Where(job => job.EmployerId == employerId).AsQueryable();
    }

    public Job GetJobById(int jobId)
    {
        return _context.Jobs.FirstOrDefault(job => job.Id == jobId && (bool)!job.IsDeleted);
    }

    public async Task<Job> UpdateJob(Job job)
    {
        var existingJob = GetJobById(job.Id);

        existingJob.Title = job.Title;
        existingJob.Description = job.Description;
        existingJob.Location = job.Location;
        existingJob.ExperienceRequired = job.ExperienceRequired;
        existingJob.OpenFrom = job.OpenFrom;

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
}
