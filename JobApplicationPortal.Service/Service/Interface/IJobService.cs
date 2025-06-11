using JobApplicationPortal.DataModels.Dtos.RequestDtos;
using JobApplicationPortal.DataModels.Dtos.ResponseDtos;

namespace JobApplicationPortal.Service.Service.Interface;

public interface IJobService
{
     public CommonDto<List<JobDto>> GetJobsByEmployer(int employerId);
    public CommonDto<List<SkillDto>> GetSkills();
}
