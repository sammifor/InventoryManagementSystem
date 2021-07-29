using System;
using System.Net.Http;
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
                client.BaseAddress = new Uri(target);
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("basic", apiKey);
                //await client.PostAsync("", null);
                HttpResponseMessage msg = await client.PostAsync("", null);
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
