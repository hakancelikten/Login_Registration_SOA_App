using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;

namespace Core.Utilities.Security.Captcha
{
    public static class CaptchaControl
    {
        public static async Task<bool> CheckCaptcha(string _remoteip, string _response, string _secret = null)
        {
            var postData = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("secret", _secret??"6LcvOBQlAAAAAEhAOFLYvCdhHA3ac7EiMkRfCNd0"),
                new KeyValuePair<string, string>("remoteip", _remoteip),
                new KeyValuePair<string, string>("response", _response)
            };

            var client = new HttpClient();
            var response = await client.PostAsync("https://www.google.com/recaptcha/api/siteverify", new FormUrlEncodedContent(postData));

            var o = (JObject)JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());

            return (bool)o["success"];
        }
    }
}
