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
            Console.WriteLine("Sleeping for 30 seconds...");
            await Task.Delay(30000);
            Console.WriteLine("Done Waiting!");

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
                    Value = CreateClientToken("client", disco.TokenEndpoint)
                    //TODO Generate Value
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

        private static string CreateClientToken(string clientId, string audience)
        {
            string jwkPair =
                "{\"p\":\"5fRQcTE_64HPa6jE863wzKmeFDJN4FbwNHBCdFmrFvo9Swq6HBGkOMgfKeuRlOA12CG9MadDDNjB_xqGAYWNPoYvw9Neh1AZS0SM578DdhFbZfHJL8gQfqQcrX1dkgis_8aj-oySykwsIPUbVkQTkKZuIEUylpoILeOcq_io-1c\",\"kty\":\"RSA\",\"q\":\"4X4IAwL-STBb6P21dEkxw0o9LUtb2qayiLxpbH2CJYdHUTXBhjdcp5ym49wxLdONrMEtGrESH5XGdC791AXM2UrExJ3Xk8Vslc-29A0agg1eaCuS_7IfpqPCSAzFfEHeYkKd30x8FnJpvcheKr4tBTzjbUS4TPcPDivSjmYNgz8\",\"d\":\"l-BoLVaDOpvbHPWgUBSbV69k9oOMbeVUE2FGiKzgfHsWj1bSjgPsrdi62lOlGtqcnYI1WFQHtYeJrSaerDw2hcs18MLDJTO5Pa94X2Dr9PvdhOnjIN1KgvcE75RnHnoe1Ui-pNMuGzPcZzdYCY1TplY9qzeLpP0OOjrFHiGv1QfVIEMSVmanQHwIcZ8uPMU3IJ2Afeq8p4twoUa1v5bb5Jdx3VGSdx5FlShvGVXKIL7r2Z1FtBbZu_20nZ2bwg7TdjWUAEXn_jQ6kKQ7iQ4dAQjCJKdMr0ejeF-7vo9vdMXQw1j-S4J8Q963SWfSjOUbXaDJnvJ83_if0DNnn8NW5Q\",\"e\":\"AQAB\",\"use\":\"sig\",\"qi\":\"0sLeItl7eK8FRsSSig0aIBa6Xw_Whq6kKn6iPYEv1zTHjfFsfg5VgkTQDCrcj9YMh7-P5tIz4_PyGDzC8Pllxs4CGSRDJWCEKcKOGz6aPsQ4Bo-qDMB-7cqoKH-opAnf2E6qkIaEu61oslFgCZ8RYbxv3odASUwb6g9LuhGQ76E\",\"dp\":\"yXWppAFJxsILPn61Tedtt2BCpd8Maya_erF_BwAn7BF_8WaMdlreTQDGjKtc99kWUyv73Kx1CaQ9EYnjLSIcBFL7NczQXCvIfTCpYb7vAVljgYS6han5CXotxbC3cWE4bz1QHAlb8O-bGaSjNoidu3STtqHpGBgKYmmusNtA8DE\",\"alg\":\"RS256\",\"dq\":\"biu5ll8cBn5OKaPryAkK-zOGeF7dXhMcH5qOl6cqs1NJEPCGwSDDcFElaSboEE_QZJttgiEJjc0LzTA4bCL5VyEkyjukXFqVFw0Jgmv0i54khhjkFXHd7PkNr04uHuu_z5pkr_kal40HBOaku5V3QZviMjzlHxWGkZHvqgtKhtk\",\"n\":\"yozwJWsQORdiTWLAr_tO9j8XJTQllBQpHfeEu4n1c99nhSrOT5qIci3A4yBXwwbLrn8Ya3Kn87Of5hQ-QgpMRGDlMm0oy599umpLgB-7mUng50HKfe_GPQg0Vna9EValYK5POUxhnhpY-94Xuo3XD99uJEEHinozSfw8P1IIpsrjLWTEYCIKnpvsVrVV0ge08b_5Z2zC7SHrXEW5r__NNmjB7lOCXW2yOrOqZxe9hYOaBNcSVX2_2sSSNQxLMaW7gmFq9f2W3hhVKWjM0PnHdXfKkbTGVkUvfHjpLydqkBu7BT1Lxuw2dU2lelsjNdXAHWJq1j4SwxLDhEuMF55faQ\"}";

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
                    new JsonWebKey(jwkPair),
                    SecurityAlgorithms.RsaSha256
                )
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }
    }
}
