using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Data.Models
{
    public class UserRefreshToken
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        [MaxLength(44)]
        public string RefreshToken { get; set; }
        public DateTime ExpireOn { get; set; }
    }
}
