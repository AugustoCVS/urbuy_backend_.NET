namespace urbuy_v1.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string AccessLevel { get; set; } = string.Empty;
    }

    public class UserBasicDTO
    {
        public string Username { get; set; }
        public string Email { get; set; }
    }
}
