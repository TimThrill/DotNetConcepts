using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace DurableFunctions
{
    public static class DurableFunctionDemo
    {
        [FunctionName(nameof(Orchestrator))]
        public static async Task<List<int>> Orchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<int>();

            var output = await context.CallActivityAsync<int>(nameof(Increment), 1);
            outputs.Add(output);
            output = await context.CallActivityAsync<int>(nameof(Increment), output);
            outputs.Add(output);
            output = await context.CallActivityAsync<int>(nameof(Increment), output);
            outputs.Add(output);

            return outputs;
        }

        [FunctionName(nameof(Increment))]
        public static int Increment([ActivityTrigger] int input, ILogger log)
        {
            log.LogInformation($"Input: {input}");
            
            if (input % 2 == 0)
            {
                throw new ArgumentException("Input cannot be even");
            }

            return ++input;
        }

        [FunctionName(nameof(OrchestratorHttpTrigger))]
        public static async Task<HttpResponseMessage> OrchestratorHttpTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync(nameof(Orchestrator), null);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}