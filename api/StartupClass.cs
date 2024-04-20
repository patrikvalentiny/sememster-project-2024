using System.Net;
using System.Reflection;
using api.Mqtt;
using api.Utils;
using Fleck;
using infrastructure;
using infrastructure.Helpers;
using MediatR;
using Serilog;
using service;
using WebSocketProxy;
using Host = WebSocketProxy.Host;

namespace api;

public static class StartupClass
{
    public static void Startup(string[] args)
    {
        //setup logger
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile(
                $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json",
                true)
            .Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            // .WriteTo.Console()
            .CreateLogger();


        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseSerilog();
        
        var conn = Environment.GetEnvironmentVariable("ASPNETCORE_ConnectionStrings__Postgres") ??
                     throw new Exception("Connection string not found");
        builder.Services.AddNpgsqlDataSource(Utilities.FormatConnectionString(conn),
            dataSourceBuilder => 
                dataSourceBuilder.EnableParameterLogging());
        builder.Services.AddSingleton<WebSocketStateService>();
        builder.Services.AddSingleton<DeviceService>();
        builder.Services.AddSingleton<DeviceRepository>();
        builder.Services.AddSingleton<MqttClientService>();
        var types = Assembly.GetExecutingAssembly();
        builder.Services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(types);
        });
        
        WsHelper.InitBaseDtos(types);
        
        // Add services to the container.
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.WebHost.UseUrls("http://*:5000");

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

        var proxyConfiguration = new TcpProxyConfiguration
        {
            PublicHost = new Host
            {
                IpAddress = IPAddress.Parse("0.0.0.0"),
                Port = 8080
            },
            HttpHost = new Host
            {
                IpAddress = IPAddress.Loopback,
                Port = 5000
            },
            WebSocketHost = new Host
            {
                IpAddress = IPAddress.Loopback,
                Port = 8181
            }
        };
        
        
        using var websocketServer = new WebSocketServer("ws://0.0.0.0:8181");
        using var tcpProxy = new TcpProxyServer(proxyConfiguration);
        
        var webSocketStateService = app.Services.GetRequiredService<WebSocketStateService>();
        var mediatr = app.Services.GetRequiredService<IMediator>();
        // Initialize Fleck
        websocketServer.Start(socket =>
        {
            socket.OnOpen = () =>
            {
                Log.Debug("Client connected: {Id}", socket.ConnectionInfo.Id);
                webSocketStateService.Connections.TryAdd(socket.ConnectionInfo.Id, socket);
            };
            socket.OnClose = () =>
            {
                Log.Debug("Client disconnected: {Id}", socket.ConnectionInfo.Id); 
                webSocketStateService.Connections.TryRemove(socket.ConnectionInfo.Id, out _);
            };
            socket.OnMessage = async message =>
            {
                Log.Debug("Message received: {Message}", message);
                try
                {
                    await socket.InvokeBaseDtoHandler(message, mediatr);
                }
                catch (Exception e)
                {
                    e.Handle(socket);
                }
            };
        });

        _ = app.Services.GetRequiredService<MqttClientService>().CommunicateWithBroker();
        var allowedOrigins = app.Environment.IsDevelopment()
            ? new List<string> { "http://localhost:4200", "http://localhost:5000" }
            : new List<string> {  };
        app.UseCors(corsPolicyBuilder => corsPolicyBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        
        // Initialize the proxy
        tcpProxy.Start();

        app.Run();
    }
}