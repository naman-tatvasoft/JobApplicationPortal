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
using Microsoft.EntityFrameworkCore;
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

    public CommonDto<JobInfoDto> GetJobById(int jobId)
    {
        var email = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email))
        {
            throw new UnAuthenticatedException();
        }

        var role = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        if (role == "Employer")
        {
            var employer = _employerRepository.GetEmployerByEmail(email);
            if (employer == null)
            {
                throw new EmployerNotFoundException();
            }

            var isJobByEmployer = _jobRepository.IsJobByEmployer(jobId, employer);
            if (!isJobByEmployer)
            {
                throw new JobNotByEmployerException();
            }
        }

        var job = _jobRepository.GetJobById(jobId);
        if (job == null)
        {
            throw new JobNotFoundException();
        }

        var jobDto = new JobInfoDto
        {
            Id = job.Id,
            Title = job.Title,
            Description = job.Description,
            Location = job.Location,
            ExperienceRequired = job.ExperienceRequired,
            OpenFrom = job.OpenFrom,
            Vacancies = job.Vacancy,
            IsActive = (bool)job.IsActive,
            CategoryId = job.CategoryId,
            CategoryName = job.Category.Name,
            skillsRequiredList = job.JobSkills.Where(skill => skill.Skill != null).Select(skill => new SkillDto
            {
                Id = skill.Id,
                Name = skill.Skill.Name
            }).ToList()
        };

        return new CommonDto<JobInfoDto>
        {
            Data = jobDto,
            StatusCode = 200,
            Message = "Job retrieved successfully."
        };
    }

    public async Task<CommonDto<JobInfoDto>> UpdateJob(int jobId, JobDto updateJobDto)
    {
        if (jobId != updateJobDto.Id)
        {
            throw new IdMismatchException();
        }

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
        var jobDto = new JobInfoDto
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

        return new CommonDto<JobInfoDto>
        {
            Data = jobDto,
            StatusCode = 200,
            Message = "Job updated successfully."
        };

    }

    public CommonDto<List<JobInfoDto>> GetJobs(string search, int pageNumber, int pageSize, string skill, string location, int experience, string category)
    {
        var email = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        var jobs = _jobRepository.GetJobs().Include(job => job.Applications).ThenInclude(a => a.Candidate).ThenInclude(c => c.User)
            .Select(job => new JobInfoDto
            {
                Id = job.Id,
                Title = job.Title,
                CompanyName = job.Employer.CompanyName,
                Description = job.Description,
                Location = job.Location,
                ExperienceRequired = job.ExperienceRequired,
                OpenFrom = job.OpenFrom,
                Vacancies = job.Vacancy,
                CategoryId = job.CategoryId,
                CategoryName = job.Category.Name,
                skillsRequiredList = job.JobSkills.Select(skill => new SkillDto
                {
                    Id = skill.Skill.Id,
                    Name = skill.Skill.Name
                }).ToList(),
                IsApplied = job.Applications.Any(a => a.Candidate.User.Email == email && a.StatusId != 5)
            });

        if (!string.IsNullOrEmpty(search))
        {
            var lowerSearch = search.ToLower();
            jobs = jobs.Where(j => j.Title.ToLower().Contains(lowerSearch) ||
                                   j.Description.ToLower().Contains(lowerSearch) ||
                                   j.Location.ToLower().Contains(lowerSearch) ||
                                   j.CategoryName.ToLower().Contains(lowerSearch));
        }

        if (!string.IsNullOrEmpty(skill))
        {
            jobs = jobs.Where(j => j.skillsRequiredList.Any(s => s.Name.ToLower() == skill.ToLower()));
        }

        if (!string.IsNullOrEmpty(location))
        {
            jobs = jobs.Where(j => j.Location.ToLower() == location.ToLower());
        }

        if (!string.IsNullOrEmpty(category))
        {
            jobs = jobs.Where(j => j.CategoryName.ToLower() == category.ToLower());
        }

        if (experience > 0)
        {
            jobs = jobs.Where(u => u.ExperienceRequired <= experience);
        }


        if (string.IsNullOrEmpty(email))
        {
            throw new UnAuthenticatedException();
        }

        var candidate = _candidateRepository.GetCandidateByEmail(email);

        if (candidate != null)
        {
            jobs = jobs.Where(j => j.OpenFrom <= DateOnly.FromDateTime(DateTime.Now));
        }

        jobs = jobs.Skip((pageNumber - 1) * pageSize).Take(pageSize);


        return new CommonDto<List<JobInfoDto>>
        {
            Data = jobs != null ? jobs.ToList() : new List<JobInfoDto>(),
            StatusCode = 200,
            Message = "Jobs retrieved successfully."
        };
    }

    public CommonDto<List<JobInfoDto>> GetCreatedJobs(string search, int pageNumber, int pageSize, string skill, string location, int experience, string category)
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

        var jobs = _jobRepository.GetJobsByEmployer(employer.Id).Select(job => new JobInfoDto
        {
            Id = job.Id,
            Title = job.Title,
            CompanyName = job.Employer.CompanyName,
            Description = job.Description,
            Location = job.Location,
            ExperienceRequired = job.ExperienceRequired,
            OpenFrom = job.OpenFrom,
            Vacancies = job.Vacancy,
            CategoryId = job.CategoryId,
            CategoryName = job.Category.Name,
            skillsRequiredList = job.JobSkills.Select(skill => new SkillDto
            {
                Id = skill.Skill.Id,
                Name = skill.Skill.Name
            }).ToList()
        });

        if (!string.IsNullOrEmpty(search))
        {
            var lowerSearch = search.ToLower();
            jobs = jobs.Where(j => j.Title.ToLower().Contains(lowerSearch) ||
                                   j.Description.ToLower().Contains(lowerSearch) ||
                                   j.Location.ToLower().Contains(lowerSearch) ||
                                   j.CategoryName.ToLower().Contains(lowerSearch));
        }

        if (!string.IsNullOrEmpty(skill))
        {
            jobs = jobs.Where(j => j.skillsRequiredList.Any(s => s.Name.ToLower() == skill.ToLower()));
        }

        if (!string.IsNullOrEmpty(location))
        {
            jobs = jobs.Where(j => j.Location.ToLower() == location.ToLower());
        }

        if (!string.IsNullOrEmpty(category))
        {
            jobs = jobs.Where(j => j.CategoryName.ToLower() == category.ToLower());
        }

        if (experience > 0)
        {
            jobs = jobs.Where(u => u.ExperienceRequired <= experience);
        }

        jobs = jobs.Skip((pageNumber - 1) * pageSize).Take(pageSize);

        return new CommonDto<List<JobInfoDto>>
        {
            Data = jobs.ToList(),
            StatusCode = 200,
            Message = "Jobs retrieved successfully."
        };
    }

    public CommonDto<List<JobInfoDto>> GetJobsByEmployer(int employerId)
    {
        if (!_employerRepository.IsEmployerIdExist(employerId))
        {
            throw new EmployerNotFoundException();
        }

        var jobs = _jobRepository.GetJobsByEmployer(employerId);

        var jobDtos = new List<JobInfoDto>();
        if (jobs.Any())
        {
            jobDtos = jobs.Select(job => new JobInfoDto
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

        return new CommonDto<List<JobInfoDto>>
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

    public async Task<CommonDto<JobPreferenceInfoDto>> UpdateJobPreference(int jobPreferenceId, JobPreferenceDto jobPreferenceDto)
    {
        if (jobPreferenceId != jobPreferenceDto.Id)
        {
            throw new IdMismatchException();
        }

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

    public async Task<CommonDto<CategoriesDto>> CreateCategory(CategoriesDto categoriesDto)
    {
        var categoryExists = _categoriesRepository.GetCategoryByName(categoriesDto.Name);
        if (categoryExists != null)
        {
            throw new CategoryAlreadyExistsException();
        }

        var category = new Category
        {
            Name = categoriesDto.Name
        };

        var createdCategory = await _categoriesRepository.CreateCategory(category);

        var createdCategoryDto = new CategoriesDto
        {
            Id = createdCategory.Id,
            Name = createdCategory.Name
        };

        return new CommonDto<CategoriesDto>
        {
            Data = createdCategoryDto,
            StatusCode = 201,
            Message = "Category created successfully."
        };
    }

    public async Task<CommonDto<SkillDto>> CreateSkill(SkillDto skillDto)
    {
        var skillExists = _skillRepository.GetSkillByName(skillDto.Name);
        if (skillExists != null)
        {
            throw new SkillAlreadyExistsException();
        }

        var skill = new Skill
        {
            Name = skillDto.Name
        };

        var createdSkill = await _skillRepository.CreateSkill(skill);

        var createdSkillDto = new SkillDto
        {
            Id = createdSkill.Id,
            Name = createdSkill.Name
        };

        return new CommonDto<SkillDto>
        {
            Data = createdSkillDto,
            StatusCode = 201,
            Message = "Skill created successfully."
        };
    }

    public string GetSkillNameById(int skillId)
    {
        var skillName = _skillRepository.GetSkillNameById(skillId);
        if (string.IsNullOrEmpty(skillName))
        {
            throw new SkillNotFoundException();
        }
        return skillName;
    }

    public async Task<CommonDto<SkillDto>> UpdateSkill(int skillId, SkillDto skillDto)
    {
        var skillIdCheck = _skillRepository.GetSkillNameById(skillId);
        if (string.IsNullOrEmpty(skillIdCheck))
        {
            throw new SkillNotFoundException();
        }

        var skill = new Skill
        {
            Id = skillId,
            Name = skillDto.Name
        };

        var updatedSkill = await _skillRepository.UpdateSkill(skill);

        var updatedSkillDto = new SkillDto
        {
            Id = updatedSkill.Id,
            Name = updatedSkill.Name
        };

        return new CommonDto<SkillDto>
        {
            Data = updatedSkillDto,
            StatusCode = 200,
            Message = "Skill updated successfully."
        };
    }

    public string GetCategoryNameById(int categoryId)
    {
        var categoryName = _categoriesRepository.GetCategoryNameById(categoryId);
        if (string.IsNullOrEmpty(categoryName))
        {
            throw new CategoryNotFoundException();
        }
        return categoryName;
    }

    public async Task<CommonDto<CategoriesDto>> UpdateCategory(int categoryId, CategoriesDto categoriesDto)
    {
        var categoryName = _categoriesRepository.GetCategoryNameById(categoryId);
        if (string.IsNullOrEmpty(categoryName))
        {
            throw new CategoryNotFoundException();
        }

        var category = new Category
        {
            Id = categoryId,
            Name = categoriesDto.Name
        };

        var updatedCategory = await _categoriesRepository.UpdateCategory(category);

        var updatedCategoryDto = new CategoriesDto
        {
            Id = updatedCategory.Id,
            Name = updatedCategory.Name
        };

        return new CommonDto<CategoriesDto>
        {
            Data = updatedCategoryDto,
            StatusCode = 200,
            Message = "Category updated successfully."
        };
    }

    public async Task<CommonDto<object>> DeleteCategory(int categoryId)
    {
        var categoryExists = _categoriesRepository.GetCategoryNameById(categoryId);
        if (string.IsNullOrEmpty(categoryExists))
        {
            throw new CategoryNotFoundException();
        }

        await _categoriesRepository.DeleteCategory(categoryId);

        return new CommonDto<object>
        {
            StatusCode = 200,
            Message = "Category deleted successfully."
        };
    }

    public async Task<CommonDto<object>> DeleteSkill(int skillId)
    {
        var skillExists = _skillRepository.GetSkillNameById(skillId);
        if (string.IsNullOrEmpty(skillExists))
        {
            throw new SkillNotFoundException();
        }

        await _skillRepository.DeleteSkill(skillId);

        return new CommonDto<object>
        {
            StatusCode = 200,
            Message = "skill deleted successfully."
        };
    }

}
