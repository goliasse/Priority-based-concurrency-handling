namespace MessageProcessor.Core.Services.MessageProcessing;

public interface IMessageProcessor
{
    bool CanProcess(string messageType);
    Task<(bool isValid, string errorDescription)> ProcessMessage(string rawMessage);
}