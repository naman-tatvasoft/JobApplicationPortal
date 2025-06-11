using JobApplicationPortal.DataModels.Dtos.RequestDtos;
using JobApplicationPortal.DataModels.Dtos.ResponseDtos;
using JobApplicationPortal.DataModels.Models;
using JobApplicationPortal.Repository.Repository.Interface;
using JobApplicationPortal.Service.Helper;
using JobApplicationPortal.Service.Service.Interface;

namespace JobApplicationPortal.Service.Service.Implementation;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IEmployerRepository _employerRepository;
    private readonly ICandidateRepository _candidateRepository;
    private readonly IRoleRepository _roleRepository;

    private readonly JwtHelper _jwtHelper;

    public AuthService(IUserRepository userRepository, 
        IEmployerRepository employerRepository, 
        ICandidateRepository candidateRepository, 
        IRoleRepository roleRepository, 
        JwtHelper jwtHelper)
    {
        _employerRepository = employerRepository;
        _candidateRepository = candidateRepository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _jwtHelper = jwtHelper;
    }

    public async Task<CommonDto<object>> RegisterEmployer(RegisterEmployerDto registerEmployerDto)
    {
        if (registerEmployerDto == null)
        {
            return new CommonDto<object>
            {
                StatusCode = 400,
                Message = "Invalid registration data."
            };
        }

        if (await _userRepository.IsEmailExists(registerEmployerDto.Email))
        {
            return new CommonDto<object>
            {
                StatusCode = 400,
                Message = "Email already exists."
            };
        }

        var hashedPassword = PasswordHasher.Hash(registerEmployerDto.Password);
        var user = new User
        {
            Email = registerEmployerDto.Email,
            Password = hashedPassword,
            RoleId = _roleRepository.GetRoleIdByName("Employer")
        };
        var savedUser = await _userRepository.AddUserAsync(user);

        var employer = new Employer
        {
            Name = registerEmployerDto.Name,
            CompanyName = registerEmployerDto.CompanyName,
            UserId = savedUser.Id
        };
        await _employerRepository.AddEmployerAsync(employer);

        return new CommonDto<object>
        {
            StatusCode = 201,
            Message = "Employer registered successfully."
        };
    }

    public async Task<CommonDto<object>> RegisterCandidate(RegisterCandidateDto registerCandidateDto)
    {
        if (registerCandidateDto == null)
        {
            return new CommonDto<object>
            {
                StatusCode = 400,
                Message = "Invalid registration data."
            };
        }

        if (await _userRepository.IsEmailExists(registerCandidateDto.Email))
        {
            return new CommonDto<object>
            {
                StatusCode = 400,
                Message = "Email already exists."
            };
        }

        var hashedPassword = PasswordHasher.Hash(registerCandidateDto.Password);
        var user = new User
        {
            Email = registerCandidateDto.Email,
            Password = hashedPassword,
            RoleId = _roleRepository.GetRoleIdByName("Candidate")
        };
        var savedUser = await _userRepository.AddUserAsync(user);

        var candidate = new Candidate
        {
            Name = registerCandidateDto.Name,
            UserId = savedUser.Id
        };
        await _candidateRepository.AddCandidateAsync(candidate);

        return new CommonDto<object>
        {
            StatusCode = 201,
            Message = "Candidate registered successfully."
        };
    }

    public async Task<CommonDto<string>> Login(LoginDTO loginDto)
    {
        if (loginDto == null)
        {
            return new CommonDto<string>
            {
                StatusCode = 400,
                Message = "Invalid Login data."
            };
        }

        if (!await _userRepository.IsEmailExists(loginDto.Email))
        {
            return new CommonDto<string>
            {
                StatusCode = 400,
                Message = "Email does not exist."
            };
        }

        var user = _userRepository.GetUserByEmail(loginDto.Email);

        if (!PasswordHasher.Verify(user.Password, loginDto.Password))
        {
            return new CommonDto<string>
            {
                StatusCode = 401,
                Message = "Invalid password"
            };
        }

        var rolename = _roleRepository.GetRoleById(user.RoleId).Name;
        var token = _jwtHelper.GenerateToken(user.Email, rolename);

        return new CommonDto<string>
        {
            StatusCode = 200,
            Message = "login successfully.",
            Data = token
        };

    }

}