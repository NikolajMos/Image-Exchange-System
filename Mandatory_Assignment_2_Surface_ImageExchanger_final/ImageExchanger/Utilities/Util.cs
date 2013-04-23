using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace ImageExchanger.Utilities
{
    class Util
    {
        public static BitmapImage LoadImage(string base64ImageData)
        {

            byte[] bytes = System.Convert.FromBase64String(base64ImageData);
            MemoryStream byteStream = new MemoryStream(bytes);

            //BitmapImage watermark = new BitmapImage();
            //string watermark = @"C:\Users\jgaBachmann\Desktop\Surface\Surface\java_logo.jpg";

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = byteStream;
            image.EndInit();
            return image;
        }
        public static string localIPAddress()
        {
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                }
            }
            return localIP;

        }
        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}
