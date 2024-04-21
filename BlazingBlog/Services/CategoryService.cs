using Microsoft.EntityFrameworkCore;

namespace BlazingBlog.Services
{
    public class CategoryService
    {
        private readonly BlogContext _blogContext;
        public CategoryService(BlogContext blogContext)
        {
            this._blogContext = blogContext;
        }
        public async Task<IEnumerable<Category>> GetCategoriesAsync() =>
            await _blogContext.Categories.AsNoTracking().ToListAsync();
        public async Task<string?> GetCategoryNameBySlugAsync(string categorySlug) =>
           (await GetCategoriesAsync()).Where(c => c.Slug == categorySlug).Select(c => c.Name).FirstOrDefault();
        public async Task<MethodResult> SaveCategoryAsync(Category model)
        {
            try
            {
                if (model.Id > 0)
                {
                    _blogContext.Categories.Update(model);
                }
                else
                {
                    model.Slug = model.Slug.Slugify();
                    await _blogContext.Categories.AddAsync(model);
                }
                await _blogContext.SaveChangesAsync();
                return MethodResult.Success();
            } catch (Exception ex)
            {
                return MethodResult.Failure(ex.Message);
            }
        }
    }
}
