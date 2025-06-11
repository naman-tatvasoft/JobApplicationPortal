using JobApplicationPortal.DataModels.Models;

namespace JobApplicationPortal.Repository.Repository.Interface;

public interface IJobRepository
{
    public IQueryable<Job> GetJobsByEmployer(int employerId);
}
