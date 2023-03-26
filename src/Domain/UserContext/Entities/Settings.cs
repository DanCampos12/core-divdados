﻿namespace Core.Divdados.Domain.UserContext.Entities;

public class Settings
{
    public string Environment { get; set; }
    public string ConnectionString { get; set; }
    public JwtBearer JwtBearer { get; set; }
}

public class JwtBearer
{
    public string ValidIssuer { get; set; }
    public string ValidAudience { get; set; }
    public string SecretKey { get; set; }
}