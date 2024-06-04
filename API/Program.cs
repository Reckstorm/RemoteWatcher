var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Application.RProcesses.List.Handler).Assembly));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

try
{
    IHttpClientFactory _clientFactory = app.Services.GetService(typeof(IHttpClientFactory)) as IHttpClientFactory;
    app.Lifetime.ApplicationStarted.Register(async () =>
    {
        var client = _clientFactory.CreateClient();
        var url = Environment.GetEnvironmentVariable("ASPNETCORE_URLS").Split(';')[0];
        var request = new HttpRequestMessage(HttpMethod.Post, $"{url}/api/Logic/start");
        await client.SendAsync(request);
    });
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
}

app.Run();