namespace Chat.Application.Responces
{
    public class AuthenticationResponse : BaseResponce
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }

    }
}
