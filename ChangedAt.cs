using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace AzureFunctions.DurableEntities
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ChangedAt
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        public void Set()
        {
            this.Value = DateTime.UtcNow.ToString();
        }

        public Task Reset()
        {
            this.Value = string.Empty;
            return Task.CompletedTask;
        }

        public Task<string> Get()
        {
            return Task.FromResult(this.Value);
        }

        public void Delete()
        {
            Entity.Current.DeleteState();
        }

        [FunctionName(nameof(ChangedAt))]
        public static Task Run([EntityTrigger] IDurableEntityContext ctx)
            => ctx.DispatchAsync<ChangedAt>();
    }
}
