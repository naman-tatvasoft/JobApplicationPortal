using JobApplicationPortal.DataModels.Dtos.RequestDtos;
using JobApplicationPortal.DataModels.Dtos.ResponseDtos;

namespace JobApplicationPortal.Service.Service.Interface;

public interface IJobService
{
    public Task<CommonDto<object>> CreateJob(JobDto registerEmployerDto);
    public CommonDto<JobInfoDto> GetJobById(int jobId);
    public Task<CommonDto<JobInfoDto>> UpdateJob(int jobId, JobDto updateJobDto);
    public CommonDto<List<JobInfoDto>> GetJobs(string search, int pageNumber, int pageSize, string skill, string location, int experience, string category);
    public CommonDto<List<JobInfoDto>> GetCreatedJobs();
    public CommonDto<List<JobInfoDto>> GetJobsByEmployer(int employerId);
    public Task<CommonDto<object>> DeleteJob(int jobId);
    public CommonDto<List<SkillDto>> GetSkills();
    public CommonDto<List<CategoriesDto>> GetCategories();

    public Task<CommonDto<JobPreferenceInfoDto>> CreateJobPreference(JobPreferenceDto jobPreferenceDto);
    public Task<CommonDto<JobPreferenceInfoDto>> UpdateJobPreference(int jobPreferenceId, JobPreferenceDto jobPreferenceDto);
    public Task<CommonDto<object>> DeleteJobPreference(int jobPreferenceId);
    public Task<CommonDto<CategoriesDto>> CreateCategory(CategoriesDto categoriesDto);
    public Task<CommonDto<SkillDto>> CreateSkill(SkillDto skillDto);
    public string GetSkillNameById(int skillId);
    public Task<CommonDto<SkillDto>> UpdateSkill(int skillId, SkillDto skillDto);
    public string GetCategoryNameById(int categoryId);
    public Task<CommonDto<CategoriesDto>> UpdateCategory(int categoryId, CategoriesDto categoriesDto);
    public Task<CommonDto<object>> DeleteCategory(int categoryId);
    public Task<CommonDto<object>> DeleteSkill(int skillId);

}
