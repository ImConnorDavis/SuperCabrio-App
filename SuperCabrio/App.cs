using System;
using SuperCabrio.Pages;
using Microsoft.Identity.Client;
using Xamarin.Forms;

namespace SuperCabrio
{
    public partial class App : Application
    {
        public static IPublicClientApplication AuthenticationClient { get; private set; }

        public static object UIParent { get; set; } = null;

        public App()
        {

            AuthenticationClient = PublicClientApplicationBuilder.Create(Constants.ClientId)
                .WithIosKeychainSecurityGroup(Constants.IosKeychainSecurityGroups)
                .WithB2CAuthority(Constants.AuthoritySignIn)
                .WithRedirectUri($"msal{Constants.ClientId}://auth")
                .Build();

            MainPage = new NavigationPage(new LoginPage());
        }
    }
}
