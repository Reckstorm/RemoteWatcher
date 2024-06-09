using System.Text.Json;
using API.Extensions;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(opt =>
{
    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    opt.Filters.Add(new AuthorizeFilter(policy));
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddAuthorizationServices(builder.Configuration);

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Application.RProcesses.List.Handler).Assembly));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

try
{
    IHttpClientFactory _clientFactory = app.Services.GetService(typeof(IHttpClientFactory)) as IHttpClientFactory;
    app.Lifetime.ApplicationStarted.Register(async () =>
    {
        var client = _clientFactory.CreateClient();
        var url = Environment.GetEnvironmentVariable("ASPNETCORE_URLS").Split(';')[0];
        string token = new TokenService(builder.Configuration).CreateToken(new Domain.User());

        var request = new HttpRequestMessage(HttpMethod.Post, $"{url}/api/Logic/start");
        request.Headers.Add("Authorization", $"Bearer {token}");
        await client.SendAsync(request);
    });
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
}

app.Run();