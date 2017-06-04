using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using MetadataExtractor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ImageResizer;
using System.IO;

namespace AWSControl
{
     class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>


       


        //1333x1000
        //667x500
        //267x 200
        // Size o = new Size { size = "original", maxWidth = 3 };


        
    

        [STAThread]
       
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
     
            //  versions.Add("small", "width=267&height=200&format=jpg");



            Application.Run(new Form1());

            

        }


        //public static void uploadImage(MetaImage pic,picDb db)
       




       
    }
}
