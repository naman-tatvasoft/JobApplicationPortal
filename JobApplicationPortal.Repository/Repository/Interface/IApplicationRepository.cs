using JobApplicationPortal.DataModels.Dtos.ResponseDtos;
using JobApplicationPortal.DataModels.Models;

namespace JobApplicationPortal.Repository.Repository.Interface;

public interface IApplicationRepository
{

    public Task<bool> CheckAlreadyApplied(int jobId, int candidateId);
    public Task<Application> CreateApplication(Application application);
    public IQueryable<ApplicationInfoDto> GetApplications();
    public List<ApplicationInfoDto> GetApplicationsByCandidate(int candidateId);
    public List<ApplicationInfoDto> GetApplicationsByJob(int jobId);
    public Application GetApplicationById(int applicationId);
    public Task<Application> UpdateApplicationStatus(int applicationId, int statusId);
    public string GetCandidateEmailByApplicationId(int applicationId);
    public Task<bool> WithdrawApplication(int applicationId);
    public int GetTotalApplications();

}
