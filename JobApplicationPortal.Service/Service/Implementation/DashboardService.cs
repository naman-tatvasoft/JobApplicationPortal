using JobApplicationPortal.DataModels.Dtos.ResponseDtos;
using JobApplicationPortal.Repository.Repository.Interface;
using JobApplicationPortal.Service.Service.Interface;

namespace JobApplicationPortal.Service.Service.Implementation;

public class DashboardService : IDashboardService
{
    private readonly IUserRepository _userRepository;
    private readonly IJobRepository _jobRepository;
    private readonly IApplicationRepository _applicationRepository;

    public DashboardService(IUserRepository userRepository,
        IApplicationRepository applicationRepository,
        IJobRepository jobRepository)
    {
        _userRepository = userRepository;
        _applicationRepository = applicationRepository;
        _jobRepository = jobRepository;
    }

    public CommonDto<AdminDataDto> AdminData()
    {
        var totalJobs =  _jobRepository.GetTotalJobs();
        var totalApplications = _applicationRepository.GetTotalApplications();

        var totalUsers = _userRepository.GetTotalUsers();
        var totalEmployers =  totalUsers.Count(u => u == "Employer");
        var totalCandidates = totalUsers.Count(u => u == "Candidate");

        var latestJobs = _jobRepository.GetJobs().Select(
            job => new JobInfoDto
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                Location = job.Location,
                ExperienceRequired = job.ExperienceRequired,
                OpenFrom = job.OpenFrom,
                Vacancies = job.Vacancy,
                CategoryName = job.Category.Name,
                 skillsRequiredList = job.JobSkills.Select(skill => new SkillDto
                {
                    Id = skill.Id,
                    Name = skill.Skill.Name
                }).ToList()
            }
        ).OrderByDescending(u => u.OpenFrom).ThenBy(u=>u.Id).Take(3).ToList();

        var latestApplications = _applicationRepository.GetApplications().OrderByDescending(u => u.ApplicationDate).ThenBy(u=>u.Id).Take(3).ToList();

        var adminData = new AdminDataDto
        {
            TotalEmployers = totalEmployers,
            TotalJobs = totalJobs,
            TotalCandidates = totalCandidates,
            TotalApplications = totalApplications,
            LatestJobs = latestJobs,
            LatestApplications = latestApplications
        };

        return new CommonDto<AdminDataDto>
        {
            StatusCode = 200,
            Message = "Admin data retrieved successfully.",
            Data = adminData
        };
    }
}
