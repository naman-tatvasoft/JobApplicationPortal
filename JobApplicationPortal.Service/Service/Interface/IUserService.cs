using JobApplicationPortal.DataModels.Dtos.ResponseDtos;

namespace JobApplicationPortal.Service.Service.Interface;

public interface IUserService
{
    public CommonDto<List<EmployerInfoDto>> GetEmployers();
    public CommonDto<List<CandidateInfoDto>> GetCandidates();
}
