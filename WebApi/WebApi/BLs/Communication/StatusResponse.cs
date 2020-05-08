using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Data.DTOs;
using WebApi.Data.Models;

namespace WebApi.BLs.Communication
{
    public class StatusResponse : BaseResponse
    {
        public StatusDto StatusDto { get; private set; }

        private StatusResponse(bool success, string message, StatusDto statusDto) : base(success, message)
        {
            StatusDto = statusDto;
        }

        public StatusResponse(StatusDto statusDto): this(true, string.Empty, statusDto)
        { }
        public StatusResponse(string message) : this(false, message, null)
        { }
    }
}
