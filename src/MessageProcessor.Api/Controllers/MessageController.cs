using MessageProcessor.Core.Data;
using MessageProcessor.Core.Events;
using MessageProcessor.Core.Models;
using MessageProcessor.Core.Services.MessageProcessing;
using Microsoft.AspNetCore.Mvc;

namespace MessageProcessor.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class MessageController : ControllerBase
{
    private readonly YourDbContext _dbContext;
    private readonly MessageEventProcessor _eventProcessor;
    private readonly ILogger<MessageController> _logger;

    public MessageController(
        YourDbContext dbContext,
        MessageEventProcessor eventProcessor,
        ILogger<MessageController> logger)
    {
        _dbContext = dbContext;
        _eventProcessor = eventProcessor;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> ReceiveMessage([FromBody] string rawMessage)
    {
        try
        {
            // Determine message type based on message content
            string messageType = DetermineMessageType(rawMessage);

            var frame = new RawDomainFrame
            {
                RawMessage = rawMessage,
                Processed = false,
                ReceivedAt = DateTime.UtcNow,
                MessageType = messageType
            };

            _dbContext.RawDomainFrames.Add(frame);
            await _dbContext.SaveChangesAsync();

            // Publish event
            await _eventProcessor.PublishMessageAsync(new MessageReceivedEvent
            {
                RawDomainFrameId = frame.Id,
                RawMessage = rawMessage,
                MessageType = messageType
            });

            return Ok(new { id = frame.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error receiving message");
            return StatusCode(500, "Error processing message");
        }
    }

    private string DetermineMessageType(string rawMessage)
    {
        // Implement logic to determine message type from raw message
        return "Type1";
    }
}