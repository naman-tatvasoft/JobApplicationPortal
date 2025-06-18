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
            throw new UnAuthenticatedException();
        }

        var candidate = _candidateRepository.GetCandidateByEmail(email);
        if (candidate == null)
        {
            throw new CandidateNotFoundException();
        }

        var jobIdCheck = _jobRepository.CheckJobById(applicationDto.JobId);
        if (!jobIdCheck)
        {
            throw new JobNotFoundOrNotOpenToApplyException();
        }

        var isAlreadyApplied = await _applicationRepository.CheckAlreadyApplied(applicationDto.JobId, candidate.Id);
        if (isAlreadyApplied)
        {
            throw new JobAlreadyAppliedException();
        }

        var isEnoughExperience = _jobRepository.CheckExperience(applicationDto.JobId, applicationDto.Experience);
        if (!isEnoughExperience)
        {
            throw new NotEnoughExperienceException();
        }

        var coverLetterFileName = string.Empty;

        if (applicationDto.CoverLetter != null && applicationDto.CoverLetter.ContentType != "application/pdf")
        {
            throw new InvalidFileTypeException();
        }

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

        if (applicationDto.Resume != null && applicationDto.Resume.ContentType != "application/pdf")
        {
            throw new InvalidFileTypeException();
        }

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

        var employerEmail = _jobRepository.GetEmployerEmailByJobId(applicationDto.JobId);
        var jobTitle = _jobRepository.GetJobById(applicationDto.JobId)?.Title;


        var sub = $@"Application Received for {jobTitle}";
        var body = $@"
            <div style='max-width: 500px; font-family: Arial, sans-serif; border: 1px solid #ddd;'>
            <div style='background: #006CAC; padding: 10px; text-align: center; height:90px; max-width:100%; display: flex; justify-content: center; align-items: center;'>
                <span style='color: #fff; font-size: 24px; margin-left: 10px; font-weight: 600;'>Job Portal</span>
            </div>
            <div style='padding: 20px 5px; background-color: #e8e8e8;'>
                <p>Job Portal</p>
                <p>New Application Recieved for job title {jobTitle}</p>
                <p>If you encounter any issues or have any questions, please do not hesitate to contact our support team.</p>
                <p><strong style='color: orange;'>Important Note:</strong>
                    If you did not created this job, please ignore this email or contact our support team immediately.
                </p>
            </div>
            </div>";

        await EmailHelper.SendEmailAsync(
                receiverEmailAddress: employerEmail,
                receiverDisplayName: "Employer",
                subject: sub,
                body: body
            );

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
            throw new UnAuthenticatedException();
        }

        var candidate = _candidateRepository.GetCandidateByEmail(email);
        if (candidate == null)
        {
            throw new CandidateNotFoundException();
        }

        var applicationInfo = _applicationRepository.GetApplicationsByCandidate(candidate.Id);

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
            throw new UnAuthenticatedException();
        }

        var employer = _employerRepository.GetEmployerByEmail(email);
        if (employer == null)
        {
            throw new UnauthorizedAccessException();
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

        var applicationInfo = _applicationRepository.GetApplicationsByJob(jobId);

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
            throw new UnAuthenticatedException();
        }

        var employer = _employerRepository.GetEmployerByEmail(email);
        if (employer == null)
        {
            throw new UnauthorizedAccessException();
        }

        var applicationIdCheck = _applicationRepository.GetApplicationById(applicationId);
        if (applicationIdCheck == null)
        {
            throw new ApplicationNotFoundException();
        }

        var jobByEmployer = _jobRepository.IsJobByEmployer(applicationIdCheck.JobId, employer);
        if (!jobByEmployer)
        {
            throw new UnauthorizedAccessException();
        }

        if (_statusRepository.GetStatusIdByName("Withdrawn") == statusId)
        {
            throw new UnauthorizedAccessException();
        }

        var updatedaApplication = await _applicationRepository.UpdateApplicationStatus(applicationId, statusId);

        if (_statusRepository.GetStatusIdByName("Hired") == statusId)
        {
            await _jobRepository.ReduceVacancy(updatedaApplication.JobId);
        }

        var candidateEmail = _applicationRepository.GetCandidateEmailByApplicationId(applicationId);
        var statusName = _statusRepository.GetStatusNameById(statusId);


        var sub = "Application status updated";
        var body = $@"
            <div style='max-width: 500px; font-family: Arial, sans-serif; border: 1px solid #ddd;'>
            <div style='background: #006CAC; padding: 10px; text-align: center; height:90px; max-width:100%; display: flex; justify-content: center; align-items: center;'>
                <span style='color: #fff; font-size: 24px; margin-left: 10px; font-weight: 600;'>Job Portal</span>
            </div>
            <div style='padding: 20px 5px; background-color: #e8e8e8;'>
                <p>Job Portal</p>
                <p>Your applied application status is updated to {statusName}</p>
                <p>If you encounter any issues or have any questions, please do not hesitate to contact our support team.</p>
                <p><strong style='color: orange;'>Important Note:</strong>
                    If you did not Applied for any job, please ignore this email or contact our support team immediately.
                </p>
            </div>
            </div>";

        await EmailHelper.SendEmailAsync(
                     receiverEmailAddress: candidateEmail,
                     receiverDisplayName: "Candidate",
                     subject: sub,
                     body: body
                 );

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

    public CommonDto<int> GetTotalApplicationByJob(int jobId)
    {
        var email = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email))
        {
            throw new UnAuthenticatedException();
        }

        var jobIdCheck = _jobRepository.GetJobById(jobId);
        if (jobIdCheck == null)
        {
            throw new JobNotFoundException();
        }

        var employer = _employerRepository.GetEmployerByEmail(email);
        if (employer != null)
        {
            var isJobByEmployer = _jobRepository.IsJobByEmployer(jobId, employer);
            if (!isJobByEmployer)
            {
                throw new JobNotByEmployerException();
            }

            var totalApplicationsEmployer = _applicationRepository.GetApplicationsByJob(jobId).Count;
        }

        var totalApplications = _applicationRepository.GetApplicationsByJob(jobId).Count;

        return new CommonDto<int>
        {
            Data = totalApplications,
            StatusCode = 200,
            Message = "Total applications retrieved successfully."
        };

    }

    public async Task<CommonDto<object>> WithdrawApplication(int applicationId)
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

        var applicationIdCheck = _applicationRepository.GetApplicationById(applicationId);
        if (applicationIdCheck == null)
        {
            throw new ApplicationNotFoundException();
        }

        if (applicationIdCheck.CandidateId != candidate.Id)
        {
            throw new UnauthorizedAccessException();
        }

        await _applicationRepository.WithdrawApplication(applicationId);

        return new CommonDto<object>
        {
            StatusCode = 200,
            Message = "Application withdrawn successfully."
        };
    }
}
