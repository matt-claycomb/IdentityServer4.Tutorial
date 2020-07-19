using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace ClientAsymmetricKey
{
    class Program
    {
        static async Task Main()
        {
            var keys = Keys;

            Console.WriteLine($"Public Key: {JsonConvert.SerializeObject(keys.Item2)}");

            Console.WriteLine("Press enter to continue.");
            Console.ReadLine();

            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5001");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "client",
                Scope = "api1",

                ClientAssertion = new ClientAssertion
                {
                    Type = OidcConstants.ClientAssertionTypes.JwtBearer,
                    Value = CreateClientToken("client", disco.TokenEndpoint, keys)
                }
            });

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                Console.ReadLine();
                return;
            }

            Console.WriteLine(tokenResponse.Json);
            
            var apiClient = new HttpClient();
            apiClient.SetBearerToken(tokenResponse.AccessToken);

            var response = await apiClient.GetAsync("https://localhost:6001/identity");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JsonConvert.DeserializeObject(content));
            }

            Console.ReadLine();
        }

        private static Tuple<JsonWebKey, JsonWebKey> Keys
        {
            get
            {
                JsonWebKey jwkPrivate;
                JsonWebKey jwkPublic;

                var privateKeyFile = "private.json";
                var publicKeyFile = "public.json";
                try {
                    jwkPrivate = new JsonWebKey(File.ReadAllText(privateKeyFile));
                    jwkPublic = new JsonWebKey(File.ReadAllText(publicKeyFile));
                }
                catch
                {
                    if (File.Exists(privateKeyFile)) File.Delete(privateKeyFile);
                    if (File.Exists(publicKeyFile)) File.Delete(publicKeyFile);

                    var rsaPrivateKey = RSA.Create(4096);
                    var rsaPublicKey = RSA.Create();
                    rsaPublicKey.ImportRSAPublicKey(rsaPrivateKey.ExportRSAPublicKey(), out _);
                    jwkPrivate = JsonWebKeyConverter.ConvertFromRSASecurityKey(new RsaSecurityKey(rsaPrivateKey));
                    jwkPublic = JsonWebKeyConverter.ConvertFromRSASecurityKey(new RsaSecurityKey(rsaPublicKey));

                    File.WriteAllText(privateKeyFile, JsonConvert.SerializeObject(jwkPrivate));
                    File.WriteAllText(publicKeyFile, JsonConvert.SerializeObject(jwkPublic));
                }

                return new Tuple<JsonWebKey, JsonWebKey>(jwkPrivate, jwkPublic);
            }
        }

        private static string CreateClientToken(string clientId, string audience, Tuple<JsonWebKey, JsonWebKey> keys)
        {
            var now = DateTime.UtcNow;

            var token = new JwtSecurityToken(
                clientId,
                audience,
                new List<Claim>()
                {
                    new Claim("jti", Guid.NewGuid().ToString()),
                    new Claim(JwtClaimTypes.Subject, clientId),
                    new Claim(JwtClaimTypes.IssuedAt, now.ToEpochTime().ToString(), ClaimValueTypes.Integer64)
                },
                now,
                now.AddMinutes(1),
                new SigningCredentials(
                    keys.Item1,
                    SecurityAlgorithms.RsaSha256
                )
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }
    }
}
