using JobApplicationPortal.DataModels.Models;

namespace JobApplicationPortal.Repository.Repository.Interface;

public interface IStatusRepository
{
    public IQueryable<Status> GetStatuses();
    public int GetStatusIdByName(string statusName);
    public string GetStatusNameById(int statusId);
    public Task<Status> CreateStatus(Status status);
    public Task<Status> UpdateStatus(Status status);
    public Task DeleteStatus(int statusId);
}
