using System.Net;
using Fleck;
using Serilog;
using WebSocketProxy;
using Host = WebSocketProxy.Host;

StartupClass.Startup(args);
// var tcp = 
// tcp.Start();
// app.Run();

public static class StartupClass
{
    public static  void  Startup(string[] args)
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
        // builder.WebHost.UseUrls("http://*:5000");

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
        app.UseWebSockets();
        
        
        // var server = new WebSocketServer("ws://0.0.0.0:8181");
        // server.Start(socket =>
        // {
        //     socket.OnOpen = () =>
        //     {
        //         // userService.ConnectionPool.TryAdd(socket.ConnectionInfo.Id, socket);
        //         Log.Information("Client connected: {Id}", socket.ConnectionInfo.Id);
        //     };
        //     socket.OnClose = () =>
        //     {
        //         Log.Information("Client disconnected: {Id}", socket.ConnectionInfo.Id);
        //         // userService.ConnectionPool.TryRemove(socket.ConnectionInfo.Id, out _);
        //         // userService.UserPool.TryRemove(socket.ConnectionInfo.Id, out _);
        //         // foreach (var (roomId, sockets) in userService.RoomPool)
        //         // {
        //         // sockets.Remove(socket.ConnectionInfo.Id);
        //         // }
        //     };
        //     socket.OnMessage = async message =>
        //     {
        //         
        //         try
        //         {
        //             await socket.Send("Hello from server");
        //             // await app.InvokeClientEventHandler(services, socket, message);
        //         }
        //         catch (Exception e)
        //         {
        //             Log.Error(e, "Error handling message");
        //             // e.Handle(socket);
        //         }
        //     };
        // });
        
        
        TcpProxyConfiguration proxyConfiguration = new TcpProxyConfiguration()
        {
            PublicHost = new Host()
            {
                IpAddress = IPAddress.Parse("0.0.0.0"),
                Port = 8080
            },
            HttpHost = new Host()
            {
                IpAddress = IPAddress.Loopback,
                Port = 5000
            },
            WebSocketHost = new Host()
            {

                IpAddress = IPAddress.Loopback,
                Port = 8181
            }
        };

        using var websocketServer = new WebSocketServer("ws://0.0.0.0:8181");
        using var tcpProxy = new TcpProxyServer(proxyConfiguration);
        // Task.Run(() => app.Run());

        // Initialize Fleck
        websocketServer.Start(connection =>
        {
            connection.OnOpen = () => Console.WriteLine("Connection on open");
            connection.OnClose = () => Console.WriteLine("Connection on close");
            connection.OnMessage = message => Console.WriteLine("Message: " + message);
        });
        // Initialize the proxy
        tcpProxy.Start();
        app.Run();
        // return Task.FromResult(tcpProxy);
        // Console.ReadKey();
    }
}