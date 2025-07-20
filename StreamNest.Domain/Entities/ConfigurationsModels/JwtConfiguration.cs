namespace StreamNest.Domain.Entities.ConfigurationsModels
{
    public class JwtConfiguration
    {
    public string Section {get; set;} = "JwtSettings";
    public string? Secret {get; set;} = Environment.GetEnvironmentVariable("JWT_SECRET");
    public string? ValidIssuer {get; set;}
    public string? ValidAudience {get; set;}
    public string? Expires {get; set;}
    }
}