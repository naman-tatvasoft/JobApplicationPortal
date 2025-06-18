using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using JobApplicationPortal.DataModels.Dtos.RequestDtos;
using JobApplicationPortal.DataModels.Dtos.ResponseDtos;
using JobApplicationPortal.DataModels.Models;
using JobApplicationPortal.Repository.Repository.Interface;
using JobApplicationPortal.Service.Exceptions;
using JobApplicationPortal.Service.Helper;
using JobApplicationPortal.Service.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic;

namespace JobApplicationPortal.Service.Service.Implementation;

public class JobService : IJobService
{
    private readonly IEmployerRepository _employerRepository;
    private readonly ICandidateRepository _candidateRepository;
    private readonly ISkillRepository _skillRepository;
    private readonly IJobRepository _jobRepository;
    private readonly IJobSkillRepository _jobSkillRepository;
    private readonly ICategoryRepository _categoriesRepository;
    private readonly IJobPreferenceRepository _jobPreferenceRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public JobService(ISkillRepository skillRepository,
        ICandidateRepository candidateRepository,
        IEmployerRepository employerRepository,
        IJobRepository jobRepository,
        IJobSkillRepository jobSkillRepository,
        ICategoryRepository categoriesRepository,
        IJobPreferenceRepository jobPreferenceRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _skillRepository = skillRepository;
        _candidateRepository = candidateRepository;
        _employerRepository = employerRepository;
        _jobRepository = jobRepository;
        _jobSkillRepository = jobSkillRepository;
        _categoriesRepository = categoriesRepository;
        _jobPreferenceRepository = jobPreferenceRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<CommonDto<object>> CreateJob(JobDto createJobDto)
    {
        var email = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email))
        {
            throw new UnAuthenticatedException();
        }

        var employer = _employerRepository.GetEmployerByEmail(email);
        if (employer == null)
        {
            throw new EmployerNotFoundException();
        }

        var jobExists = _jobRepository.JobTitleByEmployerAlreadyExists(createJobDto.Title, employer.Id);
        if (jobExists)
        {
            throw new JobNameAlreadyExistException();
        }

        var job = new Job
        {
            Title = createJobDto.Title,
            Description = createJobDto.Description,
            Location = createJobDto.Location,
            ExperienceRequired = createJobDto.ExperienceRequired,
            EmployerId = employer.Id,
            OpenFrom = createJobDto.OpenFrom,
            Vacancy = createJobDto.Vacancies,
            CategoryId = createJobDto.CategoryId,
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
                    throw new SkillNotPresentException();
                }
            }
        }

        var matchingCandidates = _jobPreferenceRepository.GetCandidatesMatchingPreference(job);
        Task.Run(async () =>
        {
            foreach (var candidate in matchingCandidates)
            {
                var sub = "Job Posting Notification";
                var body = $@"
            <div style='max-width: 500px; font-family: Arial, sans-serif; border: 1px solid #ddd;'>
            <div style='background: #006CAC; padding: 10px; text-align: center; height:90px; max-width:100%; display: flex; justify-content: center; align-items: center;'>
                <span style='color: #fff; font-size: 24px; margin-left: 10px; font-weight: 600;'>Job Portal</span>
            </div>
            <div style='padding: 20px 5px; background-color: #e8e8e8;'>
                <p>Job Portal</p>
                <p>Respected {candidate.Name},</p>
                <p>A new job matching your preferences has been posted: {job.Title}.</p>
                <p>If you encounter any issues or have any questions, please do not hesitate to contact our support team.</p>
                <p><strong style='color: orange;'>Important Note:</strong>
                    If you did not choose any job preference for same, please ignore this email or contact our support team.
                </p>
            </div>
            </div>";

                await EmailHelper.SendEmailAsync(
                    receiverEmailAddress: candidate.User.Email,
                    receiverDisplayName: candidate.Name,
                    subject: sub,
                    body: body
                );
                
                await Task.Delay(1000);

            }
        });

        return new CommonDto<object>
        {
            StatusCode = 201,
            Message = "Job created successfully."
        };
    }

    public CommonDto<JobDto> GetJobById(int jobId)
    {
        var email = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email))
        {
            throw new UnAuthenticatedException();
        }

        var employer = _employerRepository.GetEmployerByEmail(email);
        if (employer == null)
        {
            throw new EmployerNotFoundException();
        }

        var jobIdCheck = _jobRepository.GetJobById(jobId);
        if (jobIdCheck == null)
        {
            throw new JobNotFoundException();
        }

        var isJobByEmployer = _jobRepository.IsJobByEmployer(jobId, employer);
        if (!isJobByEmployer)
        {
            throw new JobNotByEmployerException();
        }

        var job = _jobRepository.GetJobById(jobId);

        var jobDto = new JobDto
        {
            Id = job.Id,
            Title = job.Title,
            Description = job.Description,
            Location = job.Location,
            ExperienceRequired = job.ExperienceRequired,
            OpenFrom = job.OpenFrom,
            Vacancies = job.Vacancy,
            CategoryId = job.CategoryId,
            CategoryName = job.Category.Name,
            skillsRequiredList = job.JobSkills.Where(skill => skill.Skill != null).Select(skill => new SkillDto
            {
                Id = skill.Id,
                Name = skill.Skill.Name
            }).ToList()
        };

        return new CommonDto<JobDto>
        {
            Data = jobDto,
            StatusCode = 200,
            Message = "Job retrieved successfully."
        };
    }

    public async Task<CommonDto<JobDto>> UpdateJob(JobDto updateJobDto)
    {
        var email = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email))
        {
            throw new UnAuthenticatedException();
        }

        var employer = _employerRepository.GetEmployerByEmail(email);

        if (employer == null)
        {
            throw new EmployerNotFoundException();
        }

        var jobIdCheck = _jobRepository.GetJobById(updateJobDto.Id);
        if (jobIdCheck == null)
        {
            throw new JobNotFoundException();
        }

        var isJobByEmployer = _jobRepository.IsJobByEmployer(updateJobDto.Id, employer);
        if (!isJobByEmployer)
        {
            throw new JobNotByEmployerException();
        }

        var IsJobDateAlreadyActive = _jobRepository.GetJobById(updateJobDto.Id).OpenFrom;
        if (IsJobDateAlreadyActive <= DateOnly.FromDateTime(DateTime.Now))
        {
            throw new JobAlreadyOpenedException();
        }

        var jobToUpdate = new Job
        {
            Id = updateJobDto.Id,
            Title = updateJobDto.Title,
            Description = updateJobDto.Description,
            Location = updateJobDto.Location,
            ExperienceRequired = updateJobDto.ExperienceRequired,
            EmployerId = employer.Id,
            IsActive = updateJobDto.IsActive,
            OpenFrom = updateJobDto.OpenFrom,
            Vacancy = updateJobDto.Vacancies,
            CategoryId = updateJobDto.CategoryId,
        };

        await _jobRepository.UpdateJob(jobToUpdate);

        await _jobSkillRepository.DeleteJobSkillByJobId(updateJobDto.Id);
        foreach (var skill in updateJobDto.skillsRequiredList)
        {
            var existingSkill = _skillRepository.GetSkillByName(skill.Name);
            if (existingSkill != null)
            {
                var jobSkill = new JobSkill
                {
                    JobId = jobToUpdate.Id,
                    SkillId = existingSkill.Id,
                };
                await _jobSkillRepository.CreateJobSkill(jobSkill);
            }
            else
            {
                throw new SkillNotPresentException();
            }
        }

        var updatedJob = _jobRepository.GetJobById(updateJobDto.Id);
        var jobDto = new JobDto
        {
            Id = updatedJob.Id,
            Title = updatedJob.Title,
            Description = updatedJob.Description,
            Location = updatedJob.Location,
            ExperienceRequired = updatedJob.ExperienceRequired,
            OpenFrom = updatedJob.OpenFrom,
            Vacancies = updatedJob.Vacancy,
            CategoryId = updatedJob.CategoryId,
            CategoryName = updatedJob.Category.Name,
            skillsRequiredList = updatedJob.JobSkills.Select(skill => new SkillDto
            {
                Id = skill.Id,
                Name = skill.Skill.Name
            }).ToList()
        };

        return new CommonDto<JobDto>
        {
            Data = jobDto,
            StatusCode = 200,
            Message = "Job updated successfully."
        };

    }

    public CommonDto<List<JobDto>> GetJobs(string search, int pageNumber, int pageSize, string skill, string location, int experience, string category)
    {
        var jobs = _jobRepository.GetJobs()
            .Select(job => new JobDto
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                Location = job.Location,
                ExperienceRequired = job.ExperienceRequired,
                OpenFrom = job.OpenFrom,
                Vacancies = job.Vacancy,
                CategoryId = job.CategoryId,
                CategoryName = job.Category.Name,
                skillsRequiredList = job.JobSkills.Select(skill => new SkillDto
                {
                    Id = skill.Id,
                    Name = skill.Skill.Name
                }).ToList()
            }).ToList();

        if (!string.IsNullOrEmpty(search))
        {
            jobs = jobs.Where(j => j.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                   j.Description.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                   j.Location.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                   j.CategoryName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        if (!string.IsNullOrEmpty(skill))
        {
            jobs = jobs.Where(j => j.skillsRequiredList.Any(s => s.Name.ToLower() == skill.ToLower())).ToList();
        }

        if (!string.IsNullOrEmpty(location))
        {
            jobs = jobs.Where(j => j.Location.ToLower() == location.ToLower()).ToList();
        }

        if (!string.IsNullOrEmpty(category))
        {
            jobs = jobs.Where(j => j.CategoryName.ToLower() == category.ToLower()).ToList();
        }

        if (experience > 0)
        {
            jobs = jobs.Where(u => u.ExperienceRequired <= experience).ToList();
        }

        jobs = jobs.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        var email = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email))
        {
            throw new UnAuthenticatedException();
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
            throw new UnAuthenticatedException();
        }

        var employer = _employerRepository.GetEmployerByEmail(email);

        if (employer == null)
        {
            throw new EmployerNotFoundException();
        }

        var jobs = _jobRepository.GetJobsByEmployer(employer.Id);

        var jobDtos = jobs.Select(job => new JobDto
        {
            Id = job.Id,
            Title = job.Title,
            Description = job.Description,
            Location = job.Location,
            ExperienceRequired = job.ExperienceRequired,
            OpenFrom = job.OpenFrom,
            Vacancies = job.Vacancy,
            CategoryId = job.CategoryId,
            CategoryName = job.Category.Name,
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
            throw new EmployerNotFoundException();
        }

        var jobs = _jobRepository.GetJobsByEmployer(employerId);

        var jobDtos = new List<JobDto>();
        if (jobs.Any())
        {
            jobDtos = jobs.Select(job => new JobDto
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                Location = job.Location,
                ExperienceRequired = job.ExperienceRequired,
                OpenFrom = job.OpenFrom,
                Vacancies = job.Vacancy,
                CategoryId = job.CategoryId,
                CategoryName = job.Category.Name,
                skillsRequiredList = job.JobSkills.Select(skill => new SkillDto
                {
                    Id = skill.Id,
                    Name = skill.Skill.Name
                }).ToList()
            }).ToList();
        }

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
            throw new UnAuthenticatedException();
        }

        var employer = _employerRepository.GetEmployerByEmail(email);

        if (employer == null)
        {
            throw new EmployerNotFoundException();
        }

        var jobIdCheck = _jobRepository.GetJobById(jobId);
        if (jobIdCheck == null)
        {
            throw new JobNotFoundException();
        }

        var isJobByEmployer = _jobRepository.IsJobByEmployer(jobId, employer);
        if (!isJobByEmployer)
        {
            throw new JobNotByEmployerException();
        }

        var IsAlreadyDeleted = _jobRepository.IsJobAlreadyDeleted(jobId);
        if (IsAlreadyDeleted)
        {
            throw new JobAlreadyDeleted();
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


    public CommonDto<List<CategoriesDto>> GetCategories()
    {
        var categories = _categoriesRepository.GetCategories()
            .Select(categories => new CategoriesDto
            {
                Id = categories.Id,
                Name = categories.Name
            })
            .ToList();

        return new CommonDto<List<CategoriesDto>>
        {
            Data = categories,
            StatusCode = 200,
            Message = "categories retieved successfully."
        };
    }


    public async Task<CommonDto<JobPreferenceInfoDto>> CreateJobPreference(JobPreferenceDto jobPreferenceDto)
    {
        var email = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email))
        {
            throw new UnAuthenticatedException();
        }

        var candidate = _candidateRepository.GetCandidateByEmail(email);
        if (candidate == null)
        {
            throw new CandidateNotFoundException();
        }

        var categoryNotExists = _categoriesRepository.GetCategories();
        if (categoryNotExists.All(c => c.Id != jobPreferenceDto.CategoryId))
        {
            throw new CategoryNotFoundException();
        }

        var jobPreference = new JobPreference
        {
            CandidateId = candidate.Id,
            CategoryId = jobPreferenceDto.CategoryId,
            ExperienceRequired = jobPreferenceDto.ExperienceRequired,
            Location = jobPreferenceDto.Location
        };

        var createdJobPreference = await _jobPreferenceRepository.CreateJobPreference(jobPreference);

        var jobPreferenceInfoDto = new JobPreferenceInfoDto
        {
            Id = createdJobPreference.Id,
            CandidateId = createdJobPreference.CandidateId,
            CategoryId = createdJobPreference.CategoryId,
            ExperienceRequired = createdJobPreference.ExperienceRequired,
            Location = createdJobPreference.Location
        };

        return new CommonDto<JobPreferenceInfoDto>
        {
            Data = jobPreferenceInfoDto,
            StatusCode = 201,
            Message = "Job preference created successfully."
        };

    }

    public async Task<CommonDto<JobPreferenceInfoDto>> UpdateJobPreference(JobPreferenceDto jobPreferenceDto)
    {
        var email = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email))
        {
            throw new UnAuthenticatedException();
        }

        var candidate = _candidateRepository.GetCandidateByEmail(email);
        if (candidate == null)
        {
            throw new CandidateNotFoundException();
        }

        var jobPreferenceExists = _jobPreferenceRepository.GetJobPreferenceById(jobPreferenceDto.Id);
        if (jobPreferenceExists == null)
        {
            throw new JobPreferenceNotExistsException();
        }

        if (jobPreferenceExists.CandidateId != candidate.Id)
        {
            throw new JobPreferenceNotBelongsToCandidateException();
        }

        var categoryNotExists = _categoriesRepository.GetCategories();
        if (categoryNotExists.All(c => c.Id != jobPreferenceDto.CategoryId))
        {
            throw new CategoryNotFoundException();
        }

        var jobPreference = new JobPreference
        {
            Id = jobPreferenceDto.Id,
            CandidateId = candidate.Id,
            CategoryId = jobPreferenceDto.CategoryId,
            ExperienceRequired = jobPreferenceDto.ExperienceRequired,
            Location = jobPreferenceDto.Location
        };

        var createdJobPreference = await _jobPreferenceRepository.UpdateJobPreference(jobPreference);

        var jobPreferenceInfoDto = new JobPreferenceInfoDto
        {
            Id = createdJobPreference.Id,
            CandidateId = createdJobPreference.CandidateId,
            CategoryId = createdJobPreference.CategoryId,
            ExperienceRequired = createdJobPreference.ExperienceRequired,
            Location = createdJobPreference.Location
        };

        return new CommonDto<JobPreferenceInfoDto>
        {
            Data = jobPreferenceInfoDto,
            StatusCode = 200,
            Message = "Job preference updated successfully."
        };
    }


    public async Task<CommonDto<object>> DeleteJobPreference(int jobPreferenceId)
    {
        var email = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email))
        {
            throw new UnAuthenticatedException();
        }

        var candidate = _candidateRepository.GetCandidateByEmail(email);
        if (candidate == null)
        {
            throw new CandidateNotFoundException();
        }

        var jobPreferenceExists = _jobPreferenceRepository.GetJobPreferenceById(jobPreferenceId);
        if (jobPreferenceExists == null)
        {
            throw new JobPreferenceNotExistsException();
        }

        if (jobPreferenceExists.CandidateId != candidate.Id)
        {
            throw new JobPreferenceNotBelongsToCandidateException();
        }

        await _jobPreferenceRepository.DeleteJobPreference(jobPreferenceId);

        return new CommonDto<object>
        {
            StatusCode = 200,
            Message = "Job preference Deleted successfully."
        };
    }

}
