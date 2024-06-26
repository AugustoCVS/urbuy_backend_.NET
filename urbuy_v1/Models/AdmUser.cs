﻿using System.ComponentModel.DataAnnotations;

namespace urbuy_v1.Models
{
    public class AdmUser
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string AccessLevel { get; set; } = string.Empty;
    }
}
