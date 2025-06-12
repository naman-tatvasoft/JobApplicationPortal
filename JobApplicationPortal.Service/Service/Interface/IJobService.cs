using JobApplicationPortal.DataModels.Dtos.RequestDtos;
using JobApplicationPortal.DataModels.Dtos.ResponseDtos;

namespace JobApplicationPortal.Service.Service.Interface;

public interface IJobService
{
    public Task<CommonDto<object>> CreateJob(JobDto registerEmployerDto);
    public CommonDto<JobDto> GetJobById(int jobId);
    public Task<CommonDto<JobDto>> UpdateJob(JobDto updateJobDto);
    public CommonDto<List<JobDto>> GetJobs(string search, int pageNumber, int pageSize, string skill, string location, int experience);
    public CommonDto<List<JobDto>> GetCreatedJobs();
    public CommonDto<List<JobDto>> GetJobsByEmployer(int employerId);
    public Task<CommonDto<object>> DeleteJob(int jobId);
    public CommonDto<List<SkillDto>> GetSkills();
}
