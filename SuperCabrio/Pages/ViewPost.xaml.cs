using System;
using System.Collections.Generic;
using System.IO;
using MongoDB.Bson;
using MongoDB.Driver;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace SuperCabrio.Pages
{
    public partial class ViewPost : ContentPage
    {
        public static string NameCheck { get; set; }
        public static int NewVotes { get; set; }
        public static int OldVotes { get; set; }
        public static bool CheckClick = false;
        public string LocationSet, AddressSet;
        public string GlobalName;
        public string PostID;
        public int MaxLikes;
        public bool AlreadyLiked = false;

        public ViewPost(string Data, string Name, int Votes, string Location, string Address, string TimePosted, string ID)
        {
            InitializeComponent();
            GlobalName = Name;
            PostName.Text = Name;
            NameCheck = Name;
            NewVotes = Votes;
            LocationSet = Location;
            AddressSet = Address;
            PostID = ID;
            MaxLikes = 0;
            //VoteBtn.BackgroundColor = Color.Green;
            //Appearing += (sender, e) =>
            //{
            //OnAppearing1();
            //Console.WriteLine("OPENED");
            //};
            TimeStamp.Text = TimePosted;
            //VoteCount.Text = Votes.ToString();
            //if (CheckClick == false) VoteBtnText.Text = "UnVote";
            //if (CheckClick == true) VoteBtnText.Text = "Vote";
            byte[] Picture = Convert.FromBase64String(Data);
            PostImage.Source = ImageSource.FromStream(() => new MemoryStream(Picture));
        }
        protected override async void OnAppearing()
        {
            //CHECK FOR ACCOUNT LIKED PICTURES
            CheckClick = true;
            Console.WriteLine("CLICKTEST: " + CheckClick);
            //var settings = MongoClientSettings.FromConnectionString("mongodb+srv://Admin:Admin123@supercabrio.wiaa1.mongodb.net/SuperCabrio?retryWrites=true&w=majority");
            var settings = MongoClientSettings.FromConnectionString("mongodb://Admin:Admin123@supercabrio-shard-00-00.wiaa1.mongodb.net:27017,supercabrio-shard-00-01.wiaa1.mongodb.net:27017,supercabrio-shard-00-02.wiaa1.mongodb.net:27017/SuperCabrio?ssl=true&replicaSet=atlas-14ng79-shard-0&authSource=admin&retryWrites=true&w=majority");
            var client = new MongoClient(settings);
            var database = client.GetDatabase("SuperCabrio");
            var collection = database.GetCollection<BsonDocument>("Pictures");
            var collection2 = database.GetCollection<BsonDocument>("Likes");

            //GET UPDATED LIKES
            BsonElement NewVoteSet;
            var NewLikeFilter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(PostID));
            var entity = collection.Find(NewLikeFilter);
            foreach (var document in entity.ToEnumerable())
            {
                NewVoteSet = document.GetElement("Votes");
                string NewVotesString = NewVoteSet.ToString().Replace("Votes=", "");
                NewVotes = Convert.ToInt32(NewVotesString);
                Console.WriteLine("NEW VOTES" + NewVotes);
            }
            VoteCount.Text = NewVotes.ToString();

            //CHECK IF USER VOTED
            var filter = Builders<BsonDocument>.Filter.Eq("Name", HomePages.UserName);
            if (collection2.Count(filter) >= 1)
            {
                var filter1 = Builders<BsonDocument>.Filter.Eq("Vote1", GlobalName);
                if (collection2.Count(filter1) >= 1)
                {
                    CheckClick = false;
                    AlreadyLiked = true;
                }
                var filter2 = Builders<BsonDocument>.Filter.Eq("Vote2", GlobalName);
                if (collection2.Count(filter2) >= 1)
                {
                    CheckClick = false;
                    AlreadyLiked = true;
                }
                var filter3 = Builders<BsonDocument>.Filter.Eq("Vote3", GlobalName);
                if (collection2.Count(filter3) >= 1)
                {
                    CheckClick = false;
                    AlreadyLiked = true;
                }
            }
            var filter7 = Builders<BsonDocument>.Filter.Eq("Name", HomePages.UserName);
            if (collection2.Count(filter7) >= 1)
            {
                var CheckFilter = Builders<BsonDocument>.Filter.Eq("Vote1", BsonNull.Value);
                if (collection2.Count(CheckFilter) <= 0)
                {
                    MaxLikes = MaxLikes + 1;
                    Console.WriteLine("VOTE1 TRUE");
                }
                var CheckFilter2 = Builders<BsonDocument>.Filter.Eq("Vote2", BsonNull.Value);
                if (collection2.Count(CheckFilter2) <= 0)
                {
                    Console.WriteLine("VOTE2 TRUE./.....");
                    MaxLikes = MaxLikes + 1;
                    Console.WriteLine("VOTE2 TRUE");
                }
                var CheckFilter3 = Builders<BsonDocument>.Filter.Eq("Vote3", BsonNull.Value);
                if (collection2.Count(CheckFilter3) <= 0)
                {
                    MaxLikes = MaxLikes + 1;
                    Console.WriteLine("VOTE3 TRUE");
                }
            }
            if (CheckClick == false) VoteBtn.Text = "UnVote";
            if (CheckClick == true) VoteBtn.Text = "Vote";
            Console.WriteLine("MAXLIKES: " + MaxLikes);
            if (MaxLikes >= 3)
            {
                VoteBtn.IsEnabled = false;
                VoteBtn.Text = "Out Of Likes";
            }
            int MyVotesString = 3 - MaxLikes;
            VotesLeftLbl.Text = MyVotesString.ToString();
            string[] PointArray = LocationSet.Split(',');
            double x = Convert.ToDouble(PointArray[0]);
            double y = Convert.ToDouble(PointArray[1]);
            Position position = new Position(x, y);
            Pin pin = new Pin
            {
                Label = "Car Sighting",
                Address = AddressSet,
                Type = PinType.Place,
                Position = position,
            };
            PictureMap.Pins.Add(pin);
            MapSpan mapSpan = MapSpan.FromCenterAndRadius(position, Distance.FromKilometers(3.5));
            PictureMap.MoveToRegion(mapSpan);
            base.OnAppearing();
        }

        protected override async void OnDisappearing()
        {
            //base.OnDisappearing();
            //var settings = MongoClientSettings.FromConnectionString("mongodb+srv://Admin:Admin123@supercabrio.wiaa1.mongodb.net/SuperCabrio?retryWrites=true&w=majority");
            var settings = MongoClientSettings.FromConnectionString("mongodb://Admin:Admin123@supercabrio-shard-00-00.wiaa1.mongodb.net:27017,supercabrio-shard-00-01.wiaa1.mongodb.net:27017,supercabrio-shard-00-02.wiaa1.mongodb.net:27017/SuperCabrio?ssl=true&replicaSet=atlas-14ng79-shard-0&authSource=admin&retryWrites=true&w=majority");
            var client = new MongoClient(settings);
            var database = client.GetDatabase("SuperCabrio");
            var collection = database.GetCollection<BsonDocument>("Pictures");
            var collection2 = database.GetCollection<BsonDocument>("Likes");
            //await collection.FindOneAndUpdateAsync(
            //Builders<BsonDocument>.Filter.Eq("Name", NameCheck),
            //Builders<BsonDocument>.Update.Set("Votes", NewVotes));

            //Post 1 Checks
            var NameFilter = Builders<BsonDocument>.Filter.Eq("Vote1", HomePages.UserName);
            var CheckFilter = Builders<BsonDocument>.Filter.Eq("Vote1", BsonNull.Value);
            var LikeFilter = Builders<BsonDocument>.Filter.Eq("Vote1", GlobalName);
            var filter = Builders<BsonDocument>.Filter.Eq("Name", HomePages.UserName);
            if (CheckClick == true)
            {
                Console.WriteLine("ALREADY LIKED: " + AlreadyLiked);
                if (AlreadyLiked == true)
                {
                    BsonElement CheckVoteSet;
                    int UpdatedVotes;
                    var CheckVoteFilter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(PostID));
                    var entity = collection.Find(CheckVoteFilter);
                    foreach (var document in entity.ToEnumerable())
                    {
                        CheckVoteSet = document.GetElement("Votes");
                        string CheckVotesString = CheckVoteSet.ToString().Replace("Votes=", "");
                        UpdatedVotes = Convert.ToInt32(CheckVotesString);
                        Console.WriteLine("NEW VOTES GOTTEN");
                        if (UpdatedVotes + 1 != NewVotes)
                        {
                            NewVotes = UpdatedVotes - 1;
                            collection.FindOneAndUpdate(
                                Builders<BsonDocument>.Filter.Eq("Name", NameCheck),
                                Builders<BsonDocument>.Update.Set("Votes", NewVotes));
                        }
                        else
                        {
                            collection.FindOneAndUpdate(
                                Builders<BsonDocument>.Filter.Eq("Name", NameCheck),
                                Builders<BsonDocument>.Update.Set("Votes", NewVotes));
                        }
                    }
                }
                Console.WriteLine("Liked");
                if (collection2.Count(filter) <= 0)
                {
                    //var document = new BsonDocument
                    //{
                    //    {"Name", HomePages.UserName},
                    //    {"Vote1", GlobalName},
                    //};
                    //await collection2.InsertOneAsync(document);
                }
                if (collection2.Count(LikeFilter) >= 1)
                {
                    await collection2.FindOneAndUpdateAsync(
                        Builders<BsonDocument>.Filter.Eq("Name", HomePages.UserName),
                        Builders<BsonDocument>.Update.Set("Vote1", BsonNull.Value));
                }
                var LikeFilter2 = Builders<BsonDocument>.Filter.Eq("Vote2", GlobalName);
                if (collection2.Count(LikeFilter2) >= 1)
                {
                    await collection2.FindOneAndUpdateAsync(
                        Builders<BsonDocument>.Filter.Eq("Name", HomePages.UserName),
                        Builders<BsonDocument>.Update.Set("Vote2", BsonNull.Value));
                }
                var LikeFilter3 = Builders<BsonDocument>.Filter.Eq("Vote3", GlobalName);
                if (collection2.Count(LikeFilter3) >= 1)
                {
                    await collection2.FindOneAndUpdateAsync(
                        Builders<BsonDocument>.Filter.Eq("Name", HomePages.UserName),
                        Builders<BsonDocument>.Update.Set("Vote3", BsonNull.Value));
                }
            }

            if (CheckClick == false)
            {
                Console.WriteLine("ALREADY LIKED: " + AlreadyLiked);
                if (AlreadyLiked == false)
                {
                    BsonElement CheckVoteSet;
                    int UpdatedVotes;
                    var CheckVoteFilter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(PostID));
                    var entity = collection.Find(CheckVoteFilter);
                    foreach (var document in entity.ToEnumerable())
                    {
                        CheckVoteSet = document.GetElement("Votes");
                        string CheckVotesString = CheckVoteSet.ToString().Replace("Votes=", "");
                        UpdatedVotes = Convert.ToInt32(CheckVotesString);
                        Console.WriteLine("NEW VOTES GOTTEN");
                        if (UpdatedVotes - 1 != NewVotes)
                        {
                            NewVotes = UpdatedVotes + 1;
                            collection.FindOneAndUpdate(
                                Builders<BsonDocument>.Filter.Eq("Name", NameCheck),
                                Builders<BsonDocument>.Update.Set("Votes", NewVotes));
                        }
                        else
                        {
                            collection.FindOneAndUpdate(
                                Builders<BsonDocument>.Filter.Eq("Name", NameCheck),
                                Builders<BsonDocument>.Update.Set("Votes", NewVotes));
                        }
                    }
                }
                if (collection2.Count(filter) <= 0)
                {
                    var document = new BsonDocument
                    {
                        {"Name", HomePages.UserName},
                        {"Vote1", GlobalName},
                    };
                    await collection2.InsertOneAsync(document);
                }
                //if (collection2.Count(filter) >= 1)
                else
                {
                    bool OwnCheck = false;
                    //CHECK IF USERS LIKED OWN POST
                    if (GlobalName == HomePages.UserName)
                    {
                        var OwnFilter = Builders<BsonDocument>.Filter.Eq("Vote1", HomePages.UserName);
                        if (collection2.Count(OwnFilter) >= 1)
                        {
                            OwnCheck = true;
                        }
                        var OwnFilter2 = Builders<BsonDocument>.Filter.Eq("Vote2", HomePages.UserName);
                        if (collection2.Count(OwnFilter2) >= 1)
                        {
                            OwnCheck = true;
                        }
                        var OwnFilter3 = Builders<BsonDocument>.Filter.Eq("Vote3", HomePages.UserName);
                        if (collection2.Count(OwnFilter3) >= 1)
                        {
                            OwnCheck = true;
                        }
                    }
                    var DupFilter = Builders<BsonDocument>.Filter.Eq("Vote1", GlobalName);
                    if (collection2.Count(DupFilter) >= 1)
                    {
                        OwnCheck = true;
                    }
                    var DupFilter2 = Builders<BsonDocument>.Filter.Eq("Vote2", GlobalName);
                    if (collection2.Count(DupFilter2) >= 1)
                    {
                        OwnCheck = true;
                    }
                    var DupFilter3 = Builders<BsonDocument>.Filter.Eq("Vote3", GlobalName);
                    if (collection2.Count(DupFilter3) >= 1)
                    {
                        OwnCheck = true;
                    }

                    if (OwnCheck == false)
                    {
                        if (collection2.Count(CheckFilter) >= 1)
                        {
                            var CheckFilter2 = Builders<BsonDocument>.Filter.Eq("Vote2", GlobalName);
                            var CheckFilter3 = Builders<BsonDocument>.Filter.Eq("Vote3", GlobalName);
                            if (collection2.Count(CheckFilter2) <= 0 && collection2.Count(CheckFilter3) <= 0)
                            {
                                await collection2.FindOneAndUpdateAsync(
                                    Builders<BsonDocument>.Filter.Eq("Name", HomePages.UserName),
                                    Builders<BsonDocument>.Update.Set("Vote1", GlobalName));
                                AlreadyLiked = true;
                            }
                        }
                        //CHECK FOR VOTE 2
                        else
                        {
                            Console.WriteLine("kjdsklfjskldjf");
                            var CheckFilter2 = Builders<BsonDocument>.Filter.Eq("Vote2", BsonNull.Value);
                            if (collection2.Count(CheckFilter2) >= 1)
                            {
                                await collection2.FindOneAndUpdateAsync(
                                    Builders<BsonDocument>.Filter.Eq("Name", HomePages.UserName),
                                    Builders<BsonDocument>.Update.Set("Vote2", GlobalName));
                            }
                            else
                            {
                                var CheckFilter3 = Builders<BsonDocument>.Filter.Eq("Vote3", BsonNull.Value);
                                if (collection2.Count(CheckFilter3) >= 1)
                                {
                                    await collection2.FindOneAndUpdateAsync(
                                        Builders<BsonDocument>.Filter.Eq("Name", HomePages.UserName),
                                        Builders<BsonDocument>.Update.Set("Vote3", GlobalName));
                                }
                            }
                        }
                    }
                }
            }
            Console.WriteLine("CLICKTEST: " + CheckClick);
            base.OnDisappearing();

        }

        async void VoteBtnClicked(object sender, EventArgs e)
        {
            if (CheckClick == true)
            {
                NewVotes = NewVotes + 1;
                VoteBtn.Text = "UnVote";
                CheckClick = false;
                Console.WriteLine("VOTES: " + NewVotes);
            }
            else
            {
                NewVotes = NewVotes - 1;
                VoteBtn.Text = "Vote";
                CheckClick = true;
                Console.WriteLine("VOTES: " + NewVotes);
            }
            VoteCount.Text = NewVotes.ToString();
        }
    }
}
