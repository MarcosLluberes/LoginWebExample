namespace LoginWebExample.ViewResult
{
    public class AuthResult
    {
        public int ID { get; set; }
        public string Username { get; set; } = null!;
        public string CompleteName { get; set; } = null!;
        public string Token { get; set; } = null!;
        public DateTime Expires { get; set; }
    }
}