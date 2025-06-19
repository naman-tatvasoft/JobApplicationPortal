namespace JobApplicationPortal.DataModels.Dtos.RequestDtos;

public class JobDto
{
    public int Id {get; set;}
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Location { get; set; } = null!;
    public int? ExperienceRequired { get; set; }
    public bool IsActive {get; set;}
    public DateOnly OpenFrom { get; set; }
    public long Vacancies {get; set;}
    public List<SkillRequestDto> skillsRequiredList { get; set; } = new List<SkillRequestDto>();
    public int CategoryId {get; set;}
}
