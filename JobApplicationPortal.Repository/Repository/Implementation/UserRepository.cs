using JobApplicationPortal.DataModels.Models;
using JobApplicationPortal.Repository.Repository.Interface;

namespace JobApplicationPortal.Repository.Repository.Implementation;

public class UserRepository : IUserRepository
{
    private readonly JobApplicationPortalContext _context;
    public UserRepository(JobApplicationPortalContext context)
    {
        _context = context;
    }

    public async Task<bool> IsEmailExists(string email)
    {
       return _context.Users.Any(u => u.Email == email);
    }

    public async Task AddUserAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

    }

    public User GetUserByEmail(string email)
    {
        return _context.Users.FirstOrDefault(u => u.Email == email);
    }
    
}
