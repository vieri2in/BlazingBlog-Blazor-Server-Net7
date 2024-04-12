namespace BlazingBlog.Models
{
    public record struct LoggedinUser(int UserId, string DisplayName) {
        public readonly bool IsEmpty => UserId == 0;
    };
}
