using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace PhotoOrganizer
{
    class Program
    {
        static void Main(string[] args)
        {
            var organizer = new Organizer();
            var images = organizer.GetImages();
            organizer.Organize(images);
        }
    }

    public class Organizer
    {
        // Path of the photos to be considered
        //private const string IMAGE_SOURCE_PATH = @"I:\01_PICS";
        //private const string IMAGE_SOURCE_PATH = @"I:\01_PICS\_Svg_iPhones\Gery\2019-09-03\";
        private const string IMAGE_SOURCE_PATH = @"I:\01_PICS\_Svg_iPhones\Dan\2019-09-01\";
        private const string IMAGE_DESTINATION_PATH = @"C:\tmp\01\";
        // Don't consider photos taken before this date
        private const string StartDate = "05/10/2018";
        // Don't consider photos taken after this date
        private const string EndDate = "21/03/2020";
        private IList<MetaDonnee> ImagesProperties { get; set; }
        private IList<string> ImagesEnErreur { get; set; }

        public Organizer()
        {
            this.ImagesProperties = new List<MetaDonnee>();
            this.ImagesEnErreur = new List<string>();
        }

        public IList<MetaDonnee> GetImages()
        {
            var startDate = DateTime.ParseExact(StartDate, "dd/MM/yyyy", null);
            var endDate = DateTime.ParseExact(EndDate, "dd/MM/yyyy", null);
            var dateTaken = new DateTime();

            foreach (var item in System.IO.Directory.GetFiles(IMAGE_SOURCE_PATH, "*.jpg",System.IO.SearchOption.AllDirectories))
            {                
                var metaData_ = MetadataExtractor.ImageMetadataReader.ReadMetadata(item).Where(_data => _data.Name != "JFIF").ToList();
                dateTaken = new DateTime();
                try
                {
                    dateTaken = DateTime.ParseExact(metaData_[2].Tags[6].Description, "yyyy:MM:dd HH:mm:ss", null);
                    if (dateTaken >= startDate && dateTaken <= endDate)
                    {
                        ImagesProperties.Add(new MetaDonnee() { Filename = item, Id = metaData_[2].Tags[6].Type, DateTaken = dateTaken, DateTakenShort = dateTaken.Date, Name = metaData_[2].Tags[6].Name });
                        Console.WriteLine($"ImagesProperties : {item}");
                    }
                }
                catch(Exception)
                {
                    ImagesEnErreur.Add(item);
                    Console.WriteLine($"ImagesEnErreur : {item}");
                    
                }
            }
            File.AppendAllLines($"{IMAGE_DESTINATION_PATH}\\Erreurs.txt", ImagesEnErreur.AsEnumerable());
            return ImagesProperties;
        }

        public void Organize(IList<MetaDonnee> metaDataList)
        {
            var listeParDate = metaDataList.GroupBy(_Data => _Data.DateTakenShort);

            foreach (var date in listeParDate)
            {
                Directory.CreateDirectory($"{IMAGE_DESTINATION_PATH}\\{date.Key.Date.ToString("[yyyy-MM-dd]")}");
                foreach (var file in date)
                {
                    try
                    {
                        File.Copy(file.Filename, $"{IMAGE_DESTINATION_PATH}\\{date.Key.Date.ToString("[yyyy-MM-dd]")}\\{file.Filename.Split('\\').Last()}");
                    }
                    catch(Exception ex)
                    {
                        if (ex is System.IO.IOException && ex.Message.Contains("already exists.")) { }
                        else
                            throw ex;
                    }
                }
            }
        }
    }
    public class MetaDonnee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Filename { get; set; }
        public DateTime DateTaken { get; set; }
        public DateTime DateTakenShort { get; set; }
    }
}
