namespace NotificationService.Infrastructure.Senders
{
    public class TwilioSettings
    {
        public string AccountSid { get; set; } = string.Empty;
        public string AuthToken { get; set; } = string.Empty;
        /// <summary>
        /// Either a Twilio phone number in E.164 format (e.g. +15551234567)
        /// or a Messaging Service SID (starting with "MG").
        /// </summary>
        public string FromNumber { get; set; } = string.Empty;
    }
}
