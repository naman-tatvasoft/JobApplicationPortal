using JobApplicationPortal.DataModels.Dtos.RequestDtos;
using JobApplicationPortal.DataModels.Dtos.ResponseDtos;

namespace JobApplicationPortal.Service.Service.Interface;

public interface IUserService
{
    public CommonDto<List<EmployerInfoDto>> GetEmployers();
    public CommonDto<List<CandidateInfoDto>> GetCandidates();
    public CommonDto<UserProfileDto> GetProfile();
    public Task<CommonDto<UserProfileDto>> UpdateEmployerProfile(UpdateEmployerProfileDto updateEmployerProfileDto);
    public Task<CommonDto<UserProfileDto>> UpdateCandidateProfile(UpdateCandidateProfileDto updateCandidateProfileDto);
    public CommonDto<List<RolesDto>> GetRoles();
    public CommonDto<List<UserInfoDto>> GetUsers(string search, int pageNumber, int pageSize, string role);

}
