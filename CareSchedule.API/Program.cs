using CareSchedule.Repositories.Implementation;
using CareSchedule.Repositories.Interface;
using CareSchedule.Services;
using CareSchedule.Services.Interface;
using CareSchedule.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<CareScheduleContext>();
builder.Services.AddScoped<ISiteRepository, SiteRepository>();
builder.Services.AddScoped<ISiteService, SiteService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
