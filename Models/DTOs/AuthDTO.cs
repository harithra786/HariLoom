namespace hariloom.Models.DTOs
{
    public class RegisterUserDTO
    {
        public string name { get; set; }
        public string phoneNumber { get; set; }
        public string password { get; set; }
        public string? email { get; set; }
        public bool isPromotionalEmailOptIn { get; set; }
        public string? ipAddress { get; set; }
    }

    public class UserDatabaseDTO
    {
        public int mstUserId { get; set; }
        public string name { get; set; }
        public string phoneNumber { get; set; }
        public string? email { get; set; }
        public DateTime createdDate { get; set; }
        public bool isPromotionalEmailOptIn { get; set; }
        public string? ipAddress { get; set; }
        public int visitCount { get; set; }
    }

    public class LoginDTO
    {
        public string phoneNumber { get; set; }
        public string password { get; set; }
        public string? returnUrl { get; set; }
    }

    public class ForgotPasswordDTO
    {
        public string phoneNumber { get; set; }
        public string newPassword { get; set; }
    }

    public class AuthTokenPayload
    {
        public int UserId { get; set; }
        public string PhoneNumber { get; set; }
        public int AccessLevel { get; set; }
    }

}
