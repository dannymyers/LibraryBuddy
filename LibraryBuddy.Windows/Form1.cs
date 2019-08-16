using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using LibraryBuddy.Core;
using Newtonsoft.Json;

namespace LibraryBuddy.Windows
{
    public partial class Form1 : Form
    {
        private static string BasePath = @"C:\Users\rando\Downloads\Photos";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RefreshDataSource();
        }

        private void RefreshDataSource(string selected = null)
        {
            ddlImages.DataSource = new DirectoryInfo(BasePath).GetFiles("*.jpg").Select(a => a.Name).ToList();
            if (selected != null)
            {
                ddlImages.SelectedItem = selected;
            }
        }

        private void BtnTakePicture_Click(object sender, EventArgs e)
        {
            var date = $"{DateTime.Now.ToString("yyyyddMMHHmmss")}.jpg";
            using (var capture = new VideoCapture())
            {
                using (var frame = capture.QueryFrame())
                {
                    using (var image = frame.Bitmap)
                    {
                        var imageName = Path.Combine(BasePath, date);
                        image.Save(imageName);
                    }
                }
            }
            RefreshDataSource(date);
        }

        private async void DdlImages_SelectedIndexChanged(object sender, EventArgs e)
        {
            await Process();
        }

        private async void BtnProcess_Click(object sender, EventArgs e)
        {
            await Process();
        }

        private async Task Process()
        {
            var imageName = Path.Combine(BasePath, ddlImages.SelectedItem.ToString());
            var result = await ImageProcessingService.ProcessAsync(imageName);

            var sb = new StringBuilder();
            //Open the source image and create the bitmap for the rotatated image
            var sourceImage = new Bitmap(imageName);
            var rotateImage = new Bitmap(sourceImage.Width, sourceImage.Height);
            //Set the resolution for the rotation image
            rotateImage.SetResolution(sourceImage.HorizontalResolution, sourceImage.VerticalResolution);
            //Create a graphics object
            using (Graphics graphics = Graphics.FromImage(rotateImage))
            {
                //Rotate the image
                graphics.TranslateTransform((float)sourceImage.Width / 2, (float)sourceImage.Height / 2);
                graphics.RotateTransform(90);
                graphics.TranslateTransform(-(float)sourceImage.Width / 2, -(float)sourceImage.Height / 2);
                graphics.DrawImage(sourceImage, new System.Drawing.Point(0, 0));
                graphics.CompositingQuality = CompositingQuality.HighSpeed;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.CompositingMode = CompositingMode.SourceCopy;
                //graphics.RotateTransform(90);
                foreach (var recResult in result.ExtractedTextModel.OcrResults)
                {
                    foreach (var line in recResult.Lines)
                    {
                        sb.AppendLine(line.Text + "<br>");
                        var y1 = (float)line.BoundingBox[0];
                        var x1 = (float)line.BoundingBox[1];
                        var y2 = (float)line.BoundingBox[2];
                        var x2 = (float)line.BoundingBox[3];
                        var y3 = (float)line.BoundingBox[4];
                        var x3 = (float)line.BoundingBox[5];
                        var y4 = (float)line.BoundingBox[6];
                        var x4 = (float)line.BoundingBox[7];
                        graphics.DrawLine(new Pen(Color.Aquamarine, 5), x1, y1, x2, y2);
                        graphics.DrawLine(new Pen(Color.Aquamarine, 5), x2, y2, x3, y3);
                        graphics.DrawLine(new Pen(Color.Aquamarine, 5), x3, y3, x4, y4);
                        graphics.DrawLine(new Pen(Color.Aquamarine, 5), x4, y4, x1, y1);
                    }
                }
            }
            pictureBox1.Image = rotateImage;
            webBrowser1.DocumentText = sb.ToString() + "<br>" + JsonConvert.SerializeObject(result.ExtractedTextModel.OcrResults, Formatting.Indented);
        }
    }
}
