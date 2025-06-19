namespace JobApplicationPortal.DataModels.Dtos.ResponseDtos;

public class JobInfoDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Location { get; set; } = null!;
    public int? ExperienceRequired { get; set; }
    public bool IsActive { get; set; }
    public DateOnly OpenFrom { get; set; }
    public long Vacancies { get; set; }
    public List<SkillDto> skillsRequiredList { get; set; } = new List<SkillDto>();
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }

}
