using System.Net;
using System.Reflection;
using api.Mqtt;
using api.Utils;
using Fleck;
using infrastructure;
using infrastructure.Helpers;
using MediatR;
using MQTTnet;
using Serilog;
using service;
using WebSocketProxy;
using Host = WebSocketProxy.Host;

namespace api;

public static class StartupClass
{
    public static WebApplication Startup(string[] args)
    {
        SetupLogger();

        var builder = WebApplication.CreateBuilder(args);

        builder.AddServices();

        var app = builder.Build();

        app.SetupApp();

        var task = Task.FromResult(CreateProxy());
        var tcpProxy = task.Result;
        tcpProxy.Start();
        Task.FromResult(StartWebSocketServer(app.Services));

        // MQTT
        _ = app.Services.GetRequiredService<MqttDevicesClient>().CommunicateWithBroker();
        _ = app.Services.GetRequiredService<MqttDeviceDataClient>().CommunicateWithBroker();
        _ = app.Services.GetRequiredService<MqttDeviceMotorPosition>().CommunicateWithBroker();

        // Initialize the proxy as task
        return app;
    }

    private static void SetupLogger()
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
    }

    private static void AddServices(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog();

        var conn = Environment.GetEnvironmentVariable("ASPNETCORE_ConnectionStrings__Postgres") ??
                   throw new Exception("Connection string not found");
        builder.Services.AddNpgsqlDataSource(Utilities.FormatConnectionString(conn),
            dataSourceBuilder =>
                dataSourceBuilder.EnableParameterLogging());
        builder.Services.AddSingleton<IWebSocketStateService, WebSocketStateService>();
        builder.Services.AddSingleton<IDeviceService, DeviceService>();
        builder.Services.AddSingleton<DeviceRepository>();
        builder.Services.AddSingleton<MqttDevicesClient>();
        builder.Services.AddSingleton<MqttFactory>();
        builder.Services.AddSingleton<MqttDeviceDataClient>();
        builder.Services.AddSingleton<MqttDeviceMotorPosition>();
        builder.Services.AddSingleton<IDataService, DataService>();
        builder.Services.AddSingleton<DataRepository>();
        builder.Services.AddSingleton<ConfigRepository>();
        builder.Services.AddSingleton<IMotorService, MotorService>();
        builder.Services.AddSingleton<MotorRepository>();
        
        var types = Assembly.GetExecutingAssembly();
        builder.Services.AddMediatR(cfg => { cfg.RegisterServicesFromAssembly(types); });
        WsHelper.InitBaseDtos(types);

        // Add services to the container.
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.WebHost.UseUrls("http://*:5000");
    }

    private static void SetupApp(this WebApplication app)
    {
        app.UseSerilogRequestLogging();
        app.UseForwardedHeaders();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapControllers();
        string[] allowedOrigins = app.Environment.IsDevelopment()
            ? ["http://localhost:4200", "http://localhost:5000"]
            : ["https://climate-ctrl.web.app", "https://climate-ctrl.firebaseapp.com", "https://climate-ctrl-staging.web.app/", "https://climate-ctrl-staging.firebaseapp.com"];

        app.UseCors(corsPolicyBuilder => corsPolicyBuilder.WithOrigins(allowedOrigins)
            .AllowAnyMethod().AllowAnyHeader());
    }

    private static TcpProxyServer CreateProxy()
    {
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
        return new TcpProxyServer(proxyConfiguration);
    }

    private static IWebSocketServer StartWebSocketServer(IServiceProvider services)
    {
        var websocketServer = new WebSocketServer("ws://0.0.0.0:8181");
        var webSocketStateService = services.GetRequiredService<IWebSocketStateService>();
        var mediatr = services.GetRequiredService<IMediator>();
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
                webSocketStateService.CloseSocket(socket);
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
                    await e.Handle(socket);
                }
            };
        });
        return websocketServer;
    }
}