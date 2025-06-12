using JobApplicationPortal.DataModels.Models;

namespace JobApplicationPortal.Repository.Repository.Interface;

public interface IStatusRepository
{
    public IQueryable<Status> GetStatuses();
    public int GetStatusIdByName(string statusName);
    public string GetStatusNameById(int statusId);

}
