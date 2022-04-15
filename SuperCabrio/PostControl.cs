using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using SuperCabrio.Pages;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Operations;
using Xamarin.Forms;
using System.Net.Mail;
using System.Net;
using System.Net.Mime;

namespace SuperCabrio
{
    public class PostControl : INotifyPropertyChanged
    {
        string DataString;
        string VotesString;
        string NameString;
        string TimeString;
        string TimePostedString;
        string LocationString;
        string AddressString;
        string id;
        int VotesInt;
        bool WinCheck;
        string WinDataString, WinNameString;
        public static int RunControl { get; set; }
        public static int PageControl { get; set; }
        public static long PostCount { get; set; }
        public static string LocationData { get; set; }

        private ObservableCollection<Post> posts;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Post> Posts
        {
            get { return posts; }
            set
            {
                posts = value;
            }
        }
        //public ObservableCollection<Post> Posts { get; set; } = new ObservableCollection<Post>();
        public PostControl()
        {
            PostControl2();
            Posts = new ObservableCollection<Post>();
        }

        public async void PostControl2()
        {
            PostCount = 0;
            //PageControl = 0;
            WinCheck = false;
            Console.WriteLine("Loading Posts");
            ///var settings = MongoClientSettings.FromConnectionString("mongodb+srv://Admin:Admin123@supercabrio.wiaa1.mongodb.net/SuperCabrio?retryWrites=true&w=majority");

            var settings = MongoClientSettings.FromConnectionString("mongodb://Admin:Admin123@supercabrio-shard-00-00.wiaa1.mongodb.net:27017,supercabrio-shard-00-01.wiaa1.mongodb.net:27017,supercabrio-shard-00-02.wiaa1.mongodb.net:27017/SuperCabrio?ssl=true&replicaSet=atlas-14ng79-shard-0&authSource=admin&retryWrites=true&w=majority");
            var client = new MongoClient(settings);
            var database = client.GetDatabase("SuperCabrio");
            var collection = database.GetCollection<BsonDocument>("Pictures");
            var collection2 = database.GetCollection<BsonDocument>("Likes");
            var sort = Builders<BsonDocument>.Sort.Descending("Votes");
            //collection.Aggregate([{$sort: { "Votes": -1} }])
            //var cursor = collection.Find(new BsonDocument()).ToCursor();
            //Posts = new ObservableCollection<Post>();

            PostCount = collection.Find(_ => true).CountDocuments();

            Console.WriteLine("NEW RUN: "+RunControl);
            //RunControl = 2;
            //GET FIRST FEW POSTS
            //for (long RunControl = 0; RunControl <= PostCount; RunControl++)

            //while (RunControl <= PostCount)
            //{
            RunControl = 0;
            //var Docs = await collection.Aggregate()
            //    .Sort("{Votes:-1}")
            //    .Skip(PageControl+RunControl)
            //    .Limit(10)
            //    .ToListAsync();
            //var Doc2 = Docs
            //    .Skip(PageControl + RunControl)
            //    .ToList();
            //SortByDescending(bson => bson["Votes"])
            //var testDocs = await collection.Aggregate()
            //.Sort("{Votes:-1}")
            var cursor = await collection.Find(_ => true).SortByDescending(bson => bson["Votes"]).Skip(PageControl+RunControl).Limit(10).ToListAsync();
            foreach (var document in cursor.AsEnumerable())
                {
                    byte[] Picture;
                    Console.WriteLine("RUNNING "+RunControl);
                    BsonElement DataSet, NameSet, VotesSet, TimeSet, TimePostedSet, idSet, LocSet, AddSet, WinDataSet, WinNameSet;
                    try
                    {
                        //CHECK TIME OF POST
                        TimeSet = document.GetElement("Time");
                        TimeString = TimeSet.ToString().Replace("Time=", "");
                        TimePostedSet = document.GetElement("TimePosted");
                        TimePostedString = TimePostedSet.ToString().Replace("TimePosted=", "");
                        idSet = document.GetElement("_id");
                        id = idSet.ToString().Replace("_id=", "");
                        DateTime TimeNow = DateTime.Now.Date;
                        var parsedDate = DateTime.Parse(TimeString);
                        int result = DateTime.Compare(TimeNow, parsedDate);
                        Console.WriteLine("ID: " + id);
                        if (result > 0)
                        {
                            PostCount = collection.Find(_ => true).CountDocuments();
                            if (WinCheck == false)
                            {
                                Console.WriteLine("OLD POST");
                                WinDataSet = document.GetElement("Data");
                                WinDataString = WinDataSet.ToString().Replace("Data=", "");
                                WinNameSet = document.GetElement("Name");
                                WinNameString = WinNameSet.ToString().Replace("Name=", "");
                                byte[] WinningPicture = Convert.FromBase64String(WinDataString);
                                MemoryStream ms = new MemoryStream(WinningPicture);
                                Console.WriteLine("SENDING EMAIL");
                                var smtpClient = new SmtpClient("smtp.gmail.com")
                                {
                                    Port = 587,
                                    Credentials = new NetworkCredential("supercabrioapp@gmail.com", "Conman00116"),
                                    EnableSsl = true,
                                };
                                var mailMessage = new MailMessage
                                {
                                    From = new MailAddress("supercabrioapp@gmail.com"),
                                    Subject = "Winning Picture",
                                    Body = "<h1>Winner: " + WinNameString + "</h1>",
                                    IsBodyHtml = true,
                                };
                                var attachment = new Attachment(ms, "Name.jpg", MediaTypeNames.Image.Jpeg);
                                //var attachment = new Attachment(testStream, "NAMe", MediaTypeNames.Image.Jpeg);
                                mailMessage.Attachments.Add(attachment);
                                mailMessage.To.Add("iphoenixdevelop@gmail.com");
                                smtpClient.Send(mailMessage);
                                WinCheck = true;
                            }
                            Console.WriteLine("Deleting");
                            var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(id));
                            collection.DeleteOne(filter);
                            try
                            {
                                database.DropCollection("Likes");
                            }
                            catch
                            {
                                Console.WriteLine("Already Deleted");
                            }
                        }
                        if (result <= 0)
                        {
                        //GET DATA
                            Console.WriteLine("CHECKPOINT 1");
                            NameSet = document.GetElement("Name");
                            NameString = NameSet.ToString().Replace("Name=", "");
                            DataSet = document.GetElement("Data");
                            DataString = DataSet.ToString().Replace("Data=", "");
                            VotesSet = document.GetElement("Votes");
                            VotesString = VotesSet.ToString().Replace("Votes=", "");
                            VotesInt = Convert.ToInt32(VotesString);
                            AddSet = document.GetElement("Address");
                            AddressString = AddSet.ToString().Replace("Address=", "");
                            LocSet = document.GetElement("Location");
                            LocationString = LocSet.ToString().Replace("Location=", "");
                            Console.WriteLine("failing");
                            if (DataString != null)
                            {
                                Picture = Convert.FromBase64String(DataString);
                                Console.WriteLine("Pawssing");

                                Posts.Add(new Post()
                                {
                                    Data = DataString,
                                    PicturePost = ImageSource.FromStream(() => new MemoryStream(Picture)),
                                    Name = NameString,
                                    Votes = VotesInt,
                                    Location = LocationString,
                                    Address = AddressString,
                                    TimePosted = TimePostedString,
                                    ID = id,
                                });
                                RunControl = RunControl + 1;
                            }
                    }
                    }
                    catch
                    {
                        Console.WriteLine("fail");
                    }
            }
            PostCount = collection.Find(_ => true).CountDocuments();
            //PostCount = collection.CountDocuments(new BsonDocument());

            //}






            //while (RunControl <= PostCount)
            //{
            //    var cursor = await collection.Find(_ => true).Skip(RunControl).Limit(1).ToListAsync();
            //    foreach (var document in cursor.AsEnumerable())
            //    {
            //        Console.WriteLine(RunControl);
            //        BsonElement DataSet, NameSet, VotesSet, TimeSet, idSet;
            //        try
            //        {
            //            //CHECK TIME OF POST
            //            TimeSet = document.GetElement("Time");
            //            TimeString = TimeSet.ToString().Replace("Time=", "");
            //            idSet = document.GetElement("_id");
            //            id = idSet.ToString().Replace("_id=", "");
            //            DateTime TimeNow = DateTime.Now.Date;
            //            var parsedDate = DateTime.Parse(TimeString);
            //            int result = DateTime.Compare(TimeNow, parsedDate);
            //            Console.WriteLine(result);
            //            if (result > 0)
            //            {
            //                Console.WriteLine("Deleting");
            //                Console.WriteLine("Current " + TimeNow);
            //                Console.WriteLine("Post " + TimeString);
            //                var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(id));
            //                collection.DeleteOne(filter);
            //            }

            //            //GET DATA
            //            NameSet = document.GetElement("Name");
            //            NameString = NameSet.ToString().Replace("Name=", "");
            //            DataSet = document.GetElement("Data");
            //            DataString = DataSet.ToString().Replace("Data=", "");
            //            VotesSet = document.GetElement("Votes");
            //            VotesString = VotesSet.ToString().Replace("Votes=", "");
            //            VotesInt = Convert.ToInt32(VotesString);
            //        }
            //        catch
            //        {
            //            Console.WriteLine("fail");
            //        }

            //        byte[] Picture = Convert.FromBase64String(DataString);

            //        Posts.Add(new Post()
            //        {
            //            Data = DataString,
            //            PicturePost = ImageSource.FromStream(() => new MemoryStream(Picture)),
            //            Name = NameString,
            //            Votes = VotesInt,
            //        });
            //        RunControl = RunControl + 1;
            //    }
            //}

            //LOAD ALL POSTS
            //using (var cursor2 = await collection.FindAsync(new BsonDocument(), new FindOptions<BsonDocument, BsonDocument>() { Sort = sort }))
            //{
            //    while (await cursor2.MoveNextAsync())
            //    {
            //        var batch = cursor2.Current;
            //        foreach (var document in batch)
            //        {
            //            BsonElement DataSet, NameSet, VotesSet;
            //            try
            //            {
            //                VotesSet = document.GetElement("Votes");
            //                VotesString = VotesSet.ToString().Replace("Votes=", "");
            //                VotesInt = Convert.ToInt32(VotesString);
            //            }
            //            catch
            //            {
            //                Console.WriteLine("fail");
            //            }

            //            NameSet = document.GetElement("Name");
            //            NameString = NameSet.ToString().Replace("Name=", "");

            //            DataSet = document.GetElement("Data");
            //            DataString = DataSet.ToString().Replace("Data=", "");
            //            byte[] Picture = Convert.FromBase64String(DataString);

            //            Posts.Add(new Post()
            //            {
            //                Data = DataString,
            //                PicturePost = ImageSource.FromStream(() => new MemoryStream(Picture)),
            //                Name = NameString,
            //                Votes = VotesInt,
            //            });
            //            RunControl = RunControl + 1;
            //            Console.WriteLine(RunControl);
            //        }
            //    }
            //}
        }
    }
}
