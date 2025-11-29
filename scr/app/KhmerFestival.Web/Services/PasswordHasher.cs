using System;
using System.Security.Cryptography;
using System.Text;

namespace Thesis.Web.Services
{
    public class PasswordHasher
    {
        /// <summary>
 /// Hash password with salt using HMACSHA512
        /// </summary>
     public static (byte[] hash, byte[] salt) HashPassword(string password)
  {
      if (string.IsNullOrWhiteSpace(password))
         throw new ArgumentException("Password cannot be empty", nameof(password));

            using (var hmac = new HMACSHA512())
            {
        var salt = hmac.Key;
      var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return (hash, salt);
      }
        }

        /// <summary>
        /// Verify password against stored hash and salt
        /// </summary>
        public static bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt)
    {
  if (string.IsNullOrWhiteSpace(password))
       throw new ArgumentException("Password cannot be empty", nameof(password));

        if (storedHash == null || storedSalt == null)
         return false;

       using (var hmac = new HMACSHA512(storedSalt))
            {
             var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

      // Compare hash - should not short circuit for security
       for (int i = 0; i < computedHash.Length; i++)
   {
           if (computedHash[i] != storedHash[i])
             return false;
 }
      }

      return true;
        }
    }
}
