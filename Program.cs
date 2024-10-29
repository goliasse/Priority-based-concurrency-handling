var builder = WebApplication.CreateBuilder(args);

// Add services to the container
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
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();