using MobileBanking.Application;
using MobileBanking.Exceptions;
using MobileBanking.Logger.Services;
using MobileBanking.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddApplicationServices();

builder.Services.AddScoped<ApiKeyAuthFilter>();

builder.Services.AddSingleton<ILoggerService, LoggerService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseExceptionHandler();
app.UseMiddleware<TenantMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();
