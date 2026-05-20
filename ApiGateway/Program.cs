var builder = WebApplication.CreateBuilder(args);
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("https://localhost:7130/swagger/v1/swagger.json", "BillingService");
        c.SwaggerEndpoint("https://localhost:7077/swagger/v1/swagger.json", "UserService");
        c.RoutePrefix = string.Empty;
    });
}

app.MapReverseProxy();
app.Run();