namespace MessageProcessor.Core.Models;

public class RawDomainFrame
{
    public int Id { get; set; }
    public string RawMessage { get; set; }
    public bool Processed { get; set; }
    public string Description { get; set; }
    public DateTime ReceivedAt { get; set; }
    public string MessageType { get; set; }
    public byte[] RowVersion { get; set; } // For concurrency control
}