namespace HomeOwners.Infrastructure.Database;

public class DatabaseSettings
{
    public string? ConnectionString { get; set; }
    public string MigrationTable { get; set; } = "changelog";
    public bool EnableMigrations { get; set; }
}
