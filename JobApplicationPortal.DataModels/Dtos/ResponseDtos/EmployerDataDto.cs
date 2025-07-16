namespace JobApplicationPortal.DataModels.Dtos.ResponseDtos;

public class EmployerDataDto
{
    public int TotalJobs { get; set; }
    public int TotalApplications { get; set; }
    public int NewApplications { get; set; }
    public List<JobInfoDto> LatestJobs { get; set; } = new List<JobInfoDto>();
    public List<ApplicationInfoDto> LatestApplications { get; set; } = new List<ApplicationInfoDto>();
    }
