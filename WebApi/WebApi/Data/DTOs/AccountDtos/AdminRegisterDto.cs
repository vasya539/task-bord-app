using System.ComponentModel.DataAnnotations;

namespace WebApi.Data.DTOs.AccountDtos
{
    public class AdminRegisterDto
    {
        public string UserName { get; set; }
        [EmailAddress]
        public string Email { get; set; }   
    }
}
