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

    public Category GetCategoryByName(string categoryName)
    {
        return _context.Categories.FirstOrDefault(c => c.Name.ToLower() == categoryName.ToLower());
    }

    public string GetCategoryNameById(int categoryId)
    {
        var category = _context.Categories.FirstOrDefault(c => c.Id == categoryId);
        return category?.Name ?? string.Empty;
    }

    public async Task<Category> CreateCategory(Category category)
    {
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<Category> UpdateCategory(Category category)
    {
        var existingCategory = _context.Categories.FirstOrDefault(s => s.Id == category.Id);
        existingCategory.Name = category.Name;
        _context.Categories.Update(existingCategory);
        await _context.SaveChangesAsync();
        return category;
    }
    
    public async Task DeleteCategory(int categoryId)
    {
        var category = _context.Categories.FirstOrDefault(c => c.Id == categoryId);
        if (category != null)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }
}
