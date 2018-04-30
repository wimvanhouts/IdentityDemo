using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Xqting.IdentityDemo.IdentityService
{
    public class Config
    {
        /// <summary>
        /// An API resource represent the API you want to protect and for which users can ask access. Each API is what is in the documentation called a "scope".
        /// In a predefined context where every API is known for the solution this can be a fixed list of scopes.
        /// In case you make a generic identity service that can be used for multiple API's in the company for example, 
        /// the list of APIs will come from some database or storage somewhere, as well as the clients and users linked to it
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("demoapi", "Protected API demo")
            };
        }


        /// <summary>
        /// Once we add open id connect to the system (on top of or next to OAuth2, scopes are used in openid connect to tell the system 
        /// what information is requested in the token (in OAuth, scopes correspond with an API)
        /// </summary>
        /// <returns></returns>
        internal static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),     // --> Default openid data, which is the subject id           
                new IdentityResources.Profile(),    // --> Basic profile information
            };
        }

        /// <summary>
        /// A client credential is a type of access that is provided to client applications that have a predefined and/or configured
        /// secret that they got somehow and that they can use to connect to the API. For example, this is the kind of access
        /// you would give to a backend application in a trusted environment where you are sure the secret cannot just be stolen.
        /// A client is NOT a person but some kind of trusted application!
        /// Typically, this is not a fixed list
        /// </summary>
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                //Client setup for the client credentials grant... this a specific flow for back-end applications
                new Client
                {
                    ClientId = "client",

                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    // secret for authentication
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    // scopes that client has access to --> See the API resource definitions, which are actually defining the scopes.
                    AllowedScopes = { "demoapi" }
                }, 

                // resource owner password grant client.. this is a client that can be used in combination with the resource password grant
                // flow, BUT, this is only advised if the client application (that is why you must declare a client) is trusted, and cannot 
                // be used for a browser since browser is not considered to be trusted. This can eb done for legacy applications where
                // a user must login with his user name and password. This application will then become the defined client
                new Client
                {
                    ClientId = "legacyuserapp", // --> Name for the client, for example some well known app
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "demoapi" }
                },

                // JavaScript Client
                // this client represent our Javascript front-end where users work with and that will use the identity server with the oidc-client.Js file
                // THis is the OIDC Implicit flow, which is the one that should be used for browsers. 
                new Client
                {
                    ClientId = "javascript",
                    ClientName = "JavaScript Client",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,

                    RedirectUris =           { "http://localhost:55555/callback.html" }, //--> No idea why this is needed to be configured because it is also configured in the client
                    PostLogoutRedirectUris = { "http://localhost:55555/index.html" },    //--> No idea why this is needed to be configured because it is also configured in the client
                    AllowedCorsOrigins =     { "http://localhost:55555" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "demoapi"                                           // --> Make sure to the add our own extra scopes for this type of user
                    }
                }
            };
        }

        /// <summary>
        /// Test users represent the people that you want to give access to your API. People that have a certain username and password
        /// and will get access to certain APIs or not. In practice, the list of users will always come from the database, or, 
        /// the users will be verified when authentication is done against a database at runtime, and it will NOT be a fixed list 
        /// of users.
        /// </summary>
        /// <returns></returns>
        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "alice",
                    Password = "password"
                },
                new TestUser
                {
                    SubjectId = "2",
                    Username = "bob",
                    Password = "password"
                }
            };
        }
    }
}
