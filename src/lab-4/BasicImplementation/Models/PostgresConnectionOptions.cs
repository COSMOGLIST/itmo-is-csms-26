namespace BasicImplementation.Models;

public class PostgresConnectionOptions
{
    public string Host { get; set; } = string.Empty;

    public string Port { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string ToConnectionString()
    {
        return $"Host={Host};Username={Username};Password={Password};Port={Port};";
    }
}