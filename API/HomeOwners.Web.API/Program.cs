using HomeOwners.Infrastructure.Configuration;
using HomeOwners.Infrastructure.Database;
using HomeOwners.Lib.Configuration.Configuration;
using Microsoft.EntityFrameworkCore;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.UseHomeOwnersConfiguration();

        builder.Services.AddObservability(builder.Configuration);
        builder.Host.UseHomeOwnersLogging();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddRepositories();
        builder.Services.AddSecurityLayer();

        builder.Services.AddDatabase(builder.Configuration);

        //var context = builder.Services.BuildServiceProvider().GetRequiredService<DatabaseContext>();
        //context.Database.Migrate();
        

        builder.Services.AddControllers();
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseApplicationLogging();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}