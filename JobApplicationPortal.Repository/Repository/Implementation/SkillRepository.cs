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
        return _context.Skills.OrderBy(s => s.Id).AsQueryable();
    }

    public Skill GetSkillByName(string skillName)
    {
        return _context.Skills.FirstOrDefault(s => s.Name.ToLower() == skillName.ToLower());
    }

    public string GetSkillNameById(int skillId)
    {
        var skill = _context.Skills.FirstOrDefault(s => s.Id == skillId);
        return skill?.Name ?? string.Empty;
    }

    public async Task<Skill> CreateSkill(Skill skill)
    {
        await _context.Skills.AddAsync(skill);
        await _context.SaveChangesAsync();
        return skill;
    }
    public async Task<Skill> UpdateSkill(Skill skill)
    {
        var existingSkill = _context.Skills.FirstOrDefault(s => s.Id == skill.Id);
        existingSkill.Name = skill.Name;
        _context.Skills.Update(existingSkill);
        await _context.SaveChangesAsync();
        return skill;
    }

    public async Task DeleteSkill(int skillId)
    {
        var skill = _context.Skills.FirstOrDefault(s => s.Id == skillId);
        if (skill != null)
        {
            _context.Skills.Remove(skill);
            await _context.SaveChangesAsync();
        }
    }

}
