using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Data.DTOs.AccountDtos
{
    public class UsersPageDto
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public IEnumerable<UserMinimalDto> Items { get; set; }

    }
}
