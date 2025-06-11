using JobApplicationPortal.DataModels.Models;
using JobApplicationPortal.Repository.Repository.Interface;

namespace JobApplicationPortal.Repository.Repository.Implementation;

public class CandidateRepository : ICandidateRepository
{
    private readonly JobApplicationPortalContext _context;
    public CandidateRepository(JobApplicationPortalContext context)
    {
        _context = context;
    }
    
    public IQueryable<Candidate> GetAllCandidates()
    {
        return _context.Candidates.AsQueryable();
    }
    
    public Candidate GetCandidateByEmail(string email)
    {
        return _context.Candidates.FirstOrDefault(c => c.User.Email == email);
    }
}
