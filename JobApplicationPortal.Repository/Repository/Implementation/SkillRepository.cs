using JobApplicationPortal.DataModels.Models;
using JobApplicationPortal.Repository.Repository.Interface;

namespace JobApplicationPortal.Repository.Repository.Implementation;

public class SkillRepository : ISkillRepository
{
    private readonly JobApplicationPortalContext _context;

    public SkillRepository(JobApplicationPortalContext context)
    {
        _context = context;
    }

    public IQueryable<Skill> GetSkills()
    {
        return _context.Skills.AsQueryable();
    }

}
