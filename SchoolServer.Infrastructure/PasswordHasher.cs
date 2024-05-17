﻿using SchoolServer.Application.Interfaces.Auth;
namespace SchoolServer.Infrastructure
{
    public class PasswordHasher : IPasswordHasher
    {
        public string Generate(string password) =>
            BCrypt.Net.BCrypt.EnhancedHashPassword(password);

        public bool Verify(string password, string hashedPassword) => 
            BCrypt.Net.BCrypt.EnhancedVerify(password, hashedPassword);
    }
}
