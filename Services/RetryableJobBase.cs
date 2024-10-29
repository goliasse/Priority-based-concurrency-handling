using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Retry;

public abstract class RetryableJobBase : IJob
{
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly ILogger<RetryableJobBase> _logger;
    
    protected RetryableJobBase(ILogger<RetryableJobBase> logger)
    {
        _logger = logger;
        _retryPolicy = Policy
            .Handle<DbUpdateConcurrencyException>()
            .WaitAndRetryAsync(
                5, // Number of retries
                attempt => TimeSpan.FromMilliseconds(Math.Min(100 * Math.Pow(2, attempt), 1000)), // Exponential backoff with max 1 second
                (exception, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning(
                        "Attempt {RetryCount} encountered concurrency exception. Retrying in {DelayMs}ms. Error: {Error}",
                        retryCount,
                        timeSpan.TotalMilliseconds,
                        exception.Message);
                }
            );
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await _retryPolicy.ExecuteAsync(async () => await ExecuteJob(context));
    }

    protected abstract Task ExecuteJob(IJobExecutionContext context);
}