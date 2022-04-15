using System;
using MongoDB.Bson.Serialization.Attributes;
using Xamarin.Forms;

namespace SuperCabrio
{
    public class Post
    {
        [BsonElement("Data")]
        public string Data { get; set; }

        public ImageSource PicturePost { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("Votes")]
        public int Votes { get; set; }

        [BsonElement("Location")]
        public string Location { get; set; }

        [BsonElement("Address")]
        public string Address { get; set; }

        [BsonElement("TimePosted")]
        public string TimePosted { get; set; }

        [BsonElement("ID")]
        public string ID { get; set; }
    }
}
