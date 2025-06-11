using JobApplicationPortal.DataModels.Dtos.RequestDtos;
using JobApplicationPortal.DataModels.Dtos.ResponseDtos;
using JobApplicationPortal.Repository.Repository.Interface;
using JobApplicationPortal.Service.Service.Interface;

namespace JobApplicationPortal.Service.Service.Implementation;

public class JobService : IJobService
{
    private readonly IEmployerRepository _employerRepository;
    private readonly ISkillRepository _skillRepository;

    private readonly IJobRepository _jobRepository;

    public JobService(ISkillRepository skillRepository,
        IEmployerRepository employerRepository,
        IJobRepository jobRepository)
    {
        _skillRepository = skillRepository;
        _employerRepository = employerRepository;
        _jobRepository = jobRepository;
    }

    //   public async Task<CommonDto> CreateJob(RegisterEmployerDto registerEmployerDto){
    //   }

    //   public CommonDto GetJobs(){
    //   }

    //   public CommonDto GetCreatedJobs(){
    //   }

    public CommonDto<List<JobDto>> GetJobsByEmployer(int employerId)
    {
        if (!_employerRepository.IsEmployerIdExist(employerId))
        {
            return new CommonDto<List<JobDto>>
            {
                StatusCode = 400,
                Message = "Employer not found."
            };
        }

        var jobs = _jobRepository.GetJobsByEmployer(employerId);

        if (!jobs.Any())
        {
            return new CommonDto<List<JobDto>>
            {
                StatusCode = 400,
                Message = "No jobs found for this employer."
            };
        }

        var jobDtos = jobs.Select(job => new JobDto
        {
            Title = job.Title,
            Description = job.Description,
            Location = job.Location,
            ExperienceRequired = job.ExperienceRequired,
            OpenFrom = job.OpenFrom,
            skillsRequiredList = job.JobSkills.Select(skill => new SkillDto
            {
                Id = skill.Id,
                Name = skill.Skill.Name
            }).ToList()
        }).ToList();

        return new CommonDto<List<JobDto>>
        {
            Data = jobDtos,
            StatusCode = 200,
            Message = "Jobs retrieved successfully."
        };
    }

    //   public async Task<CommonDto> DeleteJob(int jobId){
    //   }

    public CommonDto<List<SkillDto>> GetSkills()
    {
        var skills = _skillRepository.GetSkills()
            .Select(skill => new SkillDto
            {
                Id = skill.Id,
                Name = skill.Name
            })
            .ToList();

        return new CommonDto<List<SkillDto>>
        {
            Data = skills,
            StatusCode = 200,
            Message = "Skills retrieved successfully."
        };
    }

}
