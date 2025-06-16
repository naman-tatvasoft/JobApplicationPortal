using JobApplicationPortal.DataModels.Models;

namespace JobApplicationPortal.Repository.Repository.Interface;

public interface IUserRepository
{
    public Task<bool> IsEmailExists(string email);
    public Task AddUserAsync(User user);
    public User GetUserByEmail(string email);
    public Task<User> UpdateUserAsync(User user);

}
