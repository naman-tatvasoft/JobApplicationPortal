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

    public bool IsEmployerIdExist(int employerId)
    {
        return _context.Employers.Any(e => e.Id == employerId);
    }

    public Employer GetEmployerByEmail(string email)
    {
        return _context.Employers.FirstOrDefault(e => e.User.Email == email);
    }
}
