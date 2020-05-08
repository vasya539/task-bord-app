using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApi.Data.Models;
using WebApi.Interfaces.IRepositories;
using WebApi.Interfaces.IServices;
using WebApi.Data.DTOs;
using AutoMapper;
using WebApi.BLs.Communication;

namespace WebApi.Services
{
    /// <summary>
    /// Class for item business logic. Implements default interface for item bussiness loggic.
    /// </summary>
    public class ItemBl : IItemBl
    {
        private readonly IItemRepository _itemRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor for initializing ItemRepository and Mapper
        /// </summary>
        /// <param name="itemRepository">Class that implements IItemRepository and have default methods</param>
        /// <param name="mapper"></param>
        public ItemBl(IItemRepository itemRepository, IMapper mapper)
        {
            _mapper = mapper;
            _itemRepository = itemRepository;
        }

        #region CRUD-Operations

        /// <summary>
        /// Use repository to get all items from Database
        /// </summary>
        /// <returns>List of ItemDto to controller</returns>
        public async Task<IEnumerable<ItemDto>> GetAllAsync()
        {
            var items = await _itemRepository.GetAllAsync();

            IEnumerable<ItemDto> dtoItems = _mapper.Map<IEnumerable<Item>, IEnumerable<ItemDto>>(items);
            return dtoItems;
        }

        /// <summary>
        /// Read. Search item with param Id in database, and map it.
        /// <param name="id">Id of the item to be returned to client</param>
        /// <returns>Needfull item to controller</returns>
        public async Task<ItemDto> ReadAsync(int id)
        {
            var item = await _itemRepository.ReadAsync(id);
            var dtoItem = _mapper.Map<ItemDto>(item);

            return dtoItem;
        }

        /// <summary>
        /// Create. Map ItemDto=>Item and use repository to create a new item.
        /// </summary>
        /// <param name="item">ItemDto that got from controller</param>
        /// <returns>Item response with message for client</returns>
        public async Task<ItemResponse> CreateAsync(ItemDto item)
        {
            try
            {
                var origItem = _mapper.Map<Item>(item);
                await _itemRepository.CreateAsync(origItem);
                return new ItemResponse(true, "Created");
            }
            catch (Exception ex)
            {
                return new ItemResponse(false, $"Something went wrong:  {ex.Message}");
            }
        }

        /// <summary>
        /// Update. Get itemDto from controller, map it and update in database.
        /// </summary>
        /// <param name="item">Item to be updated in database</param>
        /// <returns>Item response with message for client</returns>
        public async Task<ItemResponse> UpdateAsync(ItemDto item)
        {
            try
            {
                var origItem = _mapper.Map<Item>(item);
                await _itemRepository.UpdateAsync(origItem);
                return new ItemResponse(true, "Updated successfully");
            }
            catch (Exception ex)
            {
                return new ItemResponse(false, $"Something went wrong:  {ex.Message}");
            }
        }

        /// <summary>
        /// Delete. Get item Id from controller, and delete item from database.
        /// </summary>
        /// <param name="id">Id of the item to be deleted</param>
        /// <returns>Item response with message for client</returns>
        public async Task<ItemResponse> DeleteAsync(int id)
        {
            try
            {
                var origItem = await _itemRepository.ReadAsync(id);
                if (origItem == null)
                    return new ItemResponse(false, "Item not found!");

                await _itemRepository.DeleteAsync(id);
                return new ItemResponse(true, "Deleted successfully");
            }
            catch (Exception ex)
            {
                return new ItemResponse(false, $"Something went wrong:  {ex.Message}");
            }
        }

        #endregion CRUD-Operations
    }
}