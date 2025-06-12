using JobApplicationPortal.DataModels.Dtos.RequestDtos;
using JobApplicationPortal.DataModels.Dtos.ResponseDtos;
using JobApplicationPortal.DataModels.Models;

namespace JobApplicationPortal.Service.Service.Interface;

public interface IApplicationService
{
    public Task<CommonDto<Application>> JobApplication(ApplicationDto applicationDto);
    public CommonDto<List<ApplicationInfoDto>> GetApplications();
    public CommonDto<List<ApplicationInfoDto>> GetApplicationsByCandidate();
    public CommonDto<List<ApplicationInfoDto>> GetApplicationsByJob(int jobId);
    public Task<CommonDto<object>> UpdateStatus(int applicationId, int statusId);
    public CommonDto<List<StatusDto>> GetStatuses();
}
