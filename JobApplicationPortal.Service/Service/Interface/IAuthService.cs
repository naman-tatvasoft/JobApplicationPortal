using JobApplicationPortal.DataModels.Dtos.RequestDtos;
using JobApplicationPortal.DataModels.Dtos.ResponseDtos;

namespace JobApplicationPortal.Service.Service.Interface;

public interface IAuthService
{
    public Task<CommonDto<object>> RegisterEmployer(RegisterEmployerDto registerEmployerDto);
    public Task<CommonDto<object>> RegisterCandidate(RegisterCandidateDto registerCandidateDto);
    public Task<CommonDto<string>> Login(LoginDTO loginDto);
}
