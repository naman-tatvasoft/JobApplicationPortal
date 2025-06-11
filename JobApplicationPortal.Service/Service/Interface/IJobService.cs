using JobApplicationPortal.DataModels.Dtos.RequestDtos;
using JobApplicationPortal.DataModels.Dtos.ResponseDtos;

namespace JobApplicationPortal.Service.Service.Interface;

public interface IJobService
{
    public Task<CommonDto<object>> CreateJob(JobDto registerEmployerDto);
    public CommonDto<List<JobDto>> GetJobs();
    public CommonDto<List<JobDto>> GetCreatedJobs();
    public CommonDto<List<JobDto>> GetJobsByEmployer(int employerId);
    public Task<CommonDto<object>> DeleteJob(int jobId);
    public CommonDto<List<SkillDto>> GetSkills();
}
