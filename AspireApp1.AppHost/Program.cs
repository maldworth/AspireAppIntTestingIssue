using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var gotenberg = builder.AddContainer("gotenberg", "gotenberg/gotenberg", "8")
    .WithHttpEndpoint(name: "gotenberg", port: 3000, targetPort: 3000);

var apiService = builder.AddProject<Projects.AspireApp1_ApiService>("apiservice")
    .WithReference(gotenberg.GetEndpoint("gotenberg"));

builder.AddProject<Projects.AspireApp1_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
