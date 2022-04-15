using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.Identity.Client;
using MongoDB.Bson;
using MongoDB.Driver;
using Xamarin.Forms;

namespace SuperCabrio.Pages
{
    public partial class HomePages : TabbedPage
    {
        public static string UserName { get; set; }
        public string StartUp { get; set; }

        //public static string UserRole { get; set; }

        //private Button Tickets;
        private AuthenticationResult authenticationResult;

        public HomePages(AuthenticationResult authResult)
        {
            authenticationResult = authResult;
            InitializeComponent();
            if (Application.Current.Properties.ContainsKey("FirstUse"))
            {
                //Do things if it's NOT the first run of the app...
            }
            else
            {
                //BindingContext = new PostControl();
            }
            //BindingContext = new PostControl();

            //Appearing += (sender, e) =>
            //{
            //    BindingContext = new PostControl();
            //};
        }
        protected override void OnAppearing()
        {
            GetClaims();
            base.OnAppearing();
            //BindingContext = new PostControl();
        }
        private void GetClaims()
        {
            var token = authenticationResult.IdToken;
            if (token != null)
            {
                var handler = new JwtSecurityTokenHandler();
                var data = handler.ReadJwtToken(authenticationResult.IdToken);
                var claims = data.Claims.ToList();
                if (data != null)
                {
                    this.welcome.Text = $"Welcome {data.Claims.FirstOrDefault(x => x.Type.Equals("given_name")).Value}";
                    UserName = data.Claims.FirstOrDefault(x => x.Type.Equals("name")).Value;
                    this.UserNameLbl.Text = "Username: " + UserName;
                    //this.address.Text = $"Address: {data.Claims.FirstOrDefault(x => x.Type.Equals("streetAddress")).Value}";
                    //string UserAddress = data.Claims.FirstOrDefault(x => x.Type.Equals("UserName")).Value;
                    //this.jobtitle.Text = $"Welcome {data.Claims.FirstOrDefault(x => x.Type.Equals("given_name")).Value}";
                    //this.issuer.Text = $"Issuer: { data.Claims.FirstOrDefault(x => x.Type.Equals("iss")).Value}";
                    //this.subscription.Text = $"Subscription: {data.Claims.FirstOrDefault(x => x.Type.Equals("sub")).Value}";
                    //this.audience.Text = $"Audience: {data.Claims.FirstOrDefault(x => x.Type.Equals("aud")).Value}";
                    //this.role.Text = $"Role: {data.Claims.FirstOrDefault(x => x.Type.Equals("jobTitle")).Value}";
                    //UserRole = data.Claims.FirstOrDefault(x => x.Type.Equals("jobTitle")).Value;
                }
            }
        }
        async void SignOutBtn_Clicked(System.Object sender, System.EventArgs e)
        {
            var accounts = (await App.AuthenticationClient.GetAccountsAsync()).ToList();
            while (accounts.Any())
            {
                await App.AuthenticationClient.RemoveAsync(accounts.First());
                await App.AuthenticationClient.RemoveAsync(authenticationResult.Account);
                accounts = (await App.AuthenticationClient.GetAccountsAsync()).ToList();
            }
            //await App.AuthenticationClient.RemoveAsync(authenticationResult.Account);
            await Navigation.PushAsync(new LoginPage());
        }
        async void ViewMyPostBtn_Clicked(System.Object sender, System.EventArgs e)
        {
            //var settings = MongoClientSettings.FromConnectionString("mongodb+srv://Admin:Admin123@supercabrio.wiaa1.mongodb.net/SuperCabrio?retryWrites=true&w=majority");
            var settings = MongoClientSettings.FromConnectionString("mongodb://Admin:Admin123@supercabrio-shard-00-00.wiaa1.mongodb.net:27017,supercabrio-shard-00-01.wiaa1.mongodb.net:27017,supercabrio-shard-00-02.wiaa1.mongodb.net:27017/SuperCabrio?ssl=true&replicaSet=atlas-14ng79-shard-0&authSource=admin&retryWrites=true&w=majority");
            var client = new MongoClient(settings);
            var database = client.GetDatabase("SuperCabrio");
            var collection = database.GetCollection<BsonDocument>("Pictures");

            BsonElement MyDataSet, MyNameSet, MyVotesSet, MyTimeSet, MyTimePostedSet, MyidSet, MyLocSet, MyAddSet;
            //var NewLikeFilter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(PostID));
            var CheckPostFilter = Builders<BsonDocument>.Filter.Eq("Name", UserName);
            var entity = collection.Find(CheckPostFilter);
            try
            {
                foreach (var document in entity.ToEnumerable())
                {
                    MyDataSet = document.GetElement("Data");
                    MyNameSet = document.GetElement("Name");
                    MyVotesSet = document.GetElement("Votes");
                    MyAddSet = document.GetElement("Address");
                    MyLocSet = document.GetElement("Location");
                    MyTimeSet = document.GetElement("Time");
                    MyTimePostedSet = document.GetElement("TimePosted");
                    MyidSet = document.GetElement("_id");
                }
                string Data = MyDataSet.ToString().Replace("Data=", "");
                string Name = MyNameSet.ToString().Replace("Name=", "");
                string VotesString = MyVotesSet.ToString().Replace("Votes=", "");
                int Votes = Convert.ToInt32(VotesString);
                string Address = MyAddSet.ToString().Replace("Address=", "");
                string Location = MyLocSet.ToString().Replace("Location=", "");
                string TimeString = MyTimeSet.ToString().Replace("Time=", "");
                string TimePosted = MyTimePostedSet.ToString().Replace("TimePosted=", "");
                string ID = MyidSet.ToString().Replace("_id=", "");
                await Navigation.PushAsync(new ViewPost(Data, Name, Votes, Location, Address, TimePosted, ID));
            }
            catch
            {
                Console.WriteLine("FAIL");
            }
        }
        async void MoreInfoBtnClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new InfoPage());
        }
        async void InstagramClicked(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri("https://instagram.com/supercabrio?utm_medium=copy_link"));
        }
    }
}
