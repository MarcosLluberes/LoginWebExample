namespace LoginWebExample.ExampleModel
{
    public class User
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Passcode { get; set; } = null!;
        public string Usuario { get; set; } = null!;
        public string TenandId { get; set; } = null!;
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdate { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime? LastPasswordReset { get; set; }
        public bool Suspended { get; set; }
        public DateTime? LastLoginFail { get; set; }
        public byte? LoginFailAttempts { get; set; }
    }
}