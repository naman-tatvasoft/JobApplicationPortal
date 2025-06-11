using JobApplicationPortal.DataModels.Models;
using JobApplicationPortal.Repository.Repository.Interface;

namespace JobApplicationPortal.Repository.Repository.Implementation;

public class JobRepository : IJobRepository
{
    private readonly JobApplicationPortalContext _context;
    public JobRepository(JobApplicationPortalContext context)
    {
        _context = context;
    }

    public IQueryable<Job> GetJobsByEmployer(int employerId)
    {
        return _context.Jobs.Where(job => job.EmployerId == employerId).AsQueryable();
    }

}
