namespace LoginWebExample.Options
{
    public class AppSettings
    {
        public int MaxLoginAttempt { get; set; }
        public int ResetLoginAttemptAt { get; set; }
        public int ResetLinkMaxAttemp { get; set; }
        public int ResetLinkTimeOut { get; set; }
        public string BaseAppUrl { get; set; } = null!;
    }
}