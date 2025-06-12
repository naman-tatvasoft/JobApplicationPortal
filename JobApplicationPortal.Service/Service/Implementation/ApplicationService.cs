using System.Security.Claims;
using JobApplicationPortal.DataModels.Dtos.RequestDtos;
using JobApplicationPortal.DataModels.Dtos.ResponseDtos;
using JobApplicationPortal.DataModels.Models;
using JobApplicationPortal.Repository.Repository.Interface;
using JobApplicationPortal.Service.Service.Interface;
using Microsoft.AspNetCore.Http;

namespace JobApplicationPortal.Service.Service.Implementation;

public class ApplicationService : IApplicationService
{
    private readonly IStatusRepository _statusRepository;
    private readonly IEmployerRepository _employerRepository;
    private readonly ICandidateRepository _candidateRepository;
    private readonly IApplicationRepository _applicationRepository;
    private readonly IJobRepository _jobRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public ApplicationService(IStatusRepository statusRepository,
            IEmployerRepository employerRepository,
            ICandidateRepository candidateRepository,
            IApplicationRepository applicationRepository,
            IJobRepository jobRepository,
            IHttpContextAccessor httpContextAccessor)
    {
        _statusRepository = statusRepository;
        _employerRepository = employerRepository;
        _candidateRepository = candidateRepository;
        _applicationRepository = applicationRepository;
        _jobRepository = jobRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<CommonDto<Application>> JobApplication(ApplicationDto applicationDto)
    {
        var email = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email))
        {
            return new CommonDto<Application>
            {
                StatusCode = 401,
                Message = "User is not authenticated."
            };
        }

        var candidate = _candidateRepository.GetCandidateByEmail(email);
        if (candidate == null)
        {
            return new CommonDto<Application>
            {
                StatusCode = 403,
                Message = "Candidate not found."
            };
        }

        var jobIdCheck = _jobRepository.CheckJobById(applicationDto.JobId);
        if (!jobIdCheck)
        {
            return new CommonDto<Application>
            {
                StatusCode = 400,
                Message = "Job not found or not active or deleted or not still open to apply."
            };
        }

        var isAlreadyApplied = await _applicationRepository.CheckAlreadyApplied(applicationDto.JobId, candidate.Id);
        if( isAlreadyApplied)
        {
            return new CommonDto<Application>
            {
                StatusCode = 400,
                Message = "You have already applied for this job."
            };
        }

        var coverLetterFileName = string.Empty;
        if (applicationDto.CoverLetter != null && applicationDto.CoverLetter.Length > 0)
        {
            var coverLetterPath = Path.Combine("uploads", "CoverLetters");
            if (!Directory.Exists(coverLetterPath))
            {
                Directory.CreateDirectory(coverLetterPath);
            }

            coverLetterFileName = $"{Guid.NewGuid()}_{applicationDto.CoverLetter.FileName}";
            var filePath = Path.Combine(coverLetterPath, coverLetterFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await applicationDto.CoverLetter.CopyToAsync(stream);
            }
        }

        var resumeFileName = string.Empty;
        if (applicationDto.Resume != null && applicationDto.Resume.Length > 0)
        {

            var resumePath = Path.Combine("uploads", "Resumes");
            if (!Directory.Exists(resumePath))
            {
                Directory.CreateDirectory(resumePath);
            }
            resumeFileName = $"{Guid.NewGuid()}_{applicationDto.Resume.FileName}";
            var filePath = Path.Combine(resumePath, resumeFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await applicationDto.Resume.CopyToAsync(stream);
            }
        }

        var application = new Application
        {
            CandidateId = candidate.Id,
            JobId = applicationDto.JobId,
            Experience = applicationDto.Experience,
            NoteForEmployer = applicationDto.NoteForEmployer,
            CoverLetter = coverLetterFileName,
            Resume = resumeFileName,
            StatusId = _statusRepository.GetStatusIdByName("Applied") != 0 ? _statusRepository.GetStatusIdByName("Applied") : 4
        };

        var createdApplication = await _applicationRepository.CreateApplication(application);

        return new CommonDto<Application>
        {
            StatusCode = 201,
            Message = "Job application submitted successfully.",
            Data = createdApplication
        };
    }

    public CommonDto<List<ApplicationInfoDto>> GetApplications()
    {
        var applicationInfo = _applicationRepository.GetApplications();
        if (applicationInfo == null || !applicationInfo.Any())
        {
            return new CommonDto<List<ApplicationInfoDto>>
            {
                StatusCode = 400,
                Message = "No applications found."
            };
        }

        return new CommonDto<List<ApplicationInfoDto>>
        {
            Data = applicationInfo,
            StatusCode = 200,
            Message = "Applications retrieved successfully."
        };
    }

    public CommonDto<List<ApplicationInfoDto>> GetApplicationsByCandidate()
    {
        var email = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email))
        {
            return new CommonDto<List<ApplicationInfoDto>>
            {
                StatusCode = 401,
                Message = "User is not authenticated."
            };
        }

        var candidate = _candidateRepository.GetCandidateByEmail(email);
        if (candidate == null)
        {
            return new CommonDto<List<ApplicationInfoDto>>
            {
                StatusCode = 403,
                Message = "You are not authorized to view applications."
            };
        }

        var applicationInfo = _applicationRepository.GetApplicationsByCandidate(candidate.Id);
        if (applicationInfo == null || !applicationInfo.Any())
        {
            return new CommonDto<List<ApplicationInfoDto>>
            {
                StatusCode = 400,
                Message = "No applications found for this Candidate."
            };
        }

        return new CommonDto<List<ApplicationInfoDto>>
        {
            Data = applicationInfo,
            StatusCode = 200,
            Message = "Applications retrieved successfully."
        };
    }

    public CommonDto<List<ApplicationInfoDto>> GetApplicationsByJob(int jobId)
    {
        var email = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email))
        {
            return new CommonDto<List<ApplicationInfoDto>>
            {
                StatusCode = 401,
                Message = "User is not authenticated."
            };
        }

        var employer = _employerRepository.GetEmployerByEmail(email);
        if (employer == null)
        {
            return new CommonDto<List<ApplicationInfoDto>>
            {
                StatusCode = 403,
                Message = "You are not authorized to update the application status."
            };
        }
        var jobIdCheck = _jobRepository.GetJobById(jobId);
        if (jobIdCheck == null)
        {
            return new CommonDto<List<ApplicationInfoDto>>
            {
                StatusCode = 400,
                Message = "Job not found."
            };
        }

        var isJobByEmployer = _jobRepository.IsJobByEmployer(jobId, employer);
        if (!isJobByEmployer)
        {
            return new CommonDto<List<ApplicationInfoDto>>
            {
                StatusCode = 400,
                Message = "Job is not created by the employer."
            };
        }

        var applicationInfo = _applicationRepository.GetApplicationsByJob(jobId);
        if (applicationInfo == null || !applicationInfo.Any())
        {
            return new CommonDto<List<ApplicationInfoDto>>
            {
                StatusCode = 400,
                Message = "No applications found for this job."
            };
        }

        return new CommonDto<List<ApplicationInfoDto>>
        {
            Data = applicationInfo,
            StatusCode = 200,
            Message = "Applications retrieved successfully."
        };
    }

    public async Task<CommonDto<object>> UpdateStatus(int applicationId, int statusId)
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
                StatusCode = 403,
                Message = "You are not authorized to update the application status."
            };
        }

        var applicationIdCheck = _applicationRepository.GetApplicationById(applicationId);
        if (applicationIdCheck == null)
        {
            return new CommonDto<object>
            {
                StatusCode = 400,
                Message = "Application not found."
            };
        }

        var jobByEmployer = _applicationRepository.JobByEmployer(applicationIdCheck.JobId, employer.Id);
        if (!jobByEmployer)
        {
            return new CommonDto<object>
            {
                StatusCode = 403,
                Message = "You are not authorized to update this application status."
            };
        }

        var updatedaApplication = await _applicationRepository.UpdateApplicationStatus(applicationId, statusId);
        if (updatedaApplication == null)
        {
            return new CommonDto<object>
            {
                StatusCode = 500,
                Message = "Failed to update application status."
            };
        }

        return new CommonDto<object>
        {
            StatusCode = 200,
            Message = "Application status updated successfully."
        };

    }

    public CommonDto<List<StatusDto>> GetStatuses()
    {
        var status = _statusRepository.GetStatuses()
                   .Select(status => new StatusDto
                   {
                       Id = status.Id,
                       Name = status.Name
                   })
                   .ToList();

        return new CommonDto<List<StatusDto>>
        {
            Data = status,
            StatusCode = 200,
            Message = "Status retieved successfully."
        };
    }
}
