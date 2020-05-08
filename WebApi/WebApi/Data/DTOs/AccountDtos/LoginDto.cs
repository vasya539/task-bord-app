using System;

using System.ComponentModel.DataAnnotations;

namespace WebApi.Data.DTOs.AccountDtos
{
    public class LoginDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Password { get; set; }

    }
}
