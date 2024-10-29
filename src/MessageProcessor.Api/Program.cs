using MessageProcessor.Core.Data;
using MessageProcessor.Core.Services.MessageProcessing;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<YourDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register message processors
builder.Services.AddScoped<IMessageProcessor, MessageProcessor1>();
// Add other message processors here
builder.Services.AddSingleton<MessageProcessorFactory>();
builder.Services.AddSingleton<MessageEventProcessor>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<MessageEventProcessor>());

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();