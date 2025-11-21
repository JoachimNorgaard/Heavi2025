using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HEAVI
{
    public class SlackClient
    {
        private readonly Uri _uri;
        private readonly Encoding _encoding = new UTF8Encoding();

        public SlackClient(string urlWithAccessToken)
        {
            _uri = new Uri(urlWithAccessToken);
        }

        //Post a message using simple strings  
        public string PostMessage(string text, string username = null, string channel = null)
        {
            return "";
            Payload payload = new Payload()
            {
                Channel = channel,
                Username = username,
                Text = text
            };

            return (PostMessage(payload));
        }

        //Post a message using a Payload object  
        public string PostMessage(Payload payload)
        {
            return "";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string payloadJson = JsonConvert.SerializeObject(payload);
            var content = new StringContent(payloadJson, Encoding.UTF8, "application/json");
            using (HttpClient client = new HttpClient())
            { var result = client.PostAsync(_uri, content).Result;
                return result.ToString();
            }

            //try
            //{
            //    string payloadJson = JsonConvert.SerializeObject(payload);

            //    using (WebClient client = new WebClient())
            //    {
            //        //client.Proxy = HandleProxy();
            //        NameValueCollection data = new NameValueCollection();
            //        data["payload"] = payloadJson;

            //        var response = client.UploadValues(_uri, "POST", data);

            //        //The response text is usually "ok"  
            //        string responseText = _encoding.GetString(response);
            //        return responseText;
            //    }
            //}
            //catch (Exception e)
            //{
            //    return "";
            //}
        }
        private WebProxy HandleProxy()
        {
            if (System.Environment.MachineName == "WH0299")
            {
                try
                {
                    string pw;
                    using (System.IO.StreamReader sr = new System.IO.StreamReader("C:/Users/E38862/npmConfig.txt"))
                    {
                        pw = sr.ReadToEnd();
                    }
                    WebProxy myProxy = new WebProxy("isas.danskenet.net", 80);
                    myProxy.Credentials = new NetworkCredential("e38862", pw);
                    return myProxy;
                }
                catch (Exception ex)
                {
                    return null;
                }

            }
            else return null;
        }
    }

    //This class serializes into the Json payload required by Slack Incoming WebHooks  
    public class Payload
    {
        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }
}
