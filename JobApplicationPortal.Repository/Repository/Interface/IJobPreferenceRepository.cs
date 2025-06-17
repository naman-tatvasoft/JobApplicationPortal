using JobApplicationPortal.DataModels.Models;

namespace JobApplicationPortal.Repository.Repository.Interface;

public interface IJobPreferenceRepository
{
    public JobPreference GetJobPreferenceById(int Id);
    public Task<JobPreference> CreateJobPreference(JobPreference jobPreference);
    public Task<JobPreference> UpdateJobPreference(JobPreference jobPreference);
    public Task DeleteJobPreference(int jobPreferenceId);
}
