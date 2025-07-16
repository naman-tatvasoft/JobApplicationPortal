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

public class UserService : IUserService
{
    private readonly IEmployerRepository _employerRepository;
    private readonly ICandidateRepository _candidateRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JwtHelper _jwtHelper;

    public UserService(IEmployerRepository employerRepository,
        ICandidateRepository candidateRepository,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        JwtHelper jwtHelper,
            IHttpContextAccessor httpContextAccessor)
    {
        _employerRepository = employerRepository;
        _candidateRepository = candidateRepository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _jwtHelper = jwtHelper;
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

        var employerToUpdate = new Employer
        {
            Id = employer.Id,
            Name = updateEmployerProfileDto.Name,
            CompanyName = updateEmployerProfileDto.CompanyName
        };

        var result = await _employerRepository.UpdateEmployer(employerToUpdate);

        var userToUpdate = new User
        {
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

        var employerToUpdate = new Candidate
        {
            Id = candidate.Id,
            Name = updateCandidateProfileDto.Name
        };

        var result = await _candidateRepository.UpdateCandidate(employerToUpdate);

        var userToUpdate = new User
        {
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

    public CommonDto<List<RolesDto>> GetRoles()
    {
        var roles = _roleRepository.GetRoles()
            .Select(role => new RolesDto
            {
                Id = role.Id,
                Name = role.Name
            })
            .ToList();

        return new CommonDto<List<RolesDto>>
        {
            Data = roles,
            StatusCode = 200,
            Message = "Roles retieved successfully."
        };
    }

    public CommonDto<List<UserInfoDto>> GetUsers(string search, int pageNumber, int pageSize, string role)
    {
        var users = _userRepository.GetUsers()
            .Select(u => new UserInfoDto
            {
                id = u.Id,
                email = u.Email,
                name = u.Role.Name == "Employer" ? u.Employer.Name : u.Role.Name == "Candidate" ? u.Candidate.Name : "Admin",
                roleName = u.Role.Name
            });

        if (!string.IsNullOrEmpty(search))
        {
            var lowerSearch = search.ToLower();
            users = users.Where(u => u.email.ToLower().Contains(lowerSearch) ||
                                     u.name.ToLower().Contains(lowerSearch) );
        }

        if (!string.IsNullOrEmpty(role))
        {
            users = users.Where(u => u.roleName.ToLower() == role.ToLower());
        }

        users = users.Skip((pageNumber - 1) * pageSize).Take(pageSize);

        return new CommonDto<List<UserInfoDto>>
        {
            Data = users.ToList(),
            StatusCode = 200,
            Message = "Users retrieved successfully."
        };
    }

    
     public async Task<CommonDto<object>> GetId(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            throw new UnAuthenticatedException();
        }

        var isValid = _jwtHelper.ValidateToken(token);
        if (!isValid)
        {
            throw new UnAuthenticatedException();
        }

        var roleName = _jwtHelper.GetClaimsFromToken(token)?.FindFirst("role")?.Value;
        var userEmail = _jwtHelper.GetClaimsFromToken(token)?.FindFirst("email")?.Value;
        var user = _userRepository.GetUserByEmail(userEmail);
        var userId = roleName== "Employer" ? user.Employer?.Id :  user.Candidate?.Id;

        return new CommonDto<object>
        {
            StatusCode = 200,
            Message = "Id is Fetched.",
            Data = userId 
        };
    }

}
