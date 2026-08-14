using SqlServerChallenges.Core;
using SqlServerChallenges.Core.Authentication;
using SqlServerChallenges.Core.Data;
using SqlServerChallenges.Web.Authentication;
using SqlServerChallenges.Web.Common;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddApplicationCore(builder.Configuration);

builder.Services.AddScoped<IUserContext, UserContext>();
builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

var app = builder.Build();
    
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

await app.Services.SeedAsync();
app.UseExceptionHandler("/Error");
app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
    .WithStaticAssets();

app.Run();