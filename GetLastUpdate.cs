using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Net;
using System.Text;

namespace AzureFunctions.DurableEntities
{
    public static class GetLastUpdate
    {
        [FunctionName("GetLastUpdate")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function)] HttpRequestMessage req,
            [DurableClient] IDurableEntityClient client, ILogger log)
        {
            var entityId = new EntityId(nameof(ChangedAt), "myCounter");
            var state = await client.ReadEntityStateAsync<ChangedAt>(entityId);
            log.LogInformation("Current State: {state}", state);
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonConvert.SerializeObject(state), Encoding.UTF8, System.Net.Mime.MediaTypeNames.Application.Json) };
        }

        [FunctionName("GetLastUpdateStarter")]
        public static async Task<HttpResponseMessage> GetLastUpdateStarter(
           [HttpTrigger(AuthorizationLevel.Function)] HttpRequestMessage req,
           [DurableClient] IDurableOrchestrationClient client, 
           ILogger log)
        {
            await client.StartNewAsync(nameof(GetLastUpdateOrchestrator));
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [FunctionName("GetLastUpdateOrchestrator")]
        public static async Task GetLastUpdateOrchestrator(
            [OrchestrationTrigger]IDurableOrchestrationContext context,
            ILogger log)
        {
            var entityId = new EntityId(nameof(ChangedAt), "myCounter");
            var result = await context.CallEntityAsync<string>(entityId, "Get");
            log.LogInformation("Current ChangedAt: {state}", result);
        }

        [FunctionName("SetLastUpdateStarter")]
        public static async Task<HttpResponseMessage> SetLastUpdateStarter(
            [HttpTrigger(AuthorizationLevel.Function)] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient client,
            ILogger log)
        {
            await client.StartNewAsync(nameof(SetLastUpdateOrchestrator));
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [FunctionName("SetLastUpdateOrchestrator")]
        public static async Task SetLastUpdateOrchestrator(
            [OrchestrationTrigger]IDurableOrchestrationContext context,
            ILogger log)
        {
            var entityId = new EntityId(nameof(ChangedAt), "myCounter");
            var result = await context.CallEntityAsync<string>(entityId, "Set");
            log.LogInformation("Current ChangedAt: {state}", result);
        }
    }
}
