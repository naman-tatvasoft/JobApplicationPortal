using JobApplicationPortal.DataModels.Models;

namespace JobApplicationPortal.Repository.Repository.Interface;

public interface IJobSkillRepository
{
        public Task CreateJobSkill(JobSkill jobSkill);
        public Task DeleteJobSkillByJobId(int jobId);
}
