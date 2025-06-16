using JobApplicationPortal.DataModels.Models;

namespace JobApplicationPortal.Repository.Repository.Interface;

public interface ICandidateRepository
{
    public IQueryable<Candidate> GetAllCandidates();
    public Candidate GetCandidateByEmail(string email);
    public Task<Candidate> UpdateCandidate(Candidate candidate);

}
