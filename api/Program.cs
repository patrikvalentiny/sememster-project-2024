using System.Reflection;
using Fleck;
using Serilog;

var app = await StartupClass.Startup(args);
app.Run();

public static class StartupClass
{
    public static Task<WebApplication> Startup(string[] args)
    {
        
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
            .Build();
        
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            // .WriteTo.Console()
            .CreateLogger();


        builder.Host.UseSerilog();
        

        var app = builder.Build();
        app.UseSerilogRequestLogging();
        app.UseForwardedHeaders();
        
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.MapControllers();
        
        var server = new WebSocketServer("ws://0.0.0.0:8181");

        server.Start(socket =>
        {
            socket.OnOpen = () =>
            {
                // userService.ConnectionPool.TryAdd(socket.ConnectionInfo.Id, socket);
                Log.Information("Client connected: {Id}", socket.ConnectionInfo.Id);
            };
            socket.OnClose = () =>
            {
                Log.Information("Client disconnected: {Id}", socket.ConnectionInfo.Id);
                // userService.ConnectionPool.TryRemove(socket.ConnectionInfo.Id, out _);
                // userService.UserPool.TryRemove(socket.ConnectionInfo.Id, out _);
                // foreach (var (roomId, sockets) in userService.RoomPool)
                // {
                    // sockets.Remove(socket.ConnectionInfo.Id);
                // }
            };
            socket.OnMessage = async message =>
            {
                
                try
                {
                    await socket.Send("Hello from server");
                    // await app.InvokeClientEventHandler(services, socket, message);
                }
                catch (Exception e)
                {
                    Log.Error(e, "Error handling message");
                    // e.Handle(socket);
                }
            };
        });
        return Task.FromResult(app);
    }
}