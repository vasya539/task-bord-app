using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Data.DTOs;
using WebApi.Data.Models;

namespace WebApi.BLs.Communication
{
    public class ItemTypeResponse : BaseResponse
    {
        public ItemDto ItemDto { get; private set; }

        private ItemTypeResponse(bool success, string message, ItemDto itemDto) : base(success, message)
        {
            ItemDto = itemDto;
        }
        public ItemTypeResponse(ItemDto itemDto) : this(true, string.Empty, itemDto)
        { }
        public ItemTypeResponse(string message) : this(false, message, null)
        { }
    }
}
