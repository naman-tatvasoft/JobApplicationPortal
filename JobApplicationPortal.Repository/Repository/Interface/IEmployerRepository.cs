using JobApplicationPortal.DataModels.Models;

namespace JobApplicationPortal.Repository.Repository.Interface;

public interface IEmployerRepository
{
    public Task AddEmployerAsync(Employer employer);
    public IQueryable<Employer> GetAllEmployers();
}
