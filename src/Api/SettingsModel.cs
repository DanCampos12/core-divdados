namespace Core.Divdados.Api;

public class SettingsModel
{
    public string Environment { get; set; }
    public string ConnectionString { get; set; }
    public string ClientId { get; set; }
    public JwtBearer JwtBearer { get; set; }
    public AWS AWS { get; set; }
}

public class JwtBearer
{
    public string ValidIssuer { get; set; }
    public string ValidAudience { get; set; }
    public string SecretKey { get; set; }
}

public class AWS
{
    public string AccessKeyId { get; set; }
    public string SecretAccessKeyId { get; set; }
}