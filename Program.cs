using Microsoft.AspNetCore.Authentication.Cookies;
using MongoDB.Driver;
using TeamProjectPlanner.Components;
using TeamProjectPlanner.Data;
using TeamProjectPlanner.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.Configure<MongoDbSettings>(settings =>
{
    builder.Configuration.GetSection(MongoDbSettings.SectionName).Bind(settings);

    if (string.IsNullOrWhiteSpace(settings.ConnectionString))
    {
        settings.ConnectionString = builder.Configuration["MONGODB_URI"] ?? string.Empty;
    }
});

builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
{
    var settings = serviceProvider
        .GetRequiredService<Microsoft.Extensions.Options.IOptions<MongoDbSettings>>()
        .Value;

    if (string.IsNullOrWhiteSpace(settings.ConnectionString))
    {
        throw new InvalidOperationException(
            "MongoDB connection string is not configured. Set MongoDB:ConnectionString or MONGODB_URI.");
    }

    return new MongoClient(settings.ConnectionString);
});

builder.Services.AddSingleton<IMongoDatabase>(serviceProvider =>
{
    var settings = serviceProvider
        .GetRequiredService<Microsoft.Extensions.Options.IOptions<MongoDbSettings>>()
        .Value;

    return serviceProvider.GetRequiredService<IMongoClient>().GetDatabase(settings.DatabaseName);
});

// Add project service
builder.Services.AddSingleton<ProjectService>();
builder.Services.AddSingleton<BoardService>();

// Add task service
builder.Services.AddSingleton<TaskService>();

// Add member service
builder.Services.AddSingleton<ProjectMemberService>();
builder.Services.AddHttpContextAccessor();
// Add authentication services
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

// Add user and auth services
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();