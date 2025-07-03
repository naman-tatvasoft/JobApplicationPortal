using System.Security.Claims;
using JobApplicationPortal.DataModels.Dtos.RequestDtos;
using JobApplicationPortal.DataModels.Dtos.ResponseDtos;
using JobApplicationPortal.DataModels.Models;
using JobApplicationPortal.Repository.Repository.Interface;
using JobApplicationPortal.Service.Exceptions;
using JobApplicationPortal.Service.Service.Interface;
using Microsoft.AspNetCore.Http;

namespace JobApplicationPortal.Service.Service.Implementation;

public class UserService : IUserService
{
    private readonly IEmployerRepository _employerRepository;
    private readonly ICandidateRepository _candidateRepository;
    private readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public UserService(IEmployerRepository employerRepository,
        ICandidateRepository candidateRepository,
        IUserRepository userRepository,
            IHttpContextAccessor httpContextAccessor)
    {
        _employerRepository = employerRepository;
        _candidateRepository = candidateRepository;
        _userRepository = userRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public CommonDto<List<EmployerInfoDto>> GetEmployers()
    {

        var employersInfoList = _employerRepository.GetAllEmployers().Select(e => new EmployerInfoDto
        {
            Id = e.Id,
            Name = e.Name,
            CompanyName = e.CompanyName,
            Email = e.User.Email
        }).ToList();

        return new CommonDto<List<EmployerInfoDto>>
        {
            StatusCode = 200,
            Message = "Employers retrieved successfully",
            Data = employersInfoList
        };
    }

    public CommonDto<List<CandidateInfoDto>> GetCandidates()
    {
        var candidatesInfoList = _candidateRepository.GetAllCandidates().Select(c => new CandidateInfoDto
        {
            Id = c.Id,
            Name = c.Name,
            Email = c.User.Email
        }).ToList();

        return new CommonDto<List<CandidateInfoDto>>
        {
            StatusCode = 200,
            Message = "Candidates retrieved successfully",
            Data = candidatesInfoList
        };
    }

    public CommonDto<UserProfileDto> GetProfile()
    {
        var role = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        if (role == "Employer")
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

            var userProfile = new UserProfileDto
            {
                Id = employer.Id,
                Name = employer.Name,
                CompanyName = employer.CompanyName,
                Email = email
            };

            return new CommonDto<UserProfileDto>
            {
                StatusCode = 200,
                Message = "Employer profile retrieved successfully",
                Data = userProfile
            };

        }
        else if (role == "Candidate")
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
            var userProfile = new UserProfileDto
            {
                Id = candidate.Id,
                Name = candidate.Name,
                Email = email
            };

            return new CommonDto<UserProfileDto>
            {
                StatusCode = 200,
                Message = "Candidate profile retrieved successfully",
                Data = userProfile
            };  
        }
        else
        {
            throw new UnauthorizedAccessException("Unauthorized access");
        }
    }

    public async Task<CommonDto<UserProfileDto>> UpdateEmployerProfile(UpdateEmployerProfileDto updateEmployerProfileDto)
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

        var employerToUpdate = new Employer{
            Id = employer.Id,
            Name = updateEmployerProfileDto.Name,
            CompanyName = updateEmployerProfileDto.CompanyName
        };

        var result = await _employerRepository.UpdateEmployer(employerToUpdate);

        var userToUpdate = new User{
            Id = employer.UserId,
            Email = updateEmployerProfileDto.Email
        };

        var resultUser = await _userRepository.UpdateUserAsync(userToUpdate);

        var userProfile = new UserProfileDto
        {
            Id = result.Id,
            Name = result.Name,
            CompanyName = result.CompanyName,
            Email = resultUser.Email
        };

        return new CommonDto<UserProfileDto>
        {
            StatusCode = 200,
            Message = "Employer profile updated successfully",
            Data = userProfile
        };
    }

     public async Task<CommonDto<UserProfileDto>> UpdateCandidateProfile(UpdateCandidateProfileDto updateCandidateProfileDto)
    {
        var email = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email))
        {
            throw new UnAuthenticatedException();
        }

        var candidate = _candidateRepository.GetCandidateByEmail(email);
        if (candidate == null)
        {
            throw new UnauthorizedAccessException();
        }

        var employerToUpdate = new Candidate{
            Id = candidate.Id,
            Name = updateCandidateProfileDto.Name
        };

        var result = await _candidateRepository.UpdateCandidate(employerToUpdate);

        var userToUpdate = new User{
            Id = result.UserId,
            Email = updateCandidateProfileDto.Email
        };

        var resultUser = await _userRepository.UpdateUserAsync(userToUpdate);

        var userProfile = new UserProfileDto
        {
            Id = result.Id,
            Name = result.Name,
            Email = resultUser.Email
        };

        return new CommonDto<UserProfileDto>
        {
            StatusCode = 200,
            Message = "Candidate profile updated successfully",
            Data = userProfile
        };
    }

}
