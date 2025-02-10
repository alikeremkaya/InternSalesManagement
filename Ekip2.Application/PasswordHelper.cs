﻿namespace Ekip2.Application
{
    public class PasswordHelper

    {
        public static string GenerateRandomPassword(int length = 12)
        {
            const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@$?_-";
            Random random = new Random();
            return new string(Enumerable.Repeat(validChars, length)
                                        .Select(s => s[random.Next(s.Length)])
                                        .ToArray());
        }
    }
}
