using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;

namespace ASCIIArt
{
    public class ASCIIArt
    {
        private static readonly string[] ASCII_CHARACTER = { "#", "$", "@", "%", "=", "+", "*", ":", "-", ".", "&nbsp;" };
        private static readonly string[] IMAGE_FILE = { ".PNG", ".JPG", ".JPEG", ".BMP" };

        public static bool isImageExtension(string path)
        {
            return Array.IndexOf(IMAGE_FILE, Path.GetExtension(path).ToUpperInvariant()) != -1;
        }

        public static string convertASCII(Bitmap image)
        {
            StringBuilder asciiArtBuilder = new StringBuilder();
            Image<Gray, byte> grayImage = new Image<Gray, byte>(image);
            for (int height = 0; height < grayImage.Height; height += 2)
            {
                for (int width = 0; width < grayImage.Width; ++width)
                {
                    byte gray = grayImage.Data[height, width, 0];
                    int index = gray * (ASCII_CHARACTER.Length - 1) / 255;
                    asciiArtBuilder.Append(ASCII_CHARACTER[index]);
                }
                asciiArtBuilder.Append("<br />");
            }
            return asciiArtBuilder.ToString();
        }

        public static byte averageRGB(byte red, byte green, byte blue)
        {
            return (byte)((red + green + blue) / 3D);
        }

        public static Bitmap resizeImage(Bitmap bmp, int width)
        {
            int height = (int)Math.Ceiling((double)bmp.Height * width / bmp.Width);
            Bitmap ret = new Bitmap(width, height);
            Graphics graphic = Graphics.FromImage(ret);
            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.DrawImage(bmp, 0, 0, width, height);
            return ret;
        }
    }
}