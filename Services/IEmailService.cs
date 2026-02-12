namespace FPTTelecomBE.Services;

public interface IEmailService
{
    Task<bool> SendRegistrationNotificationToAdminAsync(
        string registrationId,
        string customerName,
        string phone,
        string address,
        string packageName);

    Task<bool> SendJobApplicationNotificationToAdminAsync(
        string applicationId,
        string candidateName,
        string email,
        string phone,
        string position,
        string jobTitle);
}