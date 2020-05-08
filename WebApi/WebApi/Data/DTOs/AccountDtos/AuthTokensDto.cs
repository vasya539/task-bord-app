using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Data.DTOs.AccountDtos
{
    public class AuthTokensDto
    {
        [Required]
        public string AccessToken { get; set; }
        [Required]
        public string RefreshToken { get; set; }
        public DateTime ExpireOn { get; set; }

        public override bool Equals(object obj)
        {
            var dto = obj as AuthTokensDto;
            return dto != null &&
                   AccessToken == dto.AccessToken &&
                   RefreshToken == dto.RefreshToken &&
                   ExpireOn.ToLongDateString() == dto.ExpireOn.ToLongDateString();
        }
    }
}
