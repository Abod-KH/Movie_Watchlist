using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Movie_Watchlist.Infrastructure.Workers;


var builder = WebApplication.CreateBuilder(args);

 
builder.Services.AddDatabaseDeveloperPageExceptionFilter();


builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";      
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
    })
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? "YOUR_GOOGLE_CLIENT_ID";
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? "YOUR_GOOGLE_CLIENT_SECRET";
        options.CallbackPath = "/signin-google";
    });


builder.Services.AddSingleton<SqlConnectionFactory>();

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IHomeRepository, HomeRepository>();
builder.Services.AddScoped<IUserWatchlistRepository, UserWatchlistRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddHttpClient<ITmdbService, TmdbService>();
builder.Services.AddSingleton<IHomepageCacheService, HomepageCacheService>();
builder.Services.AddMemoryCache();

builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IMovieRepository, MovieRepository>();

builder.Services.AddScoped<ITvShowHomeRepository, TvShowHomeRepository>();
builder.Services.AddScoped<ITvShowRepository, TvShowRepository>();
builder.Services.AddScoped<ITvShowWatchlistRepository, TvShowWatchlistRepository>();

builder.Services.AddHostedService<TmdbSyncWorker>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");

    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/Error/{0}");
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();


app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();



app.Run();
