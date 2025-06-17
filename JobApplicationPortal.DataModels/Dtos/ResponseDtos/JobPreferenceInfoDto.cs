namespace JobApplicationPortal.DataModels.Dtos.ResponseDtos;

public class JobPreferenceInfoDto
{
    public int Id {get; set;}
    public int CandidateId { get; set; }

    public int CategoryId { get; set; }

    public int? ExperienceRequired { get; set; }

    public string Location { get; set; } = null!;
}
