namespace UserService.Domain.Entities
{
    public class Doctor : User
    {
        public string Specialty { get; set; } = default!;
    }
}
