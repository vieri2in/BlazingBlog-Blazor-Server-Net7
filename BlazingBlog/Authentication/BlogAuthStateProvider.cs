using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BlazingBlog.Authentication
{
    public class BlogAuthStateProvider : AuthenticationStateProvider, IDisposable
    {
        private const string BlogAuthType = "blog-auth";
        private readonly AuthService _authService;
        public BlogAuthStateProvider(AuthService authService)
        {
            this._authService = authService;
            AuthenticationStateChanged += BlogAuthStateProvider_AuthenticationStateChanged;
        }

        private async void BlogAuthStateProvider_AuthenticationStateChanged(Task<AuthenticationState> task)
        {
            var authState = await task;
            if (authState is not null)
            {
                var userId = Convert.ToInt32(authState.User.FindFirstValue(ClaimTypes.NameIdentifier));
                var displayName = authState.User.FindFirstValue(ClaimTypes.Name);
                LoggedinUser = new LoggedinUser(userId, displayName!);
            }
        }

        public LoggedinUser LoggedinUser { get; private set; } = new LoggedinUser()
        {
            UserId = 0,
            DisplayName = ""
        };
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var clainPrincipal = new ClaimsPrincipal();
            var user = await _authService.GetUserFromBrowerStorageAsync();
            if (user is not null)
            {
                clainPrincipal = GetClaimsPrincipalFromUser(user.Value);
            }
            var authState = new AuthenticationState(clainPrincipal);
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
            return authState;
        }
        public async Task<string?> LoginAsync(LoginModel loginModel)
        {
            var loggedInUser = await _authService.LoginUserAsync(loginModel);
            if (loggedInUser is not null)
            {
                var authState = new AuthenticationState(GetClaimsPrincipalFromUser(loggedInUser.Value));
                NotifyAuthenticationStateChanged(Task.FromResult(authState));
                return null;
            }
            else
            {
                return "Invalid credentials";
            }
        }
        private static ClaimsPrincipal GetClaimsPrincipalFromUser(LoggedinUser user)
        {
            var identity = new ClaimsIdentity(new[]
               {
                    new Claim(ClaimTypes.NameIdentifier,user.UserId.ToString()),
                     new Claim(ClaimTypes.Name,user.DisplayName)
                }, BlogAuthType);
            return new ClaimsPrincipal(identity);
        }
        public async Task LogoutAsync()
        {
            await _authService.RemoveUserFromBrowerStorageAsync();
            var authState = new AuthenticationState(new ClaimsPrincipal());
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
        }
        public void Dispose()
        {
            AuthenticationStateChanged -= BlogAuthStateProvider_AuthenticationStateChanged;
        }
    }
}
