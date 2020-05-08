using System.ComponentModel.DataAnnotations;

namespace WebApi.Data.DTOs.AccountDtos
{
    public class UpdateDto
    {
        [MinLength(3, ErrorMessage = "Title is too short.")]
        [MaxLength(64, ErrorMessage = "Title is too long.")]
        public string UserName { get; set; }
    }
}
