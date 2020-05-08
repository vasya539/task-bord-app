using System;

using System.ComponentModel.DataAnnotations;

namespace WebApi.Data.DTOs
{
    public class UserLoginDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Password { get; set; }

    }
}
