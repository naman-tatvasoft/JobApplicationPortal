using JobApplicationPortal.DataModels.Dtos.RequestDtos;
using JobApplicationPortal.DataModels.Dtos.ResponseDtos;
using JobApplicationPortal.DataModels.Models;

namespace JobApplicationPortal.Service.Service.Interface;

public interface IApplicationService
{
    public Task<CommonDto<Application>> JobApplication(ApplicationDto applicationDto);
    public CommonDto<List<ApplicationInfoDto>> GetApplications(string search, int pageNumber, int pageSize, string status);
    public CommonDto<List<ApplicationInfoDto>> GetApplicationsByCandidate(string search, int pageNumber, int pageSize, string status);
    public CommonDto<List<ApplicationInfoDto>> GetApplicationsByJob(int jobId);
    public Task<CommonDto<object>> UpdateStatus(int applicationId, int statusId);
    public CommonDto<List<StatusDto>> GetStatuses();
    public CommonDto<int> GetTotalApplicationByJob(int jobId);
    public Task<CommonDto<object>> WithdrawApplication(int applicationId);
    public CommonDto<ApplicationInfoDto> GetApplicationById(int applicationId);
    public Task<CommonDto<StatusDto>> CreateStatus(StatusDto statusDto);
    public string GetStatusNameById(int statusId);
    public Task<CommonDto<StatusDto>> UpdateStatus(int statusId, StatusDto statusDto);
    public Task<CommonDto<object>> DeleteStatus(int statusId);

}
