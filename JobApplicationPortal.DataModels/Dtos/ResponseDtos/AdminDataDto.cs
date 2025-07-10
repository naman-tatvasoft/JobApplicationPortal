namespace JobApplicationPortal.DataModels.Dtos.ResponseDtos;

public class AdminDataDto
{
    public int TotalEmployers { get; set; }
    public int TotalJobs { get; set; }
    public int TotalCandidates { get; set; }
    public int TotalApplications { get; set; }
    public List<JobInfoDto> LatestJobs { get; set; } = new List<JobInfoDto>();
    public List<ApplicationInfoDto> LatestApplications { get; set; } = new List<ApplicationInfoDto>();
    public List<UserInfoDto> LatestUsers { get; set; } = new List<UserInfoDto>();
}
