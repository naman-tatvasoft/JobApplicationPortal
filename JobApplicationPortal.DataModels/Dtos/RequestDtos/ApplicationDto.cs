using Microsoft.AspNetCore.Http;

namespace JobApplicationPortal.DataModels.Dtos.RequestDtos;

public class ApplicationDto
{
    public int JobId { get; set; }
    public int Experience { get; set; }
    public string? NoteForEmployer { get; set; }
    public IFormFile CoverLetter { get; set; }
    public IFormFile Resume { get; set; }
}
