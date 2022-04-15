using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace SuperCabrio.Pages
{
    public partial class NewPostPage : ContentPage
    {
        public string ImageString;
        public byte[] ImageData;
        [BsonElement("NewID")]
        public string NewID { get; set; }
        [BsonElement("NewData")]
        public string NewData { get; set; }
        [BsonElement("NewName")]
        public string NewName { get; set; }
        [BsonElement("NewTime")]
        public string NewTime { get; set; }

        public bool PinCheck { get; set; }
        public string Location { get; set; }
        public string Address { get; set; }

        public NewPostPage()
        {
            InitializeComponent();
            //SearchBtn.BackgroundColor = System.Drawing.Color.LightGreen;
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            Console.WriteLine(PinCheck);
            //var settings = MongoClientSettings.FromConnectionString("mongodb+srv://Admin:Admin123@supercabrio.wiaa1.mongodb.net/SuperCabrio?retryWrites=true&w=majority");
            var settings = MongoClientSettings.FromConnectionString("mongodb://Admin:Admin123@supercabrio-shard-00-00.wiaa1.mongodb.net:27017,supercabrio-shard-00-01.wiaa1.mongodb.net:27017,supercabrio-shard-00-02.wiaa1.mongodb.net:27017/SuperCabrio?ssl=true&replicaSet=atlas-14ng79-shard-0&authSource=admin&retryWrites=true&w=majority");
            var client = new MongoClient(settings);
            var database = client.GetDatabase("SuperCabrio");
            var collection = database.GetCollection<BsonDocument>("Pictures");
            var filter = Builders<BsonDocument>.Filter.Eq("Name", HomePages.UserName);
            if (collection.Count(filter) >= 1)
            {
                PickImageBtn.IsEnabled = false;
                SaveImageBtn.IsEnabled = false;
                SaveImageBtn.Text = "Posted";
                PictureLbl.Text = "You Have Already Uploaded A Picture Today";
            }
            if (PinCheck == false)
            {
                SaveImageBtn.IsEnabled = false;
            }
        }

        //Pick Picture
        async void Image_Picker(System.Object sender, System.EventArgs e)
        {
            var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Please Pick a Photo"
            });

            if (result != null)
            {
                var stream = await result.OpenReadAsync();
                PictureLbl.IsVisible = false;
                //PickedImage.Source = ImageSource.FromStream(() => stream);

                ImageData = new byte[stream.Length];
                //await stream.ReadAsync(ImageData, 0, (int)stream.Length);
                stream.Read(ImageData, 0, (int)stream.Length);
                string ImageString1 = System.Convert.ToBase64String(ImageData);
                byte[] Base64Stream = Convert.FromBase64String(ImageString1);
                PickedImage.Source = ImageSource.FromStream(() => new MemoryStream(Base64Stream));
                //ImageString = System.Convert.ToBase64String(ImageData);

                //byte[] Base64Stream = Convert.FromBase64String(ImageString);
                //PickedImage.Source = ImageSource.FromStream(() => new MemoryStream(Base64Stream));
                //SaveImageBtn.Text = "Loading";
                if (ImageData.Length != stream.Length)
                {
                    SaveImageBtn.Text = "Loading";
                }
                if (PinCheck == true)
                {
                    SaveImageBtn.IsEnabled = true;
                }
                //CONVERT IMAGE TO JPEG
                //MemoryStream testStream = new MemoryStream();
                //var image2 = SixLabors.ImageSharp.Image.Load(ImageData);
                //image2.Mutate(x => x.Resize(image2.Width / 3, image2.Height / 3));
                //image2.SaveAsJpeg(testStream);
                //byte[] NewByte = testStream.ToArray();
                //ImageString = System.Convert.ToBase64String(NewByte);

            }
        }

        async void SaveCommand(object sender, EventArgs e)
        {
            string time, TimePosted;
            SaveImageBtn.Text = "Posting";
            try
            {
                //CONVERT IMAGE TO JPEG
                MemoryStream testStream = new MemoryStream();
                var image2 = SixLabors.ImageSharp.Image.Load(ImageData);
                Console.WriteLine("SCREEN SIZE: " + this.Height);
                var test = image2.Height;
                image2.Mutate(x => x.Resize(image2.Width / 3, image2.Height / 3));
                image2.SaveAsJpeg(testStream);
                Console.WriteLine("Debug: " + test);
                byte[] NewByte = testStream.ToArray();
                ImageString = System.Convert.ToBase64String(NewByte);
            }
            catch {
                ImageString = System.Convert.ToBase64String(ImageData);
            }
            //var settings = MongoClientSettings.FromConnectionString("mongodb://Connor:Conman00116@iphoenix1-shard-00-00.imsod.mongodb.net:27017,iphoenix1-shard-00-01.imsod.mongodb.net:27017,iphoenix1-shard-00-02.imsod.mongodb.net:27017/sample_airbnb?ssl=true&replicaSet=atlas-9scrlj-shard-0&authSource=admin&retryWrites=true&w=majority");
            //var settings = MongoClientSettings.FromConnectionString("mongodb+srv://Admin:Admin123@supercabrio.wiaa1.mongodb.net/SuperCabrio?retryWrites=true&w=majority");
            var settings = MongoClientSettings.FromConnectionString("mongodb://Admin:Admin123@supercabrio-shard-00-00.wiaa1.mongodb.net:27017,supercabrio-shard-00-01.wiaa1.mongodb.net:27017,supercabrio-shard-00-02.wiaa1.mongodb.net:27017/SuperCabrio?ssl=true&replicaSet=atlas-14ng79-shard-0&authSource=admin&retryWrites=true&w=majority");
            var client = new MongoClient(settings);
            var database = client.GetDatabase("SuperCabrio");
            var collection = database.GetCollection<BsonDocument>("Pictures");
            var cursor = collection.Find(new BsonDocument()).ToCursor();
            DateTime TheTime = DateTime.Now.Date;
            DateTime GetTimePosted = DateTime.Now;
            //DateTime later = test.AddDays(1);
            try
            {
                time = TheTime.ToString();
                TimePosted = GetTimePosted.ToShortTimeString();
            }
            catch
            {
                time = "Fail";
                TimePosted = "Fail";
            }
            //string TimePosted = GetTimePosted.ToString();
            //NewName = HomePage.UserName;
            if (ImageString != null)
            {
                var document = new BsonDocument
                    {
                        {"Data", ImageString},
                        {"Votes", 0},
                        {"Name", HomePages.UserName},
                        {"Location", Location},
                        {"Address", Address },
                        {"Time", time },
                        {"TimePosted", TimePosted},
                    };
                await collection.InsertOneAsync(document);
                Console.WriteLine("Posted");
            }
            PickImageBtn.IsEnabled = false;
            SaveImageBtn.IsEnabled = false;
            SaveImageBtn.Text = "Posted";
        }
        async void SpotClicked(object sender, MapClickedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"MapClick: {e.Position.Latitude}, {e.Position.Longitude}");
            Double PosX = e.Position.Latitude;
            Double PosY = e.Position.Longitude;
            Location = $"{e.Position.Latitude}, {e.Position.Longitude}";
            Position pos = new Position(PosX, PosY);
            //GET LOCATION NAME
            Geocoder geoCoder = new Geocoder();
            IEnumerable<string> possibleAddresses = await geoCoder.GetAddressesForPositionAsync(pos);
            Address = possibleAddresses.FirstOrDefault();
            Console.WriteLine(possibleAddresses);
            Console.WriteLine(Address);
            if (PinCheck == false)
            {
                Pin pin = new Pin
                {
                    Label = "Car Sighting",
                    Address = Address,
                    Type = PinType.Place,
                    Position = new Position(PosX, PosY)
                };
                SelectMap.Pins.Add(pin);
                PinCheck = true;
            }
            else
            {
                SelectMap.Pins[0].Address = Address;
                SelectMap.Pins[0].Position = new Position(PosX, PosY);
            }
            //Position pos = new Position(PosX, PosY);
            MapSpan mapSpan = MapSpan.FromCenterAndRadius(pos, Distance.FromKilometers(3.5));
            SelectMap.MoveToRegion(mapSpan);
            if (ImageData != null)
            {
                SaveImageBtn.IsEnabled = true;
            }
        }
        async void SearchBtnClicked(object sender, EventArgs args)
        {
            string LocEntry = LocSearch.Text;
            Geocoder geoCoder = new Geocoder();
            IEnumerable<Position> approximateLocations = await geoCoder.GetPositionsForAddressAsync(LocEntry);
            Position position = approximateLocations.FirstOrDefault();
            string coordinates = $"{position.Latitude}, {position.Longitude}";
            Location = coordinates;
            Console.WriteLine("Searched: " + coordinates);
            if (PinCheck == false)
            {
                Pin pin = new Pin
                {
                    Label = "Car Sighting",
                    Address = LocEntry,
                    Type = PinType.Place,
                    Position = position
                };
                SelectMap.Pins.Add(pin);
                PinCheck = true;
            }
            else
            {
                SelectMap.Pins[0].Position = position;
            }
            MapSpan mapSpan = MapSpan.FromCenterAndRadius(position, Distance.FromKilometers(3.5));
            SelectMap.MoveToRegion(mapSpan);
            if (ImageData != null)
            {
                SaveImageBtn.IsEnabled = true;
            }
        }
    }
}
