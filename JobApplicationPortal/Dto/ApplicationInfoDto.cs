namespace JobApplicationPortal.Dto;

public class ApplicationInfoDto
{
    public int Id { get; set; }
    public string JobTitle { get; set; }
    public string CompanyName { get; set; }
    public string jobLocation {get; set;}
    public int CandidateId { get; set; }
    public string CandidateName { get; set; }
    public string CandidateEmail { get; set; }
    public int Experience { get; set; }
    public string NoteForEmployer { get; set; }
    public string Status {get; set;}
    public DateTime ApplicationDate { get; set; }
}
