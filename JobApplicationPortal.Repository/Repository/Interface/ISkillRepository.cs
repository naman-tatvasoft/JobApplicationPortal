using JobApplicationPortal.DataModels.Models;

namespace JobApplicationPortal.Repository.Repository.Interface;

public interface ISkillRepository
{
    public IQueryable<Skill> GetSkills();
    public Skill GetSkillByName(string skillName);
}
