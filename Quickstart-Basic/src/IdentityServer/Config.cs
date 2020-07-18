// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using System.Collections.Generic;
using IdentityServer4;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            { 
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(), 
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("api1", "My Test API"), 
            };

        public static IEnumerable<ApiResource> Apis => new ApiResource[]{};

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
                    ClientId = "client",

                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    // secret for authentication
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256()),
                        new Secret
                        {
                            Type = IdentityServerConstants.SecretTypes.JsonWebKey,
                            Value = "{\"kty\":\"RSA\",\"e\":\"AQAB\",\"use\":\"sig\",\"alg\":\"RS256\",\"n\":\"yozwJWsQORdiTWLAr_tO9j8XJTQllBQpHfeEu4n1c99nhSrOT5qIci3A4yBXwwbLrn8Ya3Kn87Of5hQ-QgpMRGDlMm0oy599umpLgB-7mUng50HKfe_GPQg0Vna9EValYK5POUxhnhpY-94Xuo3XD99uJEEHinozSfw8P1IIpsrjLWTEYCIKnpvsVrVV0ge08b_5Z2zC7SHrXEW5r__NNmjB7lOCXW2yOrOqZxe9hYOaBNcSVX2_2sSSNQxLMaW7gmFq9f2W3hhVKWjM0PnHdXfKkbTGVkUvfHjpLydqkBu7BT1Lxuw2dU2lelsjNdXAHWJq1j4SwxLDhEuMF55faQ\"}"
                        }
                    },

                    // scopes that client has access to
                    AllowedScopes = {"api1"}
                },

                new Client
                {
                    ClientId = "mvc",
                    
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    AllowedGrantTypes = GrantTypes.Code,

                    RedirectUris = { "https://localhost:5002/signin-oidc"},

                    PostLogoutRedirectUris = { "https://localhost:5002/signout-callback-oidc" },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api1"
                    }
                }
            };
    }
}