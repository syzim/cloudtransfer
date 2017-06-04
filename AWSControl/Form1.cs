﻿using Amazon.S3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Amazon.S3.Model;
using MetadataExtractor;
using System.Text.RegularExpressions;
using ImageResizer;
using System.Threading.Tasks;
using Amazon.S3.IO;

namespace AWSControl
{
    public partial class Form1 : Form
    {
         picDb db = new picDb();
         AmazonS3Client client = new AmazonS3Client();
         string myBucket = "usa";
         List<Size> sizeList = new List<Size>();
        static List<int> folderIndex = new List<int>();


        string baseUrl = "https://s3-ap-southeast-2.amazonaws.com/";




        public Form1()
        {

            InitializeComponent();
            pictureBox2.ImageLocation = PhotoData.getCurrent.path;

            button1.Enabled = false;

            button1.Enabled = true;

            refreshForm();
        }

      
        private void refreshForm()
        {

            label1.Text = $"{PhotoData.Index + 1} of {PhotoData.Images.Count}";
            label2.Text = PhotoData.getCurrent.lat.ToString();
            label4.Text = PhotoData.getCurrent.lng.ToString();
            label3.Text = PhotoData.getCurrent.taken.ToString();
            label5.Text = PhotoData.getCurrent.id;
            stateLabel.Text = PhotoData.getCurrent.path.Substring(19,2); ;
            if(checkOnline() == true)
            {
                label6.Text = "Online";
                label6.BackColor = System.Drawing.Color.LightGreen;
                uploadBtn.Enabled = false;
            }
            else
            {
                label6.Text = "Offline";
                label6.BackColor = System.Drawing.Color.Red;
                uploadBtn.Enabled = true;
            }
        }

        private void next_Click(object sender, EventArgs e)
        {
            var i = PhotoData.Index;
            if (i < (PhotoData.Images.Count()-1))
            {
                PhotoData.Index = PhotoData.Index + 1;
                pictureBox2.ImageLocation = PhotoData.getCurrent.path;
            }
            else
            {
                PhotoData.Index = 0;
                pictureBox2.ImageLocation = PhotoData.getCurrent.path;

            }
            refreshForm();
            


        }

        static class PhotoData
        {
            private static List<MetaImage> _images = ImageSearch();
            private static int _index = 0;
            public static List<MetaImage> Images
            {
                get { return _images; }
            }


            public static int Index
            {
                get { return _index; }
                set { _index = value; }

            }
            public static MetaImage getCurrent
            {
                get { return _images[_index]; }
            }


        }

        private async void uploadBtn_Click(object sender, EventArgs e)
        {
            var pic = PhotoData.getCurrent;
            await uploadImage(pic,db);
            

        }
        private Boolean  checkOnline()
        {
            MetaImage i = PhotoData.getCurrent;
            var e = db.MetaImages.Any(x => x.id == i.id);
            S3FileInfo file = new S3FileInfo(client, "usasmall", i.id + ".jpg");
            if(e && file.Exists == true)
            {
                return true;
            }
            return false;
        }
        private async Task uploadImage(MetaImage pic, picDb db)

        {

            Dictionary<string, string> versions = new Dictionary<string, string>();

            versions.Add("small", "width=267&height=200crop=auto&autorotate=true&bgcolor=black");
            versions.Add("medium", "width=667&height=500&autorotate=true&bgcolor=black"); //Crop to square thumbnail
            versions.Add("large", "width=1333&height=1000&autorotate=true&bgcolor=black");
            versions.Add("tiny", "width=150&height=113crop=auto&autorotate=true&bgcolor=black");
            versions.Add("nano", "width=80&height=60crop=auto&autorotate=true&bgcolor=black"); //Crop to square thumbnail
            versions.Add("xtranano", "width=30&height=23crop=auto&autorotate=true&bgcolor=black");

            string rpath = "E:\\Photos USA Dump\\";
            string state = pic.path.Substring(19, 2);

            foreach (string size in versions.Keys)
            {



                //get bucketname from id
                var bucketName = myBucket + size;
                pic.bucketname = myBucket;
                var column = size.Substring(0, 1);

                var p = "url" + column;
                var prop = pic.GetType().GetProperty(p);
                prop.SetValue(pic, (baseUrl + myBucket + size + "/" + pic.id + ".jpg"), null);


                var folder = rpath + state + "\\" + size;
                if (!System.IO.Directory.Exists(folder)) System.IO.Directory.CreateDirectory(folder);

                //resize to image;
                var y = ImageBuilder.Current.Build(new ImageJob(pic.path, folder + "\\" + pic.id, new Instructions(versions[size]), false, true));



                try
                {


                    PutObjectRequest putImage = new PutObjectRequest
                    {
                        BucketName = bucketName,
                        Key = pic.id + ".jpg",
                        FilePath = folder + "\\" + pic.id + ".jpg",
                        ContentType = "image/jpg"


                    };

                     await Task.Run(() => client.PutObject(putImage));
                    System.Diagnostics.Debug.WriteLine("Image Uploaded");




                }
                catch (AmazonS3Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("error uploading photo to bucket " + bucketName + ":" + e.Message);
                    
                }
            }
            db.MetaImages.Add(pic);
            db.SaveChanges();
            
            
        
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var i = PhotoData.Index;
            if (i > 0)
            {
                PhotoData.Index = PhotoData.Index - 1;
                pictureBox2.ImageLocation = PhotoData.getCurrent.path;
            }
            else
            {
                PhotoData.Index = PhotoData.Images.Count -1;
                pictureBox2.ImageLocation = PhotoData.getCurrent.path;

            }
            refreshForm();

        }
        private static List<MetaImage> ImageSearch()
        {
           

            string path = "E:\\Photos USA Dump";
            List<MetaImage> images = new List<MetaImage>();

            int counter = 0;


            foreach (string folder in System.IO.Directory.GetDirectories(path))
            {
                var dir = folder.Substring(19).ToLower();

                foreach (string file in System.IO.Directory.GetFiles(folder))
                {
                    try
                    {
                        counter++;
                        MetaImage i = new MetaImage();
                        i.id = file.Substring(22);
                        i.id = i.id.Split('.')[0];
                        IEnumerable<MetadataExtractor.Directory> directories = ImageMetadataReader.ReadMetadata(file);
                        var ifdDir = directories.OfType<MetadataExtractor.Formats.Exif.ExifIfd0Directory>().FirstOrDefault();
                        var gpsDirectory = directories.OfType<MetadataExtractor.Formats.Exif.GpsDirectory>().FirstOrDefault();


                        var lat = gpsDirectory?.GetGeoLocation();


                        if (lat != null)
                        {
                            i.lat = lat.Latitude;
                            i.lng = lat.Longitude;
                        };


                        var dateTime = ifdDir.GetDescription(MetadataExtractor.Formats.Exif.ExifDirectoryBase.TagDateTime);
                        var w = ifdDir.GetDescription(MetadataExtractor.Formats.Exif.ExifDirectoryBase.TagImageWidth);
                        var h = ifdDir.GetDescription(MetadataExtractor.Formats.Exif.ExifDirectoryBase.TagImageHeight);

                        var width = Convert.ToInt32(Regex.Replace(w, "[^0-9]+", string.Empty));
                        var height = Convert.ToInt32(Regex.Replace(h, "[^0-9]+", string.Empty));


                        i.path = file;

                        i.width = width;
                        i.height = height;
                        string format = "yyyy:MM:dd HH:mm:ss";

                        i.taken = DateTime.ParseExact(dateTime, format, null);
                        images.Add(i);

                    }



                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine($"an error occured{file} Error: {e.Message}");

                        continue;

                    }

                }
                folderIndex.Add(counter);
            }
            return images;

        }

        private void nextState_Click(object sender, EventArgs e)
        {
            var i = PhotoData.Index;
            int nextState = folderIndex.First(x => x > i);
            if (nextState == PhotoData.Images.Count)
            {
                PhotoData.Index = 0;
                pictureBox2.ImageLocation = PhotoData.getCurrent.path;
                refreshForm();
            }
            else
            {
                PhotoData.Index = nextState;
                pictureBox2.ImageLocation = PhotoData.getCurrent.path;
                refreshForm();
            }
        }
    }
}
