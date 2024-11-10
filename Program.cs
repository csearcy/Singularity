using Refit;
using Singularity.Models;
using Singularity.Services;
using Singularity.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.Configure<BlizzardApiOptions>(
    builder.Configuration.GetSection("BlizzardAPI")
);

builder.Services.AddRefitClient<IBlizzardApi>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://us.api.blizzard.com"));

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
    await blizzardDataService.GetRosterDataAsync();
    await blizzardDataService.GetMythicKeystoneSeasonsIndexDataAsync();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
