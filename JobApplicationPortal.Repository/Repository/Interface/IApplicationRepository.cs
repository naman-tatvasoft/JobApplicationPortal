using JobApplicationPortal.DataModels.Dtos.ResponseDtos;
using JobApplicationPortal.DataModels.Models;

namespace JobApplicationPortal.Repository.Repository.Interface;

public interface IApplicationRepository
{
    public List<ApplicationInfoDto> GetApplicationsByJob(int jobId);
    public Application GetApplicationById(int applicationId);
    public bool JobByEmployer(int jobId, int employerId);
    public Task<Application> UpdateApplicationStatus(int applicationId, int statusId);
}
