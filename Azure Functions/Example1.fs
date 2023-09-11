namespace WUGFunctions.Examples

open System.Net
open Microsoft.Azure.Functions.Worker
open Microsoft.Azure.Functions.Worker.Http
open Microsoft.Extensions.Logging

type Example1(log:ILogger<Example1>) =
    
    [<Function("Test")>]
    member _.Test ([<HttpTrigger(AuthorizationLevel.Anonymous, Route = "test", Methods = [| "post" |])>] req: HttpRequestData, ctx: FunctionContext) =
        "Hello from POST test"
    
    [<Function("Index")>]
    member _.Index ([<HttpTrigger(AuthorizationLevel.Anonymous, Route = "{*any}", Methods = [| "get" |])>] req: HttpRequestData, ctx: FunctionContext) =
        task {
            let response = req.CreateResponse(HttpStatusCode.OK)
            do! response.WriteStringAsync $"Hello from {ctx.FunctionDefinition.Name} with ID {ctx.InvocationId}"
            return response
        }
