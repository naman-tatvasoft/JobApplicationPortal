using JobApplicationPortal.DataModels.Models;

namespace JobApplicationPortal.Repository.Repository.Interface;

public interface ICategoryRepository
{
  public IQueryable<Category> GetCategories();
  public Category GetCategoryByName(string categoryName);
  public Task<Category> CreateCategory(Category category);
  public string GetCategoryNameById(int categoryId);
  public Task<Category> UpdateCategory(Category category);
  public Task DeleteCategory(int categoryId);

}
