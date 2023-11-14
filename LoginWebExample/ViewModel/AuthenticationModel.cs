namespace LoginWebExample.ViewModel
{
    public class AuthenticationModel
    {
        public AuthenticationModel() { }

        public AuthenticationModel(int id, string token, string uniqueId, string username, DateTime? expiredTime)
        {
            ID = id;
            Token = token;
            ExpiredTime = expiredTime;
            UniqueId = uniqueId;
            Username = username;
            GuidID = Guid.NewGuid().ToString();
        }

        public int ID { get; set; }
        public string Token { get; set; } = null!;
        public string? GuidID { get; set; }
        public string UniqueId { get; set; } = null!;
        public string Username { get; set; } = null!;
        public DateTime? ExpiredTime { get; set; }
    }
}