namespace WUGDurableFunctions.Examples

open System.Net
open Microsoft.Azure.Functions.Worker
open Microsoft.Azure.Functions.Worker.Http
open Microsoft.DurableTask
open Microsoft.DurableTask.Client
open Microsoft.Extensions.Logging

type Example1(log:ILogger<Example1>) =
    
    [<Function("MyActivity")>]
    member _.MyActivity ([<ActivityTrigger>] myParam:string, ctx:FunctionContext) =
        task {
            log.LogInformation("Running MyActivity with param {name}", myParam);
            return $"MyActivity with param {myParam} finished"
        }
    
    [<Function("MyOrchestration")>]
    member _.MyOrchestration ([<OrchestrationTrigger()>] ctx:TaskOrchestrationContext) =
        task {
            let! result1 = ctx.CallActivityAsync<string>("MyActivity", "Param A")
            let! result2 = ctx.CallActivityAsync<string>("MyActivity", "Param B")
            let! result3 = ctx.CallActivityAsync<string>("MyActivity", "Param C")
            return $"MyOrchestration finished with results {result1}, {result2}, {result3}"
        }
     
    [<Function("StartMyOrchestration")>]
    member _.StartMyOrchestration ([<HttpTrigger(AuthorizationLevel.Anonymous)>] req: HttpRequestData, [<DurableClient>] client:DurableTaskClient, ctx: FunctionContext) =
        task {
            let! instanceId = client.ScheduleNewOrchestrationInstanceAsync("MyOrchestration")
            return client.CreateCheckStatusResponse(req, instanceId)
        }
    
