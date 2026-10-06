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

// Add authentication and authorization services
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/auth/sign-in";
        options.LogoutPath = "/api/auth/logout";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

// Add user and auth services
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AuthService>();

// Add error handling and user feedback services
builder.Services.AddScoped<ToastService>();
builder.Services.AddScoped<UiActionRunner>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

// Show the friendly 404 page for unknown URLs
app.UseStatusCodePagesWithReExecute("/not-found");

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

// Authentication HTTP endpoints
app.MapPost("/api/auth/login", async (
    [Microsoft.AspNetCore.Mvc.FromForm] string email,
    [Microsoft.AspNetCore.Mvc.FromForm] string password,
    [Microsoft.AspNetCore.Mvc.FromForm] string? returnUrl,
    AuthService authService,
    ILogger<Program> logger) =>
{
    bool success;
    string? error;

    try
    {
        (success, error) = await authService.LoginAsync(email, password);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Login failed unexpectedly");
        (success, error) = (false, ErrorMessages.ForUser(ex));
    }

    if (!success)
    {
        var redirectUrl = $"/auth/sign-in?error={Uri.EscapeDataString(error ?? "Invalid email or password.")}";
        return Results.Redirect(redirectUrl);
    }

    return Results.Redirect(string.IsNullOrWhiteSpace(returnUrl) ? "/dashboard" : returnUrl);
}).DisableAntiforgery();

app.MapGet("/api/auth/logout", async (AuthService authService) =>
{
    await authService.LogoutAsync();
    return Results.Redirect("/auth/sign-in");
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();