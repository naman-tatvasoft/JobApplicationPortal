using JobApplicationPortal.DataModels.Dtos.ResponseDtos;
using JobApplicationPortal.Repository.Repository.Interface;
using JobApplicationPortal.Service.Service.Interface;

namespace JobApplicationPortal.Service.Service.Implementation;

public class UserService : IUserService
{
    private readonly IEmployerRepository _employerRepository;
    private readonly ICandidateRepository _candidateRepository;

    public UserService(IEmployerRepository employerRepository, 
        ICandidateRepository candidateRepository)
    {
        _employerRepository = employerRepository;
        _candidateRepository = candidateRepository;
    }

      public CommonDto<List<EmployerInfoDto>> GetEmployers(){ 
        
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

      public CommonDto<List<CandidateInfoDto>> GetCandidates(){
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
}
