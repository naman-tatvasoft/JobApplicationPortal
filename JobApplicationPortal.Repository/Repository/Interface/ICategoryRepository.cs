using JobApplicationPortal.DataModels.Models;

namespace JobApplicationPortal.Repository.Repository.Interface;

public interface ICategoryRepository
{
  public IQueryable<Category> GetCategories();

}
