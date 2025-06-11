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

    public async Task DeleteJobSkillByJobId(int jobId)
    {
        var jobSkills = _context.JobSkills.Where(js => js.JobId == jobId).ToList();
        if (jobSkills.Any())
        {
            _context.JobSkills.RemoveRange(jobSkills);
            await _context.SaveChangesAsync();
        }
    }
}
