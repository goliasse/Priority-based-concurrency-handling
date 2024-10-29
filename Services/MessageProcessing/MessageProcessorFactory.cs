public class MessageProcessorFactory
{
    private readonly IEnumerable<IMessageProcessor> _processors;

    public MessageProcessorFactory(IEnumerable<IMessageProcessor> processors)
    {
        _processors = processors;
    }

    public IMessageProcessor GetProcessor(string messageType)
    {
        return _processors.FirstOrDefault(p => p.CanProcess(messageType))
            ?? throw new ArgumentException($"No processor found for message type: {messageType}");
    }
}