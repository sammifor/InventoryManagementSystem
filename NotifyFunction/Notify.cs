using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace NotifyFunction
{
    public static class Notify
    {
        [Function("Notify")]
        public static async Task Run([TimerTrigger("0 0 * * * *")] MyInfo myTimer, FunctionContext context)
        {
            string target = Environment.GetEnvironmentVariable("TargetServer");
            string apiKey = Environment.GetEnvironmentVariable("ApiKey");

            ILogger logger = context.GetLogger("Notify");
            using(HttpClient client = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage();
                request.Method = new HttpMethod("POST");
                request.RequestUri = new Uri(target);
                request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"apikey", apiKey }
                });

                HttpResponseMessage msg = await client.SendAsync(request);
                logger.LogInformation(msg.StatusCode.ToString());
            }
        }
    }

    public class MyInfo
    {
        public MyScheduleStatus ScheduleStatus { get; set; }

        public bool IsPastDue { get; set; }
    }

    public class MyScheduleStatus
    {
        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
