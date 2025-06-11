using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace JobApplicationPortal.DataModels.Dtos.RequestDtos;

public class ApplicationDto
{
    [Required]
    public int JobId {get; set;}

    [Required]
    [Range(0, 40, ErrorMessage = "Experience required must be between 0 and 40 years")]
    public int Experience {get; set;}
    public string? NoteForEmployer {get; set;}
    
    [Required]
    public IFormFile CoverLetter {get; set;}
    
    [Required]
    public IFormFile Resume {get; set;}
}
