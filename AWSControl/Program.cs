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
    
        [STAThread]
       
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            

        }
        

       
    }
}
