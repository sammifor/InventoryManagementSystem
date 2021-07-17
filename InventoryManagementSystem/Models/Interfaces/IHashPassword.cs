using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models.Interfaces
{
    interface IHashPassword
    {
        byte[] HashPasswordWithSalt(byte[] password, byte[] salt)
        {
            byte[] result = null;
            using(SHA256 sha256hash = SHA256.Create()){
                byte[] saltAndPassword = salt.Concat(password).ToArray();
                result = sha256hash.ComputeHash(saltAndPassword);
            }
            return result;
        }
    }
}
