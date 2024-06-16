using System.Net;
using API.Extensions;
using API.Middleware;
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

var currentIP = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();

if (currentIP != null && !string.IsNullOrEmpty(currentIP))
    builder.WebHost.UseUrls($"http://{currentIP}:80");

builder.Services.AddApplicationServices();
builder.Services.AddAuthorizationServices(builder.Configuration);

builder.Services.AddWindowsService();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();

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
        var url = app.Urls.FirstOrDefault();
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