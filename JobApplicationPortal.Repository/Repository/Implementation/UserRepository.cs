using JobApplicationPortal.DataModels.Models;
using JobApplicationPortal.Repository.Repository.Interface;
using Microsoft.EntityFrameworkCore;

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
        return _context.Users.Include(u => u.Employer).Include(u => u.Candidate).FirstOrDefault(u => u.Email == email);
    }

    public async Task<User> UpdateUserAsync(User user)
    {
        var existingUser = _context.Users.FirstOrDefault(u => u.Id == user.Id);
        if (existingUser == null)
        {
            throw new Exception("User not found");
        }

        existingUser.Email = user.Email;

        _context.Users.Update(existingUser);
        await _context.SaveChangesAsync();

        return existingUser;
    }

    public int GetTotalCandidates()
    {
        return _context.Users.Count(u => u.Role.Name == "Candidate");
    }

    public int GetTotalEmployers()
    {
        return _context.Users.Count(u => u.Role.Name == "Employer");
    }

    public List<string> GetTotalUsers(){
        return _context.Users.Select(u => u.Role.Name).ToList();
    }

    public List<User> GetUsers()
    {
        return _context.Users.Include(u => u.Role).Include(u => u.Employer).Include(u => u.Candidate).OrderBy(u => u.Id).ToList();
    }
}
