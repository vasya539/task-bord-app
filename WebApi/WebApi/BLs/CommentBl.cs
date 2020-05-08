using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.BLs.Communication;
using WebApi.BLs.Interfaces;
using WebApi.Data.DTOs;
using WebApi.Data.Models;
using WebApi.Exceptions;
using WebApi.Interfaces.IRepositories;
using WebApi.Repositories.Interfaces;
using WebApi.Extensions.AppUserRolesExtensions.Items;

namespace WebApi.BLs
{
    /// <summary>
    /// Class for comment business logic. Implements default interface for comment business logic.
    /// </summary>
    public class CommentBl : ICommentBl
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IMapper _mapper;
        private readonly IProjectUserRepository _puRepo;
        private readonly IProjectRepository _projectRepo;
        private readonly ISprintRepository _sprintRepository;

        /// <summary>
        /// Constructor for initializing Comment repository, Mapper and other additional repositories.
        /// </summary>
        /// <param name="mapper">Auto-mapper</param>
        /// <param name="commentRepository">Repository for comments</param>
        /// <param name="puRepository">Repository for project-users</param>
        /// <param name="projectRepo">Repository for projects</param>
        /// <param name="sprintRepository">Repository for sprints</param>
        public CommentBl(ICommentRepository commentRepository,
            IMapper mapper,
            IProjectUserRepository puRepository,
            IProjectRepository projectRepo,
            ISprintRepository sprintRepository)
        {
            _mapper = mapper;
            _commentRepository = commentRepository;
            _puRepo = puRepository;
            _projectRepo = projectRepo;
            _sprintRepository = sprintRepository;
        }

        /// <summary>
        /// Get CommentDto from controller, map it and create a new Comment.
        /// </summary>
        /// <param name="comment">New comment to be created</param>
        /// <returns>Response with success message</returns>
        public async Task<ItemResponse> CreateAsync(CommentDto comment)
        {
            var origComment = _mapper.Map<Comment>(comment);
            await _commentRepository.CreateAsync(origComment);
            return new ItemResponse(true, "Created");
        }

        /// <summary>
        /// Get comment id from controller, find this comment in database, and delete. Also check user's permissions
        /// </summary>
        /// <param name="id">Id of comment to be deleted</param>
        /// <param name="userId">Id of loginned user</param>
        /// <returns>Response with success message</returns>
        /// <exception cref="NotFoundResponseException">Comment not found</exception>
        /// <exception cref="ForbiddenResponseException">User dont have permissions to delete comment</exception>
        public async Task<ItemResponse> DeleteAsync(int id, string userId)
        {
            var realComment = await _commentRepository.ReadAsync(id);
            if (realComment == null) throw new NotFoundResponseException();
            var userRole = await getUserRoleAsync(realComment.Item, userId);
    
            // User can delete only his comment
            if (!userRole.CanDeleteComment(realComment, userId))
                throw new ForbiddenResponseException("Sorry, you don't have access to delete this comment!");
            await _commentRepository.DeleteAsync(id);
            return new ItemResponse(true, "Deleted successfully");
        }

        /// <summary>
        /// Get all comments from database
        /// </summary>
        /// <returns>List of CommentDto</returns>
        public async Task<IEnumerable<CommentDto>> GetAllAsync()
        {
            var comments = await _commentRepository.GetAllAsync();

            IEnumerable<CommentDto> dtoComments = _mapper.Map<IEnumerable<Comment>, IEnumerable<CommentDto>>(comments);
            return dtoComments;
        }

        /// <summary>
        /// Get all comments for specific item
        /// </summary>
        /// <param name="itemId">Item id</param>
        /// <returns>List of CommentsDto</returns>
        public async Task<IEnumerable<CommentDto>> GetByItemIdAsync(int itemId)
        {
            var comment = await _commentRepository.GetByItemIdAsync(itemId);

            IEnumerable<CommentDto> dtoComments = _mapper.Map<IEnumerable<Comment>, IEnumerable<CommentDto>>(comment);
            return dtoComments;
        }

        /// <summary>
        /// Get specific comment by id
        /// </summary>
        /// <param name="id">Comment id</param>
        /// <returns>CommentDto object</returns>
        public async Task<CommentDto> ReadAsync(int id)
        {
            var comment = await _commentRepository.ReadAsync(id);
            var dtoComment = _mapper.Map<CommentDto>(comment);

            return dtoComment;
        }

        /// <summary>
        /// Update specific comment
        /// </summary>
        /// <param name="comment">Comment to be updated</param>
        /// <returns>Response with success message</returns>
        public async Task<ItemResponse> UpdateAsync(CommentDto comment)
        {
            var origComment = _mapper.Map<Comment>(comment);
            await _commentRepository.UpdateAsync(origComment);
            return new ItemResponse(true, "Created");
        }

        /// <summary>
        /// Additional method to get role of user
        /// </summary>
        /// <param name="item">Current item</param>
        /// <param name="userId">Loginned user's id</param>
        /// <returns>AppUserRole object</returns>
        /// <exception cref="NotFoundResponseException">This project not found</exception>
        public async Task<AppUserRole> getUserRoleAsync(Item item, string userId)
        {
            Sprint sprint = await _sprintRepository.GetByIdAsync(item.SprintId);
            Project project = await _projectRepo.GetByIdAsync(sprint.ProjectId);
            if (project == null)
                throw new NotFoundResponseException();
            return await _puRepo.GetRoleOfMember(userId, sprint.ProjectId);
        }
    }
}