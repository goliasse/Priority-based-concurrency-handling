public class MessageProcessor1 : IMessageProcessor
{
    public bool CanProcess(string messageType) => messageType == "Type1";

    public async Task<(bool isValid, string errorDescription)> ProcessMessage(string rawMessage)
    {
        try
        {
            var message = JsonSerializer.Deserialize<Type1Message>(rawMessage);
            // Apply business validation rules
            if (!IsValid(message))
            {
                return (false, "Validation failed: specific reason");
            }
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, $"Processing error: {ex.Message}");
        }
    }

    private bool IsValid(Type1Message message)
    {
        // Implement validation logic
        return true;
    }
}