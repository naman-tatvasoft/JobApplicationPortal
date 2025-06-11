using JobApplicationPortal.DataModels.Models;

namespace JobApplicationPortal.Repository.Repository.Interface;

public interface IEmployerRepository
{
    public IQueryable<Employer> GetAllEmployers();
    public bool IsEmployerIdExist(int employerId);
    public Employer GetEmployerByEmail(string email);
}
