using Microsoft.Extensions.Options;
using Refit;
using Singularity.Models;
using Singularity.Services;
using Singularity.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(); 
builder.Services.AddRazorPages();

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddUserSecrets<Program>(optional: true)
    .AddEnvironmentVariables(); 

builder.Services.Configure<BlizzardApiOptions>(
    builder.Configuration.GetSection("BlizzardAPI")
);

builder.Services.AddRefitClient<IBlizzardApi>()
    .ConfigureHttpClient((serviceProvider, client) =>
    {
        var options = serviceProvider.GetRequiredService<IOptions<BlizzardApiOptions>>().Value;
        client.BaseAddress = new Uri(options.BaseUrl);
    });

builder.Services.AddHttpClient<IBlizzardDataService, BlizzardDataService>();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IBlizzardDataService, BlizzardDataService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var blizzardDataService = scope.ServiceProvider.GetRequiredService<IBlizzardDataService>();    
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();
