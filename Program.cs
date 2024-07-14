using Serilog;
using ModelSaver;
using FileSaver;

class Program {
    static IModelSaver saver;
    static void Main(string[] args) 
    {
        var builder = WebApplication.CreateBuilder(args);
        using var log = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("./log.txt")
            .CreateLogger();

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        saver = new ModelToFile("./model.onnx");

        //app.UseHttpsRedirection();

        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        app.MapGet("/weatherforecast", () =>
        {
            var forecast =  Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
                .ToArray();
            return forecast;
        })
        .WithName("GetWeatherForecast")
        .WithOpenApi();

        app.MapGet("/infer", () => 
        {
            var inferenceResult = new Dictionary<String, String>()
            {
                {"model", "yolo"},
                {"time", "one"}
            };
            return inferenceResult;
        })
        .WithName("Infer")
        .WithOpenApi();

        app.MapPost("/uploadmodel", () =>
                {
                  // ingest and save model  
                })
        .WithName("UploadModel")
        .WithOpenApi();

        app.MapGet("/deletemodel", () =>
                {
                  // Delete model from storage 
                })
        .WithName("DeleteModel")
        .WithOpenApi();

        log.Information("Starting App...");
        app.Run();


    }
        record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
        {
            public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
        }
}
