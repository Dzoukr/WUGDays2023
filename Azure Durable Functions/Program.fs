open Microsoft.Azure.Functions.Worker
open Microsoft.Azure.Functions.Worker.Extensions.DurableTask
open Microsoft.Extensions.Hosting

// https://github.com/microsoft/durabletask-dotnet/issues/99
let private configureApplicationBuilder =
    fun (builder: IFunctionsWorkerApplicationBuilder) ->
        DurableTaskExtensionStartup().Configure(builder)

[<EntryPoint>]
let main args =
    let host =
        HostBuilder()
            .ConfigureFunctionsWorkerDefaults(configureApplicationBuilder)
            .Build()

    host.Run()
    0
