using GrpcServicePiter;
using GrpcServicePiter.Interceptors;
using GrpcServicePiter.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// ������ �����������
string connStr = Settings.GetConnectString(); // "Host=localhost;Username=postgres;Password=12345;Database=PiterDB5"; // ������ �������: "Server=localhost;port=643;Database=PiterDB3;Username=postgres;Password=12345;"
// ��������� �������� ApplicationContext � �������� ������� � ����������
builder.Services.AddDbContext<ApplicationContext>(options => options.UseNpgsql(connStr));

// 
var serviceProvider = builder.Services.BuildServiceProvider();
var logger = serviceProvider.GetService<ILogger<LoggingInterceptor>>();

// Add services to the container.
builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add(typeof(LoggingInterceptor), logger);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<AccountService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
