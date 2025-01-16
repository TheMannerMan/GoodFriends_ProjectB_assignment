using Configuration;
using DbContext;
using DbRepos;
using Services;
using Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Configuration.AddApplicationSecrets("../Configuration/Configuration.csproj");

//use multiple Database connections and their respective DbContexts
builder.Services.AddDatabaseConnections(builder.Configuration);
builder.Services.AddDatabaseConnectionsDbContext();
builder.Services.AddIdentityConnectionsDbContext();

//Add IdentityServices to DbContext.MainDbContext
builder.Services.AddDefaultIdentity<User>(options => {
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.SignIn.RequireConfirmedAccount = false;
}).AddEntityFrameworkStores<DbContext.MainDbContext>();

//Inject Custom logger
builder.Services.AddSingleton<ILoggerProvider, InMemoryLoggerProvider>();
builder.Services.AddScoped<FriendsDbRepos>();
builder.Services.AddScoped<IFriendsService, FriendsServiceDb>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
