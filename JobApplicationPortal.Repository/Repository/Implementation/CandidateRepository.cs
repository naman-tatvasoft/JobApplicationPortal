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

    
    public async Task<Candidate> UpdateCandidate(Candidate candidate)
    {
        var existingCandidate = await _context.Candidates.FindAsync(candidate.Id);
        if (existingCandidate == null)
        {
            throw new KeyNotFoundException("Candidate not found");
        }
        existingCandidate.Name = candidate.Name;

        _context.Candidates.Update(existingCandidate);
        await _context.SaveChangesAsync();
        
        return existingCandidate;
    }
}
