namespace UserService.Application.DTOs
{
    public class RegisterRequest
    {
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string? Role { get; set; } // "Doctor" or "Patient"
        public string? Specialty { get; set; }
    }
}
