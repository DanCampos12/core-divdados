namespace Core.Divdados.Api;

public class SettingsModel
{
    public string Environment { get; set; }
    public string ConnectionString { get; set; }
    public string ClientId { get; set; }
    public string ClientURL { get; set; }
    public JwtBearer JwtBearer { get; set; }
    public SendGrid SendGrid { get; set; }
}

public class JwtBearer
{
    public string ValidIssuer { get; set; }
    public string ValidAudience { get; set; }
    public string SecretKey { get; set; }
}

public class SendGrid
{
    public string Password { get; set; }
    public string ApiKey { get; set; }
}