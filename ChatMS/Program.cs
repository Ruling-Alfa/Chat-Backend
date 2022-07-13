using Alfa.ChatMS;

var builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigureServicesExt();
var app = builder.Build();

// app.MapGet("/", () => "Hello World!");
app.UsePipeline();
app.Run();
