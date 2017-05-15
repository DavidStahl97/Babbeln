using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace VoIPApp.Common
{
    public class ProfileImageLoader
    {
        public async static Task<BitmapImage> LoadProfileImage(ObjectId id)
        {
            var image = new BitmapImage();
            int BytesToRead = 100;

            string url = ConfigurationManager.AppSettings["ProfilePictureURL"].ToString() + id.ToString() + ".jpg";
            WebRequest request = WebRequest.Create(new Uri(url, UriKind.Absolute));
            request.Timeout = -1;
            WebResponse response = request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            BinaryReader reader = new BinaryReader(responseStream);
            MemoryStream memoryStream = new MemoryStream();

            byte[] bytebuffer = new byte[BytesToRead];
            await Task.Run(() =>
            {
                int bytesRead = reader.Read(bytebuffer, 0, BytesToRead);

                while (bytesRead > 0)
                {
                    memoryStream.Write(bytebuffer, 0, bytesRead);
                    bytesRead = reader.Read(bytebuffer, 0, BytesToRead);
                }

                image.BeginInit();
                memoryStream.Seek(0, SeekOrigin.Begin);

                image.StreamSource = memoryStream;
                image.EndInit();
            });

            return image;
        }
    }
}
