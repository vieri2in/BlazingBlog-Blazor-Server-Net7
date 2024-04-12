using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Text.Json;

namespace BlazingBlog.Authentication
{
    public class AuthService
    {
        private readonly UserService _userService;
        private readonly ProtectedLocalStorage _protectedLocalStorage;
        public AuthService(UserService userService, ProtectedLocalStorage protectedLocalStorage)
        {
            this._userService = userService;
            this._protectedLocalStorage = protectedLocalStorage;
        }
        public async Task<LoggedinUser?> LoginUserAsync(LoginModel loginModel)
        {
            var loggedInUser = await _userService.LoginAsync(loginModel);
            if (loggedInUser is not null) {
                await SaveUserToBrowerStorageAsync(loggedInUser.Value);
            }
            return loggedInUser;
        }
        private const string UserStorageKey = "blog_user";
        private JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions() { };
        public async Task SaveUserToBrowerStorageAsync(LoggedinUser loggedinUser) => await _protectedLocalStorage.SetAsync(UserStorageKey, JsonSerializer.Serialize(loggedinUser, _jsonSerializerOptions));
        public async Task<LoggedinUser?> GetUserFromBrowerStorageAsync()
        {
            try
            {
                var result = await _protectedLocalStorage.GetAsync<string>(UserStorageKey);
                if (result.Success && !string.IsNullOrEmpty(result.Value))
                {
                    var loggedInUser = JsonSerializer.Deserialize<LoggedinUser>(result.Value, _jsonSerializerOptions);
                    return loggedInUser;
                }
            } catch (Exception e)
            {
                // if this method is called other than the cliend side, it will throw exception since server has no such storage
            }
            return null;
        }
        public async Task RemoveUserFromBrowerStorageAsync()
        {
            await _protectedLocalStorage.DeleteAsync(UserStorageKey);
        }

    }
}