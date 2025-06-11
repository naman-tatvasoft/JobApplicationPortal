using JobApplicationPortal.DataModels.Models;
using JobApplicationPortal.Repository.Repository.Interface;

namespace JobApplicationPortal.Repository.Repository.Implementation;

public class JobSkillRepository : IJobSkillRepository
{
    private readonly JobApplicationPortalContext _context;
    public JobSkillRepository(JobApplicationPortalContext context)
    {
        _context = context;
    }

    public async Task CreateJobSkill(JobSkill jobSkill)
    {
        _context.JobSkills.Add(jobSkill);
        await _context.SaveChangesAsync();
    }
}
