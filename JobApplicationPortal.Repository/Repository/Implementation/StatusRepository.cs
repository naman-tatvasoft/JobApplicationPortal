using JobApplicationPortal.DataModels.Models;
using JobApplicationPortal.Repository.Repository.Interface;

namespace JobApplicationPortal.Repository.Repository.Implementation;

public class StatusRepository : IStatusRepository
{
    private readonly JobApplicationPortalContext _context;

    public StatusRepository(JobApplicationPortalContext context)
    {
        _context = context;
    }

    public IQueryable<Status> GetStatuses()
    {
        return _context.Statuses.AsQueryable();
    }

    public int GetStatusIdByName(string statusName)
    {
        var status = _context.Statuses.FirstOrDefault(s => s.Name.ToLower() == statusName.ToLower());
        return status?.Id ?? 0; 
    }

    public string GetStatusNameById(int statusId)
    {
        var status = _context.Statuses.FirstOrDefault(s => s.Id == statusId);
        return status?.Name ?? string.Empty; 
    }

    public async Task<Status> CreateStatus(Status status)
    {
        _context.Statuses.Add(status);
        await _context.SaveChangesAsync();
        return status;
    }

    public async Task<Status> UpdateStatus(Status status)
    {
        var existingStatus = _context.Statuses.FirstOrDefault(s => s.Id == status.Id);
        existingStatus.Name = status.Name;
        _context.Statuses.Update(existingStatus);
        await _context.SaveChangesAsync();
        return status;
    }

    public async Task DeleteStatus(int statusId)
    {
        var status = _context.Statuses.FirstOrDefault(s => s.Id == statusId);
        if (status != null)
        {
            _context.Statuses.Remove(status);
            await _context.SaveChangesAsync();
        }
    }
}
