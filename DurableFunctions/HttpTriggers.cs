using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace DurableFunctions
{
    public static class HttpTriggers
    {
        [FunctionName(nameof(OrchestratorHttpTrigger))]
        public static async Task<IActionResult> OrchestratorHttpTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "start-orchestrator")] HttpRequest req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            req.Query.TryGetValue("timeoutInSeconds", out var timeoutInSeconds);
            req.Query.TryGetValue("retryIntervalInSeconds", out var retryIntervalInSeconds);

            var intTimeoutInSeconds = timeoutInSeconds.Count == 0 ? 30 : int.Parse(timeoutInSeconds[0]);
            var intRetryIntervalInSeconds = retryIntervalInSeconds.Count == 0 ? 5 : int.Parse(retryIntervalInSeconds[0]);

            // Function input comes from the request content.
            var instanceId = Guid.NewGuid();
            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            var isTimeout = await starter.StartNewAsync(nameof(Orchestrator.RetryOrchestrator), instanceId.ToString(), new RetrySetting { TimeoutInSeconds = intTimeoutInSeconds, RetryIntervalInSeconds = intRetryIntervalInSeconds });

            return new OkObjectResult(new
            {
                InstanceId = instanceId,
                RetryIntervalInSeconds = intRetryIntervalInSeconds,
                TimeoutInSeconds = intTimeoutInSeconds
            });
        }
    }
}