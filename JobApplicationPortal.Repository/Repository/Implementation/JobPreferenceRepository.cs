using JobApplicationPortal.DataModels.Models;
using JobApplicationPortal.Repository.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationPortal.Repository.Repository.Implementation;

public class JobPreferenceRepository : IJobPreferenceRepository
{
    private readonly JobApplicationPortalContext _context;
    public JobPreferenceRepository(JobApplicationPortalContext context)
    {
        _context = context;
    }

    
    public JobPreference GetJobPreferenceById(int Id)
    {
        var jobPreference = _context.JobPreferences.FirstOrDefault(jp => jp.Id == Id);
        if (jobPreference == null)
        {
            return null;
        }
        return jobPreference;
    }
    
    public async Task<JobPreference> CreateJobPreference(JobPreference jobPreference)
    {
        _context.JobPreferences.Add(jobPreference);
        await _context.SaveChangesAsync();
        return jobPreference;
    }

    public async Task<JobPreference> UpdateJobPreference(JobPreference jobPreference)
    {
        var existingJobPreference = await _context.JobPreferences
            .FirstOrDefaultAsync(jp => jp.Id == jobPreference.Id);
        
        existingJobPreference.CategoryId = jobPreference.CategoryId;
        existingJobPreference.Location = jobPreference.Location;
        existingJobPreference.ExperienceRequired = jobPreference.ExperienceRequired;

        _context.JobPreferences.Update(existingJobPreference);
        await _context.SaveChangesAsync();

        return jobPreference;
    }

    public async Task DeleteJobPreference(int jobPreferenceId)
    {
        var jobPreference = await _context.JobPreferences.FindAsync(jobPreferenceId);
        if (jobPreference != null)
        {
            _context.JobPreferences.Remove(jobPreference);
            await _context.SaveChangesAsync();
        }
    }

    public List<Candidate> GetCandidatesMatchingPreference(Job job)
    {
        return _context.JobPreferences
            .Include(jp => jp.Candidate)
            .ThenInclude(jp => jp.User)
            .Where(jp => jp.CategoryId == job.CategoryId && 
                         jp.Location == job.Location && 
                         jp.ExperienceRequired <= job.ExperienceRequired)
            .Select(jp => jp.Candidate)
            .ToList();
    }
}
