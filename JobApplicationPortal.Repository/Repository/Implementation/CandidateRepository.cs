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
    
    public async Task AddCandidateAsync(Candidate candidate)
    {
        _context.Candidates.Add(candidate);
        await _context.SaveChangesAsync();
    }
}
