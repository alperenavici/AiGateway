using AiGateway.Core.AiStrategy;
using AiGateway.Core.Interfaces;
using AiGateway.Service;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddKeyedScoped<ILanguageModelStrategy,OpenAiStrategy>("Openai");
builder.Services.AddKeyedScoped<ILanguageModelStrategy,LocalLlmStrategy>("LocalLlm");
builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(AskAiCommandHandler).Assembly));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


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
