using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoOrganizer
{
    class Program
    {
        static void Main(string[] args)
        {
            var organizer = new Organizer();
            organizer.GetImages();
        }
    }

    public class Organizer
    {
        private const string IMAGE_PATH = @"C:\Users\hinom\OneDrive\Images";
        private IDictionary<string, List<MetaDonnee>> ImagesProperties { get; set; }

        public Organizer()
        {
            this.ImagesProperties = new Dictionary<string, List<MetaDonnee>>();
        }

        public void GetImages()
        {
            foreach (var item in System.IO.Directory.GetFiles(IMAGE_PATH, "*.jpg",System.IO.SearchOption.AllDirectories))
            {
                var image = new Bitmap(item);
                var metaData = image.PropertyItems.Where(_Data => _Data.Type == 2);
                foreach(var donnee in metaData) { Console.WriteLine(new MetaDonnee(donnee.Id, donnee.Value, donnee.Type).ToString()); }
                ImagesProperties.Add(item, metaData.Select(_Data => new  MetaDonnee(_Data.Id, _Data.Value, _Data.Type)).ToList());
            }
        }

        public class MetaDonnee
        {
            public int Id { get; set; }
            public string Value { get; set; }
            public short  Type { get; set; }

            public MetaDonnee(int id, byte[] value, short type)
            {
                Id = id;
                Value = new ASCIIEncoding().GetString(value);
                Type = type;
            }

            public override string ToString()
            {
                return $"Id:{Id};Value:{Value};Type:{Type}";
            }
        }
    }
}
