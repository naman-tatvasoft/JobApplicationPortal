using System.Security.Claims;
using JobApplicationPortal.DataModels.Dtos.ResponseDtos;
using JobApplicationPortal.Repository.Repository.Interface;
using JobApplicationPortal.Service.Service.Interface;
using Microsoft.AspNetCore.Http;

namespace JobApplicationPortal.Service.Service.Implementation;

public class ApplicationService : IApplicationService
{
    private readonly IStatusRepository _statusRepository;
    private readonly IEmployerRepository _employerRepository;
    private readonly IApplicationRepository _applicationRepository;
    private readonly IJobRepository _jobRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public ApplicationService(IStatusRepository statusRepository,
            IEmployerRepository employerRepository,
            IApplicationRepository applicationRepository,
            IJobRepository jobRepository,
            IHttpContextAccessor httpContextAccessor)
    {
        _statusRepository = statusRepository;
        _employerRepository = employerRepository;
        _applicationRepository = applicationRepository;
        _jobRepository = jobRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    //   public async Task<CommonDto> JobApplication(ApplicationDto applicationDto){
    //   }

    //   public CommonDto GetApplications(){
    //   } 

    //   public CommonDto GetApplicationsByCandidate(){
    //   }

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
