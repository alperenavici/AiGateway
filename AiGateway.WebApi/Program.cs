using AiGateway.Core.AiStrategy;
using AiGateway.Core.Interfaces;
using AiGateway.Data.Context;
using AiGateway.Service;
using AiGateway.Service.Behaviors;
using AiGateway.WebApi.Exceptions;
using FluentValidation;
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

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
