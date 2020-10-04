using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Entities;

namespace AliceIdentityService.Models
{
    public static class ModelExtensions
    {
        public static ClientType GetClientType(this Client client)
        {
            var value = client.Properties?.Find(p => p.Key == "ClientType").Value;
            return (ClientType)Enum.Parse(typeof(ClientType), value);
        }

        public static void SetClientType(this Client client, ClientType clientType)
        {
            if (client.Properties == null)
                client.Properties = new List<ClientProperty>();

            var property = client.Properties.Find(p => p.Key == "ClientType");
            if (property != null)
            {
                property.Value = clientType.ToString();
            }
            else
            {
                client.Properties.Add(new ClientProperty
                {
                    Key = "ClientType",
                    Value = clientType.ToString()
                });
            }
        }

        // Copied from IdentityServer4.Models to avoid namespace clash
        // with IdentityServer4.EntityFramework.Entities
        public static string Sha256(this string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hash = sha.ComputeHash(bytes);

                return Convert.ToBase64String(hash);
            }
        }

        public static string Sha512(this string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            using (var sha = SHA512.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hash = sha.ComputeHash(bytes);

                return Convert.ToBase64String(hash);
            }
        }
    }
}
