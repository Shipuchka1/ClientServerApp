using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
   public class RESTApi
    {
        public static string ServerUrl { get; set; }
        public static string Get(string path)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ServerUrl + path);

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static  string Post<T>(string path, List<T> objects)
        {
           
            var request = (HttpWebRequest)WebRequest.Create(ServerUrl+path);


            var postData = JsonConvert.SerializeObject(objects);
            var data = Encoding.ASCII.GetBytes(postData);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = data.Length;
          //  request.TransferEncoding = "UTF-8";

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            return responseString;
        }


    }
}
