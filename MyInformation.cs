﻿using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Security;
using System.Threading.Tasks;

namespace up_console
{
    public class MyInformation
    {
        public MyInformation(IPublicClientApplication app, HttpClient client, string microsoftGraphBaseEndpoint)
        {
            tokenAcquisitionHelper = new PublicAppUsingUsernamePassword(app);
            protectedApiCallHelper = new ProtectedApiCallHelper(client);
            this.MicrosoftGraphBaseEndpoint = microsoftGraphBaseEndpoint;
        }

        protected PublicAppUsingUsernamePassword tokenAcquisitionHelper;

        protected ProtectedApiCallHelper protectedApiCallHelper;

        /// <summary>
        /// Scopes to request access to the protected Web API (here Microsoft Graph)
        /// </summary>
        private static string[] Scopes { get; set; } = new string[] { "User.Read" };

        /// <summary>
        /// Base endpoint for Microsoft Graph
        /// </summary>
        private string MicrosoftGraphBaseEndpoint { get; set; }

        /// <summary>
        /// URLs of the protected Web APIs to call (here Microsoft Graph endpoints)
        /// </summary>
        private string WebApiUrlMe { get { return $"{MicrosoftGraphBaseEndpoint}/v1.0/me"; } }
        private string WebApiUrlMyManager { get { return $"{MicrosoftGraphBaseEndpoint}/v1.0/me/manager"; } }
        private string WebApiUrlMyFiles { get { return $"{MicrosoftGraphBaseEndpoint}/v1.0/me/drive/root/children?$select=name,Id,webUrl"; } }


        /// <summary>
        /// Calls the Web API and displays its information
        /// </summary>
        /// <returns></returns>
        public async Task DisplayMeAndMyManagerRetryingWhenWrongCredentialsAsync()
        {
            bool again = true;
            while(again)
            {
                again = false;
                try
                {
                    await DisplayMeAndMyManagerAsync();
                }
                catch (ArgumentException ex) when (ex.Message.StartsWith("U/P"))
                {
                    // Wrong user or password
                    WriteTryAgainMessage();
                    again = true;
                }
            }
        }

        private async Task DisplayMeAndMyManagerAsync()
        {
            string username = "onedrive_uyg@tofas.com.tr"; 
            var secure = new SecureString();
            foreach (char c in "One09344")
            {
                secure.AppendChar(c);
            } 
            SecureString password = secure; 

            AuthenticationResult authenticationResult = await tokenAcquisitionHelper.AcquireATokenFromCacheOrUsernamePasswordAsync(Scopes, username, password);
            Console.WriteLine(username);
            if (authenticationResult != null)
            {
                DisplaySignedInAccount(authenticationResult.Account);

                string accessToken = authenticationResult.AccessToken;
                //await CallWebApiAndDisplayResultAsync(WebApiUrlMe, accessToken, "Me");
                //await CallWebApiAndDisplayResultAsync(WebApiUrlMyManager, accessToken, "My manager");
                await CallWebApiAndDisplayResultAsync(WebApiUrlMyFiles, accessToken, "My files");
            }else{
                Console.WriteLine("authenticationResult NULL");

            }
        }

        private static void WriteTryAgainMessage()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Wrong user or password. Try again!");
            Console.ResetColor();
        }
        private static void DisplaySignedInAccount(IAccount account)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{account.Username} successfully signed-in");
        }

        private async Task CallWebApiAndDisplayResultAsync(string url, string accessToken, string title)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(title);
            Console.ResetColor();
            await protectedApiCallHelper.CallWebApiAndProcessResultAsync(url, accessToken, Display);
            Console.WriteLine();
        }
        private static void Display(JObject result)
        {
            foreach (JProperty child in result.Properties().Where(p => !p.Name.StartsWith("@")))
            {
                Console.WriteLine($"{child.Name} = {child.Value}");
            }
        }
    }
}
