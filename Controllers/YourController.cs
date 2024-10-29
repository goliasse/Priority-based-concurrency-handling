public class YourController : ControllerBase
{
    private readonly YourDbContext _dbContext;
    private readonly ILogger<YourController> _logger;

    public YourController(YourDbContext dbContext, ILogger<YourController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> ReceiveMessage([FromBody] YourMessage message)
    {
        try
        {
            // API endpoints use direct database access without retries
            var entity = new YourEntity
            {
                // Set properties
            };

            _dbContext.Add(entity);
            await _dbContext.SaveChangesAsync();
            
            return Ok();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex, "Concurrency error in API endpoint");
            return StatusCode(409, "Concurrency error occurred");
        }
    }
}