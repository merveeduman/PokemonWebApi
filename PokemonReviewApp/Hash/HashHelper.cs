using System.Security.Cryptography;
using System.Text;

namespace PokemonReviewApp.Hash
{
    public static class HashHelper
    {
        public static string ComputeSha512Hash(string input)
        {
            using (var sha512 = SHA512.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);//Gelen metni (input) byte dizisine çeviriyor
                var hash = sha512.ComputeHash(bytes);//hash hesaplama--- 64bytes uzunluğunda olur   buradaki çıktı binary formattadır

                var sb = new StringBuilder();

                foreach (var b in hash) //Her byte’ı iki basamaklı hexadecimal (16’lık sistem) formatına çevirir
                    sb.Append(b.ToString("x2")); // 128 karakterlik string oluşur

                return sb.ToString();//sonucu string olarak dönderdik

            }
        }
    }

}
