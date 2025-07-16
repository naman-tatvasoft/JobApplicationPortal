using JobApplicationPortal.DataModels.Dtos.ResponseDtos;
using JobApplicationPortal.Repository.Repository.Implementation;
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
        var totalJobs = _jobRepository.GetJobs().Count();
        var totalApplications = _applicationRepository.GetApplications().Count(a => a.Status.Name != "Withdrawn");

        var totalUsers = _userRepository.GetTotalUsers();
        var totalEmployers = totalUsers.Count(u => u == "Employer");
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
        ).OrderByDescending(u => u.OpenFrom).ThenBy(u => u.Id).Take(3).ToList();

        var latestApplications = _applicationRepository.GetApplications().Select(j => new ApplicationInfoDto
        {
            Id = j.Id,
            Experience = j.Experience,
            NoteForEmployer = j.NoteForEmployer,
            ResumeName = j.Resume,
            CoverLetterName = j.CoverLetter,
            ApplicationDate = (DateTime)j.AppliedDate,

            JobTitle = j.Job.Title,
            CompanyName = j.Job.Employer.CompanyName,
            jobLocation = j.Job.Location,

            CandidateId = j.Candidate.Id,
            CandidateName = j.Candidate.Name,
            CandidateEmail = j.Candidate.User.Email,

            Status = j.Status.Name,
        }).OrderByDescending(u => u.ApplicationDate).ThenBy(u => u.Id).Take(3).ToList();

        var LatestUsers = _userRepository.GetUsers().OrderByDescending(u => u.Id).Take(3).Select(user => new UserInfoDto
        {
            id = user.Id,
            email = user.Email,
            name = user.Role.Name == "Employer" ? user.Employer.Name : user.Candidate.Name,
            roleName = user.Role.Name
        }).ToList();

        var adminData = new AdminDataDto
        {
            TotalEmployers = totalEmployers,
            TotalJobs = totalJobs,
            TotalCandidates = totalCandidates,
            TotalApplications = totalApplications,
            LatestJobs = latestJobs,
            LatestApplications = latestApplications,
            LatestUsers = LatestUsers
        };

        return new CommonDto<AdminDataDto>
        {
            StatusCode = 200,
            Message = "Admin data retrieved successfully.",
            Data = adminData
        };
    }


    public CommonDto<EmployerDataDto> EmployerData(int id)
    {
        var totalJobs = _jobRepository.GetJobs().Count(j => j.EmployerId == id);

        var application = _applicationRepository.GetApplications();
        var totalApplications = application.Count(a => a.Status.Name != "Withdrawn" && a.Job.Employer.Id == id);
        var newApplications = application.Count(a => a.Status.Name != "Applied" && a.Job.Employer.Id == id);

        var latestJobs = _jobRepository.GetJobs().Where(j => j.EmployerId == id).Select(
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
        ).OrderByDescending(u => u.OpenFrom).ThenBy(u => u.Id).Take(3).ToList();

        var latestApplications = _applicationRepository.GetApplications().Where(a => a.Job.Employer.Id == id).Select(j => new ApplicationInfoDto
        {
            Id = j.Id,
            Experience = j.Experience,
            NoteForEmployer = j.NoteForEmployer,
            ResumeName = j.Resume,
            CoverLetterName = j.CoverLetter,
            ApplicationDate = (DateTime)j.AppliedDate,

            JobTitle = j.Job.Title,
            CompanyName = j.Job.Employer.CompanyName,
            jobLocation = j.Job.Location,

            CandidateId = j.Candidate.Id,
            CandidateName = j.Candidate.Name,
            CandidateEmail = j.Candidate.User.Email,

            Status = j.Status.Name,
        }).OrderByDescending(u => u.ApplicationDate).ThenBy(u => u.Id).Take(3).ToList();

        var employerData = new EmployerDataDto
        {
            TotalJobs = totalJobs,
            TotalApplications = totalApplications,
            NewApplications = newApplications,
            LatestJobs = latestJobs,
            LatestApplications = latestApplications,
        };

        return new CommonDto<EmployerDataDto>
        {
            StatusCode = 200,
            Message = "Employer data retrieved successfully.",
            Data = employerData
        };
    }

}
