namespace Netflix.WebAPI.Services.Abstracts
{
    public interface IEmailService
    {
        int SendVerificationCode(string recipientEmail);
        (bool IsValid, string Message) CheckVerificationCode(int inputCode);
    }
}
