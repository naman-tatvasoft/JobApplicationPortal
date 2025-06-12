using JobApplicationPortal.DataModels.Dtos.ResponseDtos;

namespace JobApplicationPortal.Service.Service.Interface;

public interface IApplicationService
{
    public CommonDto<List<ApplicationInfoDto>> GetApplicationsByJob(int jobId);
    public Task<CommonDto<object>> UpdateStatus(int applicationId, int statusId);
    public CommonDto<List<StatusDto>> GetStatuses();
}
