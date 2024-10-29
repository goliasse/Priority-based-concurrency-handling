public class MessageReceivedEvent
{
    public int RawDomainFrameId { get; set; }
    public string RawMessage { get; set; }
    public string MessageType { get; set; }
}