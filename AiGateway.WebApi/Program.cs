using AiGateway.Core.AiStrategy;
using AiGateway.Core.Interfaces;
using AiGateway.Data.Context;
using AiGateway.Service;
using AiGateway.Service.Behaviors;
using AiGateway.Service.Consumers;
using AiGateway.Service.Hubs;
using AiGateway.WebApi.Exceptions;
using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddKeyedScoped<ILanguageModelStrategy,OpenAiStrategy>("Openai");
builder.Services.AddKeyedScoped<ILanguageModelStrategy,LocalLlmStrategy>("LocalLlm");
builder.Services.AddValidatorsFromAssembly(typeof(AskAiCommandValidator).Assembly);
builder.Services.AddMediatR(cfg => 
{
    cfg.RegisterServicesFromAssembly(typeof(AskAiCommandHandler).Assembly);
    
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>)); 
});
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<AiProcessingConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
        cfg.Host("localhost", "/", h => {
            h.Username("guest");
            h.Password("guest");
        });
    });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("SignalRPolicy", policy =>
    {
        policy.SetIsOriginAllowed(_ => true) // Dikkat: Sadece yerel test için her yere izin veriyoruz. Canlıda buraya React/Next.js adresin yazılır!
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // SignalR (WebSockets) token ve cookie taşıdığı için bu ayar ZORUNLUDUR.
    });
});
builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("SignalRPolicy");
app.MapHub<AiHub>("/ai-hub");
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
