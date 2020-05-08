using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApi.Data.Models;
using WebApi.Repositories;
using WebApi.Data.DTOs;
using WebApi.BLs.Interfaces;
using AutoMapper;
using WebApi.Repositories.Interfaces;

namespace WebApi.BLs
{
    public class ItemTypeBl : IItemTypeBl
    {
        private readonly IItemTypeRepository _itemTypeRepository;
        private readonly IMapper _mapper;

        public ItemTypeBl(IItemTypeRepository itemTypeRepository, IMapper mapper)
        {
            _mapper = mapper;
            _itemTypeRepository = itemTypeRepository;
        }
        public async Task Create(ItemTypeDto itemTypeDto)
        {
            itemTypeDto.Id = 0;
            var normalItemType = _mapper.Map<ItemType>(itemTypeDto);
            await _itemTypeRepository.CreateAsync(normalItemType);
        }

        public async Task Delete(int id)
        {
            try
            {
                await _itemTypeRepository.DeleteAsync(id);
            }
            catch (Exception)
            {
                return;
            }
        }

        public async Task<List<ItemTypeDto>> GetAllAsync()
        {
            var itemTypes = await _itemTypeRepository.GetAllAsync();
            List<ItemTypeDto> itemTypesDtos = new List<ItemTypeDto>();
            foreach (var i in itemTypes)
            {
                var normal = _mapper.Map<ItemTypeDto>(i);
                itemTypesDtos.Add(normal);
            }
            return itemTypesDtos;
        }

        public async Task<ItemTypeDto> Read(int id)
        {
            var itemType = await _itemTypeRepository.ReadAsync(id);
            var itemTypeDto = _mapper.Map<ItemTypeDto>(itemType);
            return itemTypeDto;
        }

        public async Task Update(ItemTypeDto itemTypeDto)
        {
            var newItemType = _mapper.Map<ItemType>(itemTypeDto);
            try
            {
                await _itemTypeRepository.UpdateAsync(newItemType);

            }
            catch (DbUpdateConcurrencyException)
            {
                return;
            }
            return;
        }
    }
}
