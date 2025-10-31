namespace SchoolApp.Security
{
    public static class EncryptionUtil
    {
        public static string Encrypt(string plainText)
        {
            var encryptedPassword = BCrypt.Net.BCrypt.HashPassword(plainText);
            return encryptedPassword;
        }

        public static bool IsValidPassword(string plaiText, string cipherText)
        {
            var isValid = BCrypt.Net.BCrypt.Verify(plaiText, cipherText);
            return isValid;
        }
    }
}
