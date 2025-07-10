using JobApplicationPortal.DataModels.Models;
using JobApplicationPortal.Repository.Repository.Interface;

namespace JobApplicationPortal.Repository.Repository.Implementation;

public class RoleRepository : IRoleRepository
{
    private readonly JobApplicationPortalContext _context;

    public RoleRepository(JobApplicationPortalContext context)
    {
        _context = context;
    }

    public int GetRoleIdByName(string roleName)
    {
        var role = _context.Roles.FirstOrDefault(r => r.Name == roleName);
        return role?.Id ?? 0;
    }

    public Role GetRoleById(int roleId)
    {
        return _context.Roles.FirstOrDefault(r => r.Id == roleId);
    }
    
    public List<Role> GetRoles()
    {
        return _context.Roles.ToList();
    }
}
