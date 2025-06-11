using JobApplicationPortal.DataModels.Models;
using JobApplicationPortal.Repository.Repository.Interface;

namespace JobApplicationPortal.Repository.Repository.Implementation;

public class EmployerRepository : IEmployerRepository
{
    private readonly JobApplicationPortalContext _context;
    public EmployerRepository(JobApplicationPortalContext context)
    {
        _context = context;
    }
    
    public IQueryable<Employer> GetAllEmployers()
    {
        return _context.Employers.AsQueryable();
    }
}
