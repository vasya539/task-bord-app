using AutoMapper;
using System.Collections.Generic;
using WebApi.Data.DTOs;
using WebApi.Data.DTOs.AccountDtos;
using WebApi.Data.Models;

namespace WebApi.Data.Profiles
{
    public class AutoMapperProfiler : Profile
    {
        public AutoMapperProfiler()
        {
            CreateMap<Item, ItemDto>();
            CreateMap<ItemDto, Item>();

            CreateMap<SprintDto, Sprint>();
            CreateMap<Sprint, SprintDto>();

            CreateMap<User, UserDto>()
                .ReverseMap()
                .ForMember(u => u.Id, opts => opts.NullSubstitute(null));

            CreateMap<UserFoundModel, UserDto>();

            CreateMap<User, ProjectMemberDto>();

            CreateMap<Project, SimpleProjectDto>();

            CreateMap<Project, ProjectDto>();
            CreateMap<ProjectDto, Project>();

            CreateMap<ProjectUserDto, ProjectUser>();
            CreateMap<ProjectUser, ProjectUserDto>();

            CreateMap<ItemType, ItemTypeDto>();
            CreateMap<ItemTypeDto, ItemType>();
            CreateMap<Status, StatusDto>();
            CreateMap<StatusDto, Status>();
            
            CreateMap<Comment, CommentDto>();
            CreateMap<CommentDto, Comment>();

            CreateMap<UserMinimalDto, User>().ReverseMap();
            CreateMap<List<User>, List<UserMinimalDto>>().ReverseMap();
            CreateMap<List<UserMinimalDto>, List<User>>().ReverseMap();
            CreateMap<AppUserRole, AppUserRoleDto>();
            CreateMap<AppUserRoleDto, AppUserRole>()
                .ConvertUsing(dto => AppUserRole.GetUserRoleById(dto.Id));
           CreateMap<User, AdminRegisterDto>().ReverseMap();
            CreateMap<Item, ItemListDto>();
        }
    }
}