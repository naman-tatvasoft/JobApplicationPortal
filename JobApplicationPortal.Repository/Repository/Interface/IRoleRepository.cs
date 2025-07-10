using JobApplicationPortal.DataModels.Models;

namespace JobApplicationPortal.Repository.Repository.Interface;

public interface IRoleRepository
{
    public int GetRoleIdByName(string roleName);
    public Role GetRoleById(int roleId);
    public List<Role> GetRoles();
}
