namespace FPTTelecomBE.Services;

public interface IChatbotService
{
    string? GetAutoReply(string userMessage);
    bool IsRequestingStaff(string message);
    string GetWelcomeMessage(string? userName);
}