using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace fi.retorch.com.Code.Utilities
{
    public class Authentication
    {
        private const int saltSize = 24;

        public static byte[] GeneratePasswordHash(string plainText, byte[] salt)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(plainText);
            //byte[] salt = GeneratePasswordSalt(saltSize);
            return GenerateSaltedHash(byteArray, salt);
        }

        public static bool ValidatePassword(string entered, byte[] validPassword, byte[] salt)
        {
            bool isValid = false;

            byte[] byteEntered = GenerateSaltedHash(Encoding.UTF8.GetBytes(entered), salt);
            isValid = CompareByteArrays(byteEntered, validPassword);

            return isValid;
        }

        internal static byte[] GeneratePasswordSalt(int size = saltSize)
        {
            //Generate a cryptographic random number.
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[size];
            rng.GetBytes(buff);

            return buff;
            // Return a Base64 string representation of the random number.
            //return Convert.ToBase64String(buff);
        }

        private static byte[] GenerateSaltedHash(byte[] plainText, byte[] salt)
        {
            HashAlgorithm algorithm = new SHA256Managed();

            byte[] plainTextWithSaltBytes =
              new byte[plainText.Length + salt.Length];

            for (int i = 0; i < plainText.Length; i++)
            {
                plainTextWithSaltBytes[i] = plainText[i];
            }
            for (int i = 0; i < salt.Length; i++)
            {
                plainTextWithSaltBytes[plainText.Length + i] = salt[i];
            }

            return algorithm.ComputeHash(plainTextWithSaltBytes);
        }

        private static bool CompareByteArrays(byte[] array1, byte[] array2)
        {
            if (array1.Length != array2.Length)
            {
                return false;
            }

            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}