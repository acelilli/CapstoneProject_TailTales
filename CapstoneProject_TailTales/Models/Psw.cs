using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace CapstoneProject_TailTales.Models
{
    public class Psw
    {
        // Metodo per fare Hash (criptare) la password
        // 1. Calcola l'hash della password
        // 2. Converte l'array di byte in una stringa esadecimale
        // Restituisce la stringa passata al builder
        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashedBytes.Length; i++)
                {
                    builder.Append(hashedBytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}