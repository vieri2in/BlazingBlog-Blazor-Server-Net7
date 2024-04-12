using Microsoft.EntityFrameworkCore;

namespace BlazingBlog.Services
{
    public class UserService
    {
        private readonly BlogContext _blogContext;
        public UserService(BlogContext blogContext)
        {
            this._blogContext = blogContext;
        }
        public async Task<LoggedinUser?> LoginAsync(LoginModel loginModel) {
            var dbUser = await _blogContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == loginModel.Username);
            if (dbUser is not null) {
                // success
                return new LoggedinUser(dbUser.Id, $"{dbUser.FirstName} {dbUser.LastName}".Trim());
            } else {
                // fail
                return null;
            }
        }
    }
}
