using JobApplicationPortal.DataModels.Dtos.ResponseDtos;

namespace JobApplicationPortal.Service.Service.Interface;

public interface IDashboardService
{
    public CommonDto<AdminDataDto> AdminData();
    public CommonDto<EmployerDataDto> EmployerData(int id);

}
