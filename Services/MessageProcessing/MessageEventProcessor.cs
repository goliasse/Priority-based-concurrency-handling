public class MessageEventProcessor : IHostedService
{
    private readonly ILogger<MessageEventProcessor> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly MessageProcessorFactory _processorFactory;
    private readonly Channel<MessageReceivedEvent> _channel;
    private const int BatchSize = 100;
    private const int BatchTimeoutMs = 1000; // 1 second timeout

    public MessageEventProcessor(
        ILogger<MessageEventProcessor> logger,
        IServiceScopeFactory scopeFactory,
        MessageProcessorFactory processorFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _processorFactory = processorFactory;
        _channel = Channel.CreateUnbounded<MessageReceivedEvent>();
    }

    public async Task PublishMessageAsync(MessageReceivedEvent message)
    {
        await _channel.Writer.WriteAsync(message);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _ = ProcessMessages(cancellationToken);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _channel.Writer.Complete();
        return Task.CompletedTask;
    }

    private async Task ProcessMessages(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await ProcessMessageBatch(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message batch");
                await Task.Delay(1000, cancellationToken); // Back off on error
            }
        }
    }

    private async Task ProcessMessageBatch(CancellationToken cancellationToken)
    {
        var messages = new List<MessageReceivedEvent>();
        var batchTimer = Stopwatch.StartNew();

        // Collect messages until we hit batch size or timeout
        while (messages.Count < BatchSize && 
               batchTimer.ElapsedMilliseconds < BatchTimeoutMs && 
               !cancellationToken.IsCancellationRequested)
        {
            if (await _channel.Reader.WaitToReadAsync(cancellationToken))
            {
                if (_channel.Reader.TryRead(out var message))
                {
                    messages.Add(message);
                }
            }
            else
            {
                break; // Channel completed
            }
        }

        if (messages.Count == 0)
        {
            return;
        }

        // Process the batch with a single DbContext
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<YourDbContext>();

        var messagesByType = messages.GroupBy(m => m.MessageType);
        foreach (var messageGroup in messagesByType)
        {
            var processor = _processorFactory.GetProcessor(messageGroup.Key);
            
            foreach (var message in messageGroup)
            {
                try
                {
                    var (isValid, errorDescription) = await processor.ProcessMessage(message.RawMessage);
                    var frame = await dbContext.RawDomainFrames
                        .FindAsync(new object[] { message.RawDomainFrameId }, cancellationToken);

                    if (frame != null)
                    {
                        frame.Processed = true;
                        frame.Description = errorDescription;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message {MessageId}", message.RawDomainFrameId);
                }
            }
        }

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex, "Concurrency error while saving batch");
            // Consider implementing retry logic or handling specific cases
        }
    }
}