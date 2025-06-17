namespace JobApplicationPortal.DataModels.Dtos.RequestDtos;

public class JobPreferenceDto
{
    public int Id {get; set;}
    public int CategoryId { get; set; }

    public int? ExperienceRequired { get; set; }

    public string Location { get; set; } = null!;
}
