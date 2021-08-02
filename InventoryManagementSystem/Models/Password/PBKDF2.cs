using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace InventoryManagementSystem.Models.Password
{
    public class PBKDF2
    {
        private const int BYTE_SIZE = 64;

        private const int ITERATION_COUNT = 200000;

        private byte[] _salt;

        private HashAlgorithmName hashAlgorithmName = HashAlgorithmName.SHA512;

        private byte[] _hashedPassword;

        /// <summary>
        /// Get the salt.
        /// </summary>
        public byte[] Salt { get => _salt; }

        /// <summary>
        /// Get the hashed password.
        /// </summary>
        public byte[] HashedPassword { get => _hashedPassword; }

        /// <summary>
        /// Hash a given password with a salt.
        /// </summary>
        /// <param name="password">The password string to be hashed.</param>
        /// <param name="salt">The salt used to hash the password. Leave this null and a random generated salt will be used.</param>
        public PBKDF2(string password, byte[] salt)
        {
            if(salt == null)
            {
                using(Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(password, BYTE_SIZE, ITERATION_COUNT, hashAlgorithmName))
                {
                    _hashedPassword = rfc2898.GetBytes(BYTE_SIZE);
                    _salt = rfc2898.Salt;
                }
            }
            else
            {
                using(Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(password, salt, ITERATION_COUNT, hashAlgorithmName))
                {
                    _hashedPassword = rfc2898.GetBytes(BYTE_SIZE);
                    _salt = salt;
                }
            }

        }

    }
}
