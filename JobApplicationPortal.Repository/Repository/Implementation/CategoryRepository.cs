using JobApplicationPortal.DataModels.Models;
using JobApplicationPortal.Repository.Repository.Interface;

namespace JobApplicationPortal.Repository.Repository.Implementation;

public class CategoryRepository : ICategoryRepository
{
    private readonly JobApplicationPortalContext _context;
    public CategoryRepository(JobApplicationPortalContext context)
    {
        _context = context;
    }

    public IQueryable<Category> GetCategories()
    {
        return _context.Categories.AsQueryable();
    }
}
