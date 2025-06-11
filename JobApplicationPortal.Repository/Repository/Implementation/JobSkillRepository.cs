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
}
