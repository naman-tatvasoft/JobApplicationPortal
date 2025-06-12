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

}
