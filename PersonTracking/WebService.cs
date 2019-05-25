using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PersonTracking
{
    class WebService
    {
        public static void sendDataToService(string data)
        {
            // put data to service
            String address = "http://localhost:8080/testimages/webresources/imshow";
            //Convert data to the byte
            byte[] byteData = Encoding.ASCII.GetBytes(data);
            using (var client = new System.Net.WebClient())
            {
                client.UploadData(address, "PUT", byteData);
            }

        }

        public static String getWebData()
        {

            string sURL = "http://localhost:8080/WebApplication1/webresources/TestRes";
            WebRequest wrGETURL;

            wrGETURL = WebRequest.Create(sURL);
            Stream objStream;
            objStream = wrGETURL.GetResponse().GetResponseStream();

            StreamReader objReader = new StreamReader(objStream);

            string sLine = "";

            sLine = objReader.ReadLine();

            return sLine;
        }
    }
}
