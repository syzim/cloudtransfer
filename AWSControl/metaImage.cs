using System;
using System.Data.Entity;


namespace AWSControl
{
    public class MetaImage
    {
        public string path { get; set; }
        public string id { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public double lat { get; set; } 
        public double lng { get; set; }
        public DateTime taken { get; set; }
        public string bucketname { get; set; }
        public string urlo { get; set; }
        public string urls { get; set; }
        public string urlm { get; set; }
        public string urll { get; set; }
        public string urlt { get; set; }
        public string urln { get; set; }
        public string urlx { get; set; }




    }
    public class picDb : DbContext
    {
        public  DbSet<MetaImage> MetaImages {get;set;}
    }
    public class Size
    {
        public string size { get; set; }
        public int maxWidth { get; set; }
    }
}
