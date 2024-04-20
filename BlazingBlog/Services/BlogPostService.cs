using Microsoft.EntityFrameworkCore;

namespace BlazingBlog.Services
{
    public class BlogPostService
    {
        private readonly BlogContext _blogContext;
        public BlogPostService(BlogContext blogContext)
        {
            this._blogContext = blogContext;
        }
        public async Task<IEnumerable<BlogPost>> GetBlogPostsAsync(bool publishedOnly = false, string? categorySlug = null) {
            var query =  _blogContext.BlogPosts
            .Include(post => post.Category).AsNoTracking();
            if (!string.IsNullOrWhiteSpace(categorySlug))
            {
                var categoryId = await _blogContext.Categories.AsNoTracking().Where(c => c.Slug == categorySlug).Select(c => c.Id).FirstOrDefaultAsync();
                if(categoryId > 0)
                {
                    query = query.Where(bp => bp.CategoryId == categoryId );
                }
            }
            if (publishedOnly) {
                query = query.Where(bp => bp.IsPublished);
            }
            return await query.ToListAsync();
         
        }
        public async Task<BlogPostSaveModel?> GetBlogPostAsync(int postId) =>
             await _blogContext.BlogPosts
            .AsNoTracking()
            .Include(post => post.Category)
            .Select(BlogPostSaveModel.Selector)
            .FirstOrDefaultAsync(post => post.Id == postId);
        public async Task<MethodResult> SaveBlogPostAsync(BlogPostSaveModel post, int userId)
        {
            if (post.Id == 0)
            {
                // create
                var entity = post.ToBlogPostEntity(userId);
                entity.Slug = entity.Slug.Slugify();
                entity.CreatedOn = DateTime.Now;
                if (entity.IsPublished)
                {
                    entity.PublishedOn = DateTime.Now;
                }
                await _blogContext.AddAsync(entity);
            }
            else
            {
                //update
                BlogPost? entity = await _blogContext.BlogPosts
                    .FirstOrDefaultAsync(bp => bp.Id == post.Id);
                if (entity is not null)
                {
                    var wasPublished = entity.IsPublished;
                    entity = post.Merge(entity);
                    entity.ModifiedOn = DateTime.Now;
                    if (entity.IsPublished)
                    {
                        if (wasPublished)
                        {
                            // do nothing
                        }
                        else
                        {
                            entity.PublishedOn = DateTime.Now;
                        }
                    } else if (wasPublished)
                    {
                        entity.PublishedOn = null;
                    }
                }
                else
                {
                    return MethodResult.Failure("The blog post does not exist");
                }

            }
            try
            {
                if (await _blogContext.SaveChangesAsync() > 0)
                {
                    return MethodResult.Success();
                }
                else
                {
                    return MethodResult.Failure("Unknow erro occured while saving blog post");
                }
            } catch (Exception ex)
            {
                return MethodResult.Failure(ex.Message);
                //throw;
            }
        }
    }
}
