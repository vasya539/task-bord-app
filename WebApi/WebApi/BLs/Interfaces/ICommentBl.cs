using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.BLs.Communication;
using WebApi.Data.DTOs;

namespace WebApi.BLs.Interfaces
{
    /// <summary>
    /// Specific comment business logic interface with CRUD operations.
    /// </summary>
    public interface ICommentBl
    {
        Task<IEnumerable<CommentDto>> GetAllAsync();

        Task<IEnumerable<CommentDto>> GetByItemIdAsync(int itemId);

        Task<CommentDto> ReadAsync(int id);

        Task<ItemResponse> CreateAsync(CommentDto comment);

        Task<ItemResponse> UpdateAsync(CommentDto comment);

        Task<ItemResponse> DeleteAsync(int id, string userId);
    }
}