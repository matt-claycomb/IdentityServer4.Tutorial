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
                            Value = "{\"AdditionalData\":{\"AdditionalData\":{},\"KeyId\":null,\"X5tS256\":null,\"KeySize\":4096,\"HasPrivateKey\":false,\"CryptoProviderFactory\":{\"CryptoProviderCache\":{},\"CustomCryptoProvider\":[],\"CacheSignatureProviders\":[]}},\"Alg\":null,\"Crv\":null,\"D\":null,\"DP\":null,\"DQ\":null,\"E\":\"AQAB\",\"K\":null,\"KeyId\":null,\"Kid\":null,\"Kty\":\"RSA\",\"N\":\"2yKyNaH3u2Sqd4h8ObmugMMxyIHMEFVlX303uWm1D_vbo8gL3K4aUXWRhVrBC79GMMBUg_jNRpMMbvCsdqrO6SdvCBz7GX04Njj5bRkf6fE3btMrWT9Ltqw7fSkL7RPOBK4Fr24_VykoIY0izrejEKdz0nkGnpYeDST0oICGJvRKnOwQUHeV0SfIdzF2qQ-DE7s4h4yPE0E69QLlybrETNPkzb0HVKpflAphER40zdw40tr0WIVvReuWkY3_W_Jg0UCnSEvNS72c-QqvoFHBcnCS6Wb4hHYtC4x_uctz6Axu0rK3sNNM4xV1qen2Lc1hCqGxYJsX3DmooeX8bD0r8X7ui1gi_y7PnfSo2Nekq4v4r5pgMAsjpBVjNb-gO9sXVcgQMMNc4wTmsOwe0fxX2ubWLLn1jIWU5gkTkaOVpXbeoFUkOH2AA8upe_Bi4mWMZalrC2FJV4Xs3Cl-ajtD88rchbya8MHWBUKhPgCiMMLNxNy-nG17NyY2hVii5AcmqmfxHb9Q9-gPuv1yrgXoNqTN19G1CQoKjcVfiHgULO7bnqCq0ZgcI_8j8jUnKHptxbPLUmQ5zjHKnt6hNUfk1oXLagwm_u9YfkBjKoMSzTW4T27GPNigZES1wHqaoui4kZWHC6_6u-XcME5SPBcMdY4hAPtEZGi7Zp099azrYcE\",\"Oth\":null,\"P\":null,\"Q\":null,\"QI\":null,\"Use\":null,\"X\":null,\"X5t\":null,\"X5tS256\":null,\"X5u\":null,\"Y\":null,\"KeySize\":4096,\"HasPrivateKey\":false,\"CryptoProviderFactory\":{\"CryptoProviderCache\":{},\"CustomCryptoProvider\":null,\"CacheSignatureProviders\":true}}"
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