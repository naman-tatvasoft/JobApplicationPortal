using JobApplicationPortal.DataModels.Models;

namespace JobApplicationPortal.Repository.Repository.Interface;

public interface ICandidateRepository
{
    public Task AddCandidateAsync(Candidate candidate);
    public IQueryable<Candidate> GetAllCandidates();
}
