using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DurableFunctions
{
    public static class Orchestrator
    {
        [FunctionName(nameof(WhileLoopOrchestrator))]
        public static async Task WhileLoopOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context, ILogger logger)
        {
            logger = context.CreateReplaySafeLogger(logger);
            var timeout = context.CurrentUtcDateTime.AddSeconds(30);

            while(context.CurrentUtcDateTime < timeout)
            {
                var dueTime = context.CurrentUtcDateTime.AddSeconds(10);

                await context.CreateTimer(dueTime, CancellationToken.None);

                logger.LogInformation("I'm running");
            }

            logger.LogInformation("I'm completed");
        }

        [FunctionName(nameof(RetryOrchestrator))]
        public static async Task RetryOrchestrator(
        [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var retrySettings = context.GetInput<RetrySetting>();

            DateTime dueTime = context.CurrentUtcDateTime.AddSeconds(retrySettings.TimeoutInSeconds);

            var reconciliationContext = new ReconciliationContext
            {
                RetrySetting = retrySettings,
                StartTime = context.CurrentUtcDateTime
            };
            var reconciliationOrchestrator = context.CallSubOrchestratorAsync(nameof(ReconciliationOrchestrator), $"{nameof(ReconciliationOrchestrator)}-{context.InstanceId}", reconciliationContext);
            var timeoutTask = context.CreateTimer(dueTime, CancellationToken.None);

            var task = await Task.WhenAny(reconciliationOrchestrator, timeoutTask);

            if (task == reconciliationOrchestrator)
            {
                Console.WriteLine("Completed the reconciliation job");
            }
            else
            {
                Console.WriteLine("Terminate the orchestrator because of timeout");
            }
        }

        [FunctionName(nameof(ReconciliationOrchestrator))]
        public static async Task ReconciliationOrchestrator(
        [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            Console.WriteLine($"Excuting {nameof(ReconciliationOrchestrator)}-{context.InstanceId} at {context.CurrentUtcDateTime}");
            var reconciliationContext = context.GetInput<ReconciliationContext>();

            DateTime dueTime = context.CurrentUtcDateTime.AddSeconds(reconciliationContext.RetrySetting.RetryIntervalInSeconds);

            if (dueTime > reconciliationContext.StartTime.AddSeconds(reconciliationContext.RetrySetting.TimeoutInSeconds))
            {
                Console.WriteLine($"Completed {nameof(ReconciliationOrchestrator)}-{context.InstanceId}");
                return;
            }

            await context.CreateTimer(dueTime, CancellationToken.None);
            context.ContinueAsNew(reconciliationContext);
        }
    }
}
