using System.ComponentModel.DataAnnotations;

namespace LoginWebExample.ViewModel
{
    public class LoginModel
    {
        [Required, MaxLength(256)]
        public string UserName { get; set; } = null!;

        [Required, MaxLength(64)]
        public string Password { get; set; } = null!;
    }
}