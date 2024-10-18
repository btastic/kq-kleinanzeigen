using KQ.Kleinanzeigen.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using KQ.Kleinanzeigen.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<AppDbContext>(
    options =>
    {
        options.UseLazyLoadingProxies();
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            b => b.MigrationsAssembly("KQ.Kleinanzeigen.Infrastructure"));
    });

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using IServiceScope scope = app.Services.CreateScope();
IServiceProvider services = scope.ServiceProvider;
AppDbContext appDbContext = services.GetRequiredService<AppDbContext>();

appDbContext.EnsureMigrationsApplied();

app.Run();
