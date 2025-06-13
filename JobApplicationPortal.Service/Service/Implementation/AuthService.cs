using JobApplicationPortal.DataModels.Dtos.RequestDtos;
using JobApplicationPortal.DataModels.Dtos.ResponseDtos;
using JobApplicationPortal.DataModels.Models;
using JobApplicationPortal.Repository.Repository.Interface;
using JobApplicationPortal.Service.Exceptions;
using JobApplicationPortal.Service.Helper;
using JobApplicationPortal.Service.Service.Interface;

namespace JobApplicationPortal.Service.Service.Implementation;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;

    private readonly JwtHelper _jwtHelper;

    public AuthService(IUserRepository userRepository,
        IRoleRepository roleRepository,
        JwtHelper jwtHelper)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _jwtHelper = jwtHelper;
    }

    public async Task<CommonDto<object>> RegisterEmployer(RegisterEmployerDto registerEmployerDto)
    {
        if (registerEmployerDto == null)
        {
            throw new RegistrationException();
        }

        if (await _userRepository.IsEmailExists(registerEmployerDto.Email))
        {
            throw new EmailAlreadyExistException();
        }

        var hashedPassword = PasswordHasher.Hash(registerEmployerDto.Password);
        var user = new User
        {
            Email = registerEmployerDto.Email,
            Password = hashedPassword,
            RoleId = _roleRepository.GetRoleIdByName("Employer"),
            Employer = new Employer
            {
                Name = registerEmployerDto.Name,
                CompanyName = registerEmployerDto.CompanyName
            }
        };
        await _userRepository.AddUserAsync(user);

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
            throw new RegistrationException();
        }

        if (await _userRepository.IsEmailExists(registerCandidateDto.Email))
        {
            throw new EmailAlreadyExistException();
        }

        var hashedPassword = PasswordHasher.Hash(registerCandidateDto.Password);
        var user = new User
        {
            Email = registerCandidateDto.Email,
            Password = hashedPassword,
            RoleId = _roleRepository.GetRoleIdByName("Candidate"),
            Candidate = new Candidate
            {
                Name = registerCandidateDto.Name,
            }
        };
        await _userRepository.AddUserAsync(user);

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
            throw new LoginException();
        }

        if (!await _userRepository.IsEmailExists(loginDto.Email))
        {
            throw new EmailDoesNotExistException();
        }

        var user = _userRepository.GetUserByEmail(loginDto.Email);

        if (!PasswordHasher.Verify(user.Password, loginDto.Password))
        {
           throw new InvalidPasswordException();
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