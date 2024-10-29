public class ExampleJob : RetryableJobBase
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ExampleJob> _logger;

    public ExampleJob(
        IServiceScopeFactory scopeFactory,
        ILogger<ExampleJob> logger) : base(logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteJob(IJobExecutionContext context)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<YourDbContext>();

        // Use optimistic concurrency
        var entity = await dbContext.YourTable
            .FromSqlRaw("SELECT * FROM YourTable WITH (READCOMMITTEDLOCK) WHERE Id = @id", 
                new SqlParameter("@id", 1))
            .FirstOrDefaultAsync();

        if (entity != null)
        {
            entity.Property = "Updated Value";
            await dbContext.SaveChangesAsync();
        }
    }
}