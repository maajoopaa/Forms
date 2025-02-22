namespace Forms.Models.Account
{
    public class AccountViewModel
    {
        public LoginViewModel? loginViewModel { get; set; }

        public RegisterViewModel? registerViewModel { get; set; }
    }

    public class LoginViewModel
    {
        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;

    }

    public class RegisterViewModel
    {
        public string Username { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;
    }

}
