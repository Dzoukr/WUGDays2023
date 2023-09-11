namespace WUGDurableFunctions.Examples

open System
open System.Net
open Microsoft.Azure.Functions.Worker
open Microsoft.Azure.Functions.Worker.Http
open Microsoft.DurableTask
open Microsoft.DurableTask.Client
open Microsoft.Extensions.Logging

type Example2(log:ILogger<Example2>) =
    
    [<Function("PrepareBudget")>]
    member _.PrepareBudget ([<ActivityTrigger>] ctx:FunctionContext) =
        task {
            let logger = ctx.GetLogger("MyActivity")
            let amount = Random.Shared.Next(100,1000)
            logger.LogInformation("Budget for {amount} EUR", amount)
            return amount
        }
    
    [<Function("BudgetOrchestration")>]
    member _.BudgetOrchestration ([<OrchestrationTrigger()>] ctx:TaskOrchestrationContext) =
        task {
            let! budget = ctx.CallActivityAsync<int>("PrepareBudget")
            let! approved = ctx.WaitForExternalEvent<bool>("Approval")
            if approved then
                return $"Budget for {budget} EUR was approved!"
            else 
                return $"Budget for {budget} EUR was denied!"
        }
    
    [<Function("BudgetApproval")>]
    member _.BudgetApproval ([<QueueTrigger("approval-queue")>] message:string, [<DurableClient>] client:DurableTaskClient, ctx: FunctionContext) =
        task {
            let parts = message.Split("|")
            let instanceId = parts[0]
            let isApproved = parts[1] |> Convert.ToBoolean
            log.LogInformation("Got approval for {instanceId} with decision {isApproved}", instanceId, isApproved)
            do! client.RaiseEventAsync(instanceId, "Approval", isApproved)
        }
         
    [<Function("StartBudgetOrchestration")>]
    member _.StartBudgetOrchestration ([<HttpTrigger(AuthorizationLevel.Anonymous)>] req: HttpRequestData, [<DurableClient>] client:DurableTaskClient, ctx: FunctionContext) =
        task {
            let! instanceId = client.ScheduleNewOrchestrationInstanceAsync("BudgetOrchestration")
            return client.CreateCheckStatusResponse(req, instanceId)
        }
    
