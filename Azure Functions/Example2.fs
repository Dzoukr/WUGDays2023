namespace WUGFunctions.Examples

open System
open System.IO
open System.Net
open Azure.Storage.Blobs
open Azure.Storage.Blobs.Models
open Microsoft.Azure.Functions.Worker
open Microsoft.Azure.Functions.Worker.Http
open Microsoft.Extensions.Logging


type Example2(log:ILogger<Example2>) =
    
    [<Function("RepeatEvery5Seconds")>]
    [<QueueOutput("files-queue")>]
    member _.RepeatEvery5Seconds ([<TimerTrigger("0/5 * * * * *")>] timer:TimerInfo, ctx: FunctionContext) =
        let date = DateTimeOffset.Now
        log.LogInformation("Running at {date}", date)
        $"Hello from Azure Functions! at {date}"
        
    [<Function("ListenToQueue")>]
    [<BlobOutput("fileblobs/{rand-guid}.txt")>]
    member _.ListenToQueue ([<QueueTrigger("files-queue")>] message:string, ctx: FunctionContext) =
        log.LogInformation("Message received: {message}", message)
        $"I am storing this value + {message}"
        
    [<Function("ObserveBlobs")>]
    member _.ObserveBlobs ([<BlobTrigger(blobPath = "fileblobs/{name}")>] blob:byte[], uri:Uri, blobTrigger:string, properties:BlobProperties) =
        log.LogInformation("Just received blob {blobTrigger} on path {uri}", blobTrigger, uri)
        ()