using JobApplicationPortal.DataModels.Models;

namespace JobApplicationPortal.Repository.Repository.Interface;

public interface IJobRepository
{

    public IQueryable<Job> GetJobs();
    public IQueryable<Job> GetJobsByEmployer(int employerId);
    public Job GetJobById(int jobId);
    public Task<Job> UpdateJob(Job job);
    public bool IsJobByEmployer(int jobId, Employer employer);
    public bool IsJobAlreadyDeleted(int jobId);
    public Task DeleteJob(int jobId);
    public bool JobTitleByEmployerAlreadyExists(string title, int employerId);
    public Task<Job> CreateJob(Job job);
    public bool CheckJobById(int jobId);
}
