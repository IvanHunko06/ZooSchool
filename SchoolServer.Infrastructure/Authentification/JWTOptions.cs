﻿namespace SchoolServer.Infrastructure.Authentification;

public class JWTOptions
{
    public string SecretKey { get; set; } = string.Empty;
    public int ExpiresHours { get; set; }
    public string JWTCookieName { get; set; } = string.Empty;
    public string UsernameClaim { get; set;} = string.Empty;
}
