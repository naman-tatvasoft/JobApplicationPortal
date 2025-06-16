using System.ComponentModel.DataAnnotations;
using JobApplicationPortal.DataModels.Models;
using JobApplicationPortal.DataModels.Dtos.ResponseDtos;

namespace JobApplicationPortal.DataModels.Dtos.RequestDtos;

public class JobDto
{
    public int Id {get; set;}
    
    [MaxLength(50, ErrorMessage = "Title cannot exceed 50 characters")]
    public string Title { get; set; } = null!;

    [MaxLength(255, ErrorMessage = "Description cannot exceed 255 characters")]
    public string Description { get; set; } = null!;

    public string Location { get; set; } = null!;

    [Range(0, 40, ErrorMessage = "Experience required must be between 0 and 40 years")]
    public int? ExperienceRequired { get; set; }

    public bool IsActive {get; set;}
    public DateOnly OpenFrom { get; set; }
    public long Vacancies {get; set;}
    public List<SkillDto> skillsRequiredList { get; set; } = new List<SkillDto>();

    public int CategoryId {get; set;}

    public string CategoryName {get; set;}

}
