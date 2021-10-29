using DLFaceScan.model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DLFaceScan.service
{
    class CognitiveService
    {
        private static readonly string API_KEY_1 = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
 
        private static readonly string ENDPOINT_URL = "https://faceapidjimba.cognitiveservices.azure.com" + "/face/v1.0/";

        public static async Task<FaceDetectResult> FaceDetect(Stream imageStream)
        {

            if (imageStream == null)
            {
                return null;
            }

            var url = ENDPOINT_URL + "detect" + "?returnFaceAttributes=age,gender";

            using (var webClient = new WebClient())
            {
                try
                {
                    webClient.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";
                    webClient.Headers.Add("Ocp-Apim-Subscription-Key", API_KEY_1);

                    var data = ReadStream(imageStream);

                    var result = await Task.Run(() => webClient.UploadData(url, data));

                    //var result = webClient.UploadData(url, data);

                    if(result == null)
                    {
                        return null;
                    }

                    string json = Encoding.UTF8.GetString(result, 0, result.Length);

                    var faceResult = Newtonsoft.Json.JsonConvert.DeserializeObject<FaceDetectResult[]>(json);

                    if(faceResult.Length >= 0)
                    {
                        return faceResult[0];
                    }

                    Console.WriteLine("Réponse Ok : " + json);
                }

                catch (Exception ex)
                {
                    Console.WriteLine("Exception webClient :  "+ ex.Message);
                }

                return null;
            }
        }

        private static byte[] ReadStream(Stream input)
        {
            BinaryReader b = new BinaryReader(input);
            byte[] data = b.ReadBytes((int)input.Length);
            return data;
        }
    }
}
