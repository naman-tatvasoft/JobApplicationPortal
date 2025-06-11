using System.Security.Claims;
using JobApplicationPortal.DataModels.Dtos.RequestDtos;
using JobApplicationPortal.DataModels.Dtos.ResponseDtos;
using JobApplicationPortal.DataModels.Models;
using JobApplicationPortal.Repository.Repository.Interface;
using JobApplicationPortal.Service.Service.Interface;
using Microsoft.AspNetCore.Http;

namespace JobApplicationPortal.Service.Service.Implementation;

public class JobService : IJobService
{
    private readonly IEmployerRepository _employerRepository;
    private readonly ICandidateRepository _candidateRepository;
    private readonly ISkillRepository _skillRepository;
    private readonly IJobRepository _jobRepository;
    private readonly IJobSkillRepository _jobSkillRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public JobService(ISkillRepository skillRepository,
        ICandidateRepository candidateRepository,
        IEmployerRepository employerRepository,
        IJobRepository jobRepository,
        IJobSkillRepository jobSkillRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _skillRepository = skillRepository;
        _candidateRepository = candidateRepository;
        _employerRepository = employerRepository;
        _jobRepository = jobRepository;
        _jobSkillRepository = jobSkillRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<CommonDto<object>> CreateJob(JobDto createJobDto)
    {
        if (createJobDto == null || !createJobDto.skillsRequiredList.Any())
        {
            return new CommonDto<object>
            {
                StatusCode = 400,
                Message = "Invalid job data."
            };
        }

        var email = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email))
        {
            return new CommonDto<object>
            {
                StatusCode = 401,
                Message = "User is not authenticated."
            };
        }

        var employer = _employerRepository.GetEmployerByEmail(email);


        var jobExists = _jobRepository.JobTitleByEmployerAlreadyExists(createJobDto.Title, employer.Id);
        if (jobExists)
        {
            return new CommonDto<object>
            {
                StatusCode = 400,
                Message = "Job with the same title already exists for this employer."
            };
        }


        var job = new Job
        {
            Title = createJobDto.Title,
            Description = createJobDto.Description,
            Location = createJobDto.Location,
            ExperienceRequired = createJobDto.ExperienceRequired,
            EmployerId = employer.Id,
            OpenFrom = createJobDto.OpenFrom
        };

        var jobSaved = await _jobRepository.CreateJob(job);

        if (createJobDto.skillsRequiredList != null && createJobDto.skillsRequiredList.Any())
        {
            foreach (var skill in createJobDto.skillsRequiredList)
            {
                var existingSkill = _skillRepository.GetSkillByName(skill.Name);
                if (existingSkill != null)
                {
                    var jobSkill = new JobSkill
                    {
                        JobId = jobSaved.Id,
                        SkillId = existingSkill.Id,
                    };
                    await _jobSkillRepository.CreateJobSkill(jobSkill);
                }
                else
                {
                    return new CommonDto<object>
                    {
                        StatusCode = 400,
                        Message = "Such skill is not present"
                    };
                }
            }
        }

        return new CommonDto<object>
        {
            StatusCode = 201,
            Message = "Job created successfully."
        };
    }

    public CommonDto<List<JobDto>> GetJobs()
    {
        var jobs = _jobRepository.GetJobs()
            .Select(job => new JobDto
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

        var email = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email))
        {
            return new CommonDto<List<JobDto>>
            {
                StatusCode = 401,
                Message = "User is not authenticated."
            };
        }

        var candidate = _candidateRepository.GetCandidateByEmail(email);

        if (candidate != null)
        {
            jobs = jobs.Where(j => j.OpenFrom <= DateOnly.FromDateTime(DateTime.Now)).ToList();
        }

        return new CommonDto<List<JobDto>>
        {
            Data = jobs,
            StatusCode = 200,
            Message = "Jobs retrieved successfully."
        };
    }

    public CommonDto<List<JobDto>> GetCreatedJobs()
    {
        var email = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email))
        {
            return new CommonDto<List<JobDto>>
            {
                StatusCode = 401,
                Message = "User is not authenticated."
            };
        }

        var employer = _employerRepository.GetEmployerByEmail(email);

        if (employer == null)
        {
            return new CommonDto<List<JobDto>>
            {
                StatusCode = 400,
                Message = "Employer not found."
            };
        }

        var jobs = _jobRepository.GetJobsByEmployer(employer.Id);

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

    public async Task<CommonDto<object>> DeleteJob(int jobId)
    {
        var email = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email))
        {
            return new CommonDto<object>
            {
                StatusCode = 401,
                Message = "User is not authenticated."
            };
        }

        var employer = _employerRepository.GetEmployerByEmail(email);

        if (employer == null)
        {
            return new CommonDto<object>
            {
                StatusCode = 400,
                Message = "Employer not found."
            };
        }

        var jobIdCheck = _jobRepository.GetJobById(jobId);
        if (jobIdCheck == null)
        {
            return new CommonDto<object>
            {
                StatusCode = 400,
                Message = "Job not found."
            };
        }

        var isJobByEmployer = _jobRepository.IsJobByEmployer(jobId, employer);
        if (!isJobByEmployer)
        {
            return new CommonDto<object>
            {
                StatusCode = 400,
                Message = "Job is not created by the employer."
            };
        }

        var IsAlreadyDeleted = _jobRepository.IsJobAlreadyDeleted(jobId);
        if (IsAlreadyDeleted)
        {
            return new CommonDto<object>
            {
                StatusCode = 400,
                Message = "Job is already deleted."
            };
        }

        await _jobRepository.DeleteJob(jobId);

        return new CommonDto<object>
        {
            StatusCode = 200,
            Message = "Job Deleted successfully."
        };

    }

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
            Message = "Skills retieved successfully."
        };
    }

}
