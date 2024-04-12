using BlazingBlog.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddTransient<UserService>();
builder.Services.AddTransient<CategoryService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<BlogAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(serviceProvider => serviceProvider.GetRequiredService<BlogAuthStateProvider>());
builder.Services.AddDbContext<BlogContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Blog"));
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
