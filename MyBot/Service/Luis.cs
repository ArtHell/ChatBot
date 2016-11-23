using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace MyBot.Controllers
{
    public static class Luis
    {
        public static dynamic Analyze(string data)
        {
            var appUri =
                "https://api.projectoxford.ai/luis/v2.0/apps/175d7a41-cb15-411e-8874-c415e66ce161?subscription-key=b93a02c36f044b97a1a5f18f4fc40a44&q=";
            WebRequest req = WebRequest.Create(appUri + data);
            WebResponse resp = req.GetResponse();
            Stream stream = resp.GetResponseStream();
            StreamReader sr = new StreamReader(stream);
            string json = sr.ReadToEnd();
            sr.Close();
            return JsonConvert.DeserializeObject(json);
        }
    }
}