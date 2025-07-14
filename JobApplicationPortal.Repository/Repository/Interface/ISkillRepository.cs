using JobApplicationPortal.DataModels.Models;

namespace JobApplicationPortal.Repository.Repository.Interface;

public interface ISkillRepository
{
    public IQueryable<Skill> GetSkills();
    public Skill GetSkillByName(string skillName);
    public Task<Skill> CreateSkill(Skill skill);
    public string GetSkillNameById(int skillId);
    public Task<Skill> UpdateSkill(Skill skill);
    public Task DeleteSkill(int skillId);

}
