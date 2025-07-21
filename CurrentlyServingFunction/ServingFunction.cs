using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace ServingDisplay
{
    public static class SendServingUpdate
    {
        public class ServingUpdate
        {
            public string Number { get; set; }
        }

        [FunctionName("SendServingUpdate")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [SignalR(HubName = "servingHub")] IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<ServingUpdate>(requestBody);

            if (string.IsNullOrWhiteSpace(data?.Number))
            {
                return new BadRequestObjectResult("Missing number.");
            }

            await signalRMessages.AddAsync(new SignalRMessage
            {
                Target = "numberUpdated",
                Arguments = new[] { data.Number }
            });

            return new OkResult();
        }
    }
}
