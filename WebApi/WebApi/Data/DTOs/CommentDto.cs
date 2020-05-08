using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Data.Models;

namespace WebApi.Data.DTOs
{
    public class CommentDto
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public string UserId { get; set; }

        public ItemDto Item { get; set; }
        public UserDto User { get; set; }
    }
}