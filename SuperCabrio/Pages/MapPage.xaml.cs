using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace SuperCabrio.Pages
{
    public partial class MapPage : ContentPage
    {
        public MapPage()
        {
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            Position StartPosition = new Position(35, -40);
            MapSpan mapSpan = MapSpan.FromCenterAndRadius(StartPosition, Distance.FromKilometers(4000));
            MainMap.MoveToRegion(mapSpan);
            MapUpdate();
        }
        public async void MapUpdate()
        {
            //var settings = MongoClientSettings.FromConnectionString("mongodb+srv://Admin:Admin123@supercabrio.wiaa1.mongodb.net/SuperCabrio?retryWrites=true&w=majority");
            var settings = MongoClientSettings.FromConnectionString("mongodb://Admin:Admin123@supercabrio-shard-00-00.wiaa1.mongodb.net:27017,supercabrio-shard-00-01.wiaa1.mongodb.net:27017,supercabrio-shard-00-02.wiaa1.mongodb.net:27017/SuperCabrio?ssl=true&replicaSet=atlas-14ng79-shard-0&authSource=admin&retryWrites=true&w=majority");
            var client = new MongoClient(settings);
            var database = client.GetDatabase("SuperCabrio");
            var collection = database.GetCollection<BsonDocument>("Pictures");
            BsonElement LocationSet, AddSet, NameSet;
            var cursor = collection.Find(_ => true);
            foreach (var document in cursor.ToEnumerable())
            {
                LocationSet = document.GetElement("Location");
                string LocationString = LocationSet.ToString().Replace("Location=", "");
                NameSet = document.GetElement("Name");
                string NameString = NameSet.ToString().Replace("Name=", "");
                AddSet = document.GetElement("Address");
                string AddressString = AddSet.ToString().Replace("Address=", "");

                string[] PointArray = LocationString.Split(',');
                double x = Convert.ToDouble(PointArray[0]);
                double y = Convert.ToDouble(PointArray[1]);
                Position position = new Position(x, y);
                Pin pin = new Pin
                {
                    Label = NameString,
                    Address = AddressString,
                    Type = PinType.Place,
                    Position = position,
                };
                MainMap.Pins.Add(pin);
                pin.Clicked += async (object sender, EventArgs e) =>
                //pin.MarkerClicked += async (s, args) =>
                {
                    Console.WriteLine("KJFDSLKJFSDKLJFSDKLJSDFKLJDF");
                    //var p = sender as Pin;
                    BsonElement MyDataSet, MyNameSet, MyVotesSet, MyTimeSet, MyTimePostedSet, MyidSet, MyLocSet, MyAddSet;
                    //var NewLikeFilter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(PostID));
                    var CheckPostFilter = Builders<BsonDocument>.Filter.Eq("Name", pin.Label);
                    var entity = collection.Find(CheckPostFilter);
                    try
                    {
                        foreach (var document2 in entity.ToEnumerable())
                        {
                            Console.WriteLine("CheckPoint1");
                            MyDataSet = document2.GetElement("Data");
                            MyNameSet = document2.GetElement("Name");
                            MyVotesSet = document2.GetElement("Votes");
                            MyAddSet = document2.GetElement("Address");
                            MyLocSet = document2.GetElement("Location");
                            MyTimeSet = document2.GetElement("Time");
                            MyTimePostedSet = document2.GetElement("TimePosted");
                            MyidSet = document2.GetElement("_id");
                            Console.WriteLine("CheckPoint2");
                            Console.WriteLine(MyDataSet);
                            Console.WriteLine(MyNameSet);
                            Console.WriteLine(MyVotesSet);
                            Console.WriteLine(MyAddSet);
                            Console.WriteLine(MyLocSet);
                            Console.WriteLine(MyTimeSet);
                            Console.WriteLine(MyTimePostedSet);
                            Console.WriteLine(MyidSet);
                        }
                        Console.WriteLine("CheckPoint3");
                        string Data = MyDataSet.ToString().Replace("Data=", "");
                        string Name = MyNameSet.ToString().Replace("Name=", "");
                        string VotesString = MyVotesSet.ToString().Replace("Votes=", "");
                        int Votes = Convert.ToInt32(VotesString);
                        string Address = MyAddSet.ToString().Replace("Address=", "");
                        string Location = MyLocSet.ToString().Replace("Location=", "");
                        string TimeString = MyTimeSet.ToString().Replace("Time=", "");
                        string TimePosted = MyTimePostedSet.ToString().Replace("TimePosted=", "");
                        string ID = MyidSet.ToString().Replace("_id=", "");
                        Console.WriteLine("CheckPoint4");
                        await Navigation.PushAsync(new ViewPost(Data, Name, Votes, Location, Address, TimePosted, ID));
                    }
                    catch
                    {
                        Console.WriteLine("FAIL");
                    }
                };
            }
        }
    }
}
