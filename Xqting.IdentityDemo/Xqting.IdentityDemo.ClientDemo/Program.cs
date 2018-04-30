using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;

namespace Xqting.IdentityDemo.ClientDemo
{
    class Program
    {        
        static void Main(string[] args)
        {
            var task = ClientCredentialsAsync(args);
            task.Wait();
            task = ResourceOwnerAsync(args);
            task.Wait();
        }

        /// <summary>
        /// this function illustrates the client credentials access way for identityserver
        /// This is to be used when for example a backend application must use teh API. this is for 
        /// trusted applications and for secure applications because the own a secret they can use to 
        /// access the API and it must of course be sure that teh key is not shared or lost or stolen
        /// </summary>
        static async System.Threading.Tasks.Task ClientCredentialsAsync(string[] args)
        {
            Console.WriteLine("----------- CLIENT CREDENTIALS GRANT --------------");

            // discover endpoints from metadata
            var disco = await DiscoveryClient.GetAsync("http://localhost:54321");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                Console.ReadKey();
                return;
            }

            // request token
            var tokenClient = new TokenClient(disco.TokenEndpoint, "client", "secret");    //See the identity server's config.cs where the client is defined with his password
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("demoapi");//Scopes defined, in idetity server, these are the API resources that you have defined 

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                Console.ReadKey();
                return;
            }

            Console.WriteLine(tokenResponse.Json);

            // call api
            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);

            var response = await client.GetAsync("http://localhost:12345/api/protected"); //This is our protected API
            Console.WriteLine("----------- RESPONSE FROM THE API --------------");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
            }

            Console.WriteLine("End of application, press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// this function illustrates the resource owner access way for identityserverS
        /// This is to be used when users must authenticate with username and password IN A TRUSTED APPLICATIONS
        /// Browsers are not considered to be trusted and thus this is no a good way to handle browser/web app 
        /// authentication, but, can be used for authentication in legacy trusted apps
        /// </summary>
        static async System.Threading.Tasks.Task ResourceOwnerAsync(string[] args)
        {
            Console.WriteLine("----------- RESOURCE OWNER GRANT --------------");

            // discover endpoints from metadata
            var disco = await DiscoveryClient.GetAsync("http://localhost:54321");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                Console.ReadKey();
                return;
            }

            // request token
            var tokenClient = new TokenClient(disco.TokenEndpoint, "legacyuserapp", "secret");  // c:ient must still authenticate
            var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync("alice", "password", "demoapi"); // these are the user credentials to use from within the trusted application

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                Console.ReadKey();
                return;
            }

            Console.WriteLine(tokenResponse.Json);

            // call api
            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);

            var response = await client.GetAsync("http://localhost:12345/api/protected"); //This is our protected API
            Console.WriteLine("----------- RESPONSE FROM THE API --------------");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
            }

            Console.WriteLine("End of application, press any key to exit");
            Console.ReadKey();
        }
    }
}
