using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Identity.Client;
using Xamarin.Forms;

namespace SuperCabrio.Pages
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            SignInLoader.IsVisible = true;

        }

        protected override async void OnAppearing()
        {
            try
            {
                // Look for existing account
                var accounts = await App.AuthenticationClient.GetAccountsAsync();
                if (accounts.Count() >= 1)
                {
                    //SignInLoader.IsVisible = true;
                    SignInLoader.IsRunning = true;
                    SignInBtn.IsVisible = false;
                    AuthenticationResult result = await App.AuthenticationClient
                        .AcquireTokenSilent(Constants.Scopes, accounts.FirstOrDefault())
                        .ExecuteAsync();

                    await Navigation.PushAsync(new HomePages(result));
                }
            }
            catch
            {
                // Do nothing - the user isn't logged in
            }
            base.OnAppearing();
        }
        async void OnSignInClicked(Object Sender, EventArgs e)
        {
            AuthenticationResult result;
            try
            {
                Console.WriteLine("Point 1");
                result = await App.AuthenticationClient
                    .AcquireTokenInteractive(Constants.Scopes)
                    .WithPrompt(Prompt.ForceLogin)
                    .WithParentActivityOrWindow(App.UIParent)
                    .ExecuteAsync();
                //await Navigation.PushAsync(new LoginResultPage(result));
                Console.WriteLine("point achieved");
                await Navigation.PushAsync(new HomePages(result));
            }
            catch (MsalClientException ex)
            {
                Console.WriteLine("Fail");
            }
        }
    }
}
