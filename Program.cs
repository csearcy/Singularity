using Singularity.Models;
using Singularity.Services;
using Singularity.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.Configure<BlizzardApiOptions>(
    builder.Configuration.GetSection("BlizzardAPI")
);

builder.Services.AddHttpClient<IBlizzardDataService, BlizzardDataService>();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IBlizzardDataService, BlizzardDataService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var blizzardDataService = scope.ServiceProvider.GetRequiredService<IBlizzardDataService>();    
    await blizzardDataService.GetRosterDataAsync();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();

