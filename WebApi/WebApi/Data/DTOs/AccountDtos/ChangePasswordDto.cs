using System.ComponentModel.DataAnnotations;

namespace WebApi.Data.DTOs.AccountDtos
{
    public class ChangePasswordDto
    {
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "Passwords don't match")]
        public string ConfirmNewPassword { get; set; }

        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }
    }
}
