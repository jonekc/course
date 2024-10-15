using System.ComponentModel.DataAnnotations;

namespace Projekt.Shared.Models
{
    public class AuthenticateRequest
    {
        [Required(ErrorMessage = "Wpisz login")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Wpisz hasło")]
        public string Password { get; set; }
    }
}
