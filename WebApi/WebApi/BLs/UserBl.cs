using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

using WebApi.Repositories.Interfaces;
using WebApi.BLs.Interfaces;
using WebApi.Data.DTOs;
using WebApi.Data.Models;
using WebApi.Exceptions;

namespace WebApi.BLs
{
	public class UserBl : IUserBl
	{
		private readonly IUserRepository _userRepo;
		private readonly IProjectUserRepository _puRepo;
		private readonly IMapper _mapper;

		public const int MIN_TEMPLATE_LENGTH = 4;
		public const int MAX_TEMPLATE_LENGTH = 40;
		public const int USER_FIND_PAGE_SIZE = 4;

		public UserBl(IUserRepository userRepository, IMapper mapper, IProjectUserRepository puRepo)
		{
			_userRepo = userRepository;
			_puRepo = puRepo;
			_mapper = mapper;
		}

		public async Task DeleteAsync(string id)
		{
			if(await _userRepo.ExistsWithId(id))
			{
				// User can delete profile only if it hasn't any projects
				if ((await _puRepo.GetProjectsOfUser(id)).Count() == 0)
					await _userRepo.DeleteAsync(id);
				else
					throw new ForbiddenResponseException("Forbidden: you have some projects.");
			}
		}

		public async Task<IEnumerable<UserDto>> GetAllAsync()
		{
			IEnumerable<User> users = await _userRepo.GetAllAsync();
			return _mapper.Map<IEnumerable<User>, IEnumerable<UserDto>>(users);
		}

		public async Task<UserDto> GetByIdAsync(string id)
		{
			User user = await _userRepo.GetByIdAsync(id);
			if (user == null)
				throw new NotFoundResponseException();
			return _mapper.Map<UserDto>(user);
		}

		public async Task<UserDto> GetDetailedByIdAsync(string id)
		{
			User user = await _userRepo.GetDetailedByIdAsync(id);
			if (user == null)
				throw new NotFoundResponseException();
			return _mapper.Map<UserDto>(user);
		}

		public async Task<UserDto> GetByUserNameAsync(string userName, bool detailed)
		{
			User user = await _userRepo.GetByUserNameAsync(userName, detailed);
			if (user == null)
				throw new NotFoundResponseException();
			return _mapper.Map<UserDto>(user);
		}

		public async Task<IEnumerable<SimpleProjectDto>> GetProjectsOfUser(string senderId, string requestedUserId)
		{
			if (! await _userRepo.ExistsWithId(requestedUserId))
				throw new NotFoundResponseException();

			// get list of common projects. These projects are available for these 2 users
			IEnumerable<Project> res_projs = await GetCommonProjectsOfUsersAsync(senderId, requestedUserId);
			
			return await MapProjectsToDto(res_projs);
		}

		private async Task<IEnumerable<Project>> GetCommonProjectsOfUsersAsync(string firstUserId, string secondUserId)
		{
			// if it's the same user make only one request to DB
			if(firstUserId == secondUserId)
				return await _puRepo.GetProjectsOfUser(firstUserId);

			// else make 2 requests & return common
			IEnumerable<Project> firstUserProjects = await _puRepo.GetProjectsOfUser(firstUserId);
			IEnumerable<Project> secondUserProjects = await _puRepo.GetProjectsOfUser(secondUserId);

			return firstUserProjects
				.Join(secondUserProjects, f => f.Id, s => s.Id, (f, s) => f)
				.Select(x => x)
				.ToList();
		}

		private async Task<IEnumerable<SimpleProjectDto>> MapProjectsToDto(IEnumerable<Project> projs)
		{
			List<SimpleProjectDto> res = new List<SimpleProjectDto>();

			// Dto must contain info about project & owner
			foreach (Project p in projs)
			{
				User owner = await _puRepo.GetOwnerOfProject(p.Id);

				SimpleProjectDto dto = _mapper.Map<Project, SimpleProjectDto>(p);
				dto.Owner = _mapper.Map<User, UserDto>(owner);

				res.Add(dto);
			}
			return res;
		}

		public async Task<FoundUsersPageDto> Find(string template, int pageNumber)
		{
			string tmpl = PrepareRequest(template); // trim, delete duplicate spaces, etc.
			CheckTharRequestIsCorrect(template, pageNumber);

			IEnumerable<UserFoundModel> data;
			if(tmpl.Contains(' '))
			{
				var words = tmpl.Split(' ');
				if (words.Count() != 2)
					throw new BadRequestResponseException("request must contain one or two words");

				// find by first & last names ("john con")
				data = await _userRepo.FindAdvancedByFullNamePlusOneRowAsync(words[0], words[1], pageNumber, USER_FIND_PAGE_SIZE);
			} else if(tmpl.StartsWith('@'))
			{
				// find by username ("@john")
				data = await _userRepo.FindAdvancedByUserNamePlusOneRowAsync(tmpl.Substring(1), pageNumber, USER_FIND_PAGE_SIZE);
			} else
			{
				// find by username, firstname, lastname ("john")
				data = await _userRepo.FindAdvancedPlusOneRowAsync(tmpl, pageNumber, USER_FIND_PAGE_SIZE); ;
			}

			return MakeDtoFromFindResult(data, pageNumber);
		}

		private string PrepareRequest(string template)
		{
			if (template == null)
				return null;

			// remove duplicated space-symbols
			return System.Text.RegularExpressions.Regex.Replace(template.Trim(), @"\s+", " ");
		}

		private void CheckTharRequestIsCorrect(string template, int pageNumber)
		{
			if (string.IsNullOrEmpty(template))
				throw new BadRequestResponseException("template cannot be empty");

			if (pageNumber <= 0)
				throw new BadRequestResponseException("page must be greater than 0.");

			int length = template.Where(c => !Char.IsWhiteSpace(c)).Count();
			if (length < MIN_TEMPLATE_LENGTH || length > MAX_TEMPLATE_LENGTH)
				throw new BadRequestResponseException($"template must have number of non-space characters between {MIN_TEMPLATE_LENGTH} and {MAX_TEMPLATE_LENGTH}");

			foreach (char c in template)
			{
				if (!(Char.IsLetterOrDigit(c) || c == ' ' || c == '@' || c == '_'))
					throw new BadRequestResponseException($"template cannot contain '{c}'");
			}
		}

		private FoundUsersPageDto MakeDtoFromFindResult(IEnumerable<UserFoundModel> data, int pageNumber)
		{
			FoundUsersPageDto res = new FoundUsersPageDto();

			res.PageNumber = pageNumber;

			// if response contain +1 row => there is next page
			if (data.Count() == USER_FIND_PAGE_SIZE + 1)
				res.HasNext = true;
			else
				res.HasNext = false;

			var usersOnPage = data.Take(System.Math.Min(USER_FIND_PAGE_SIZE, data.Count()));
			res.Users = _mapper.Map<IEnumerable<UserFoundModel>, IEnumerable<UserDto>>(usersOnPage);

			return res;
		}

		public async Task UpdateAsync(string userId, UserDto dto)
		{
			if (userId != dto.Id)
				throw new BadRequestResponseException();

			User user = _mapper.Map<User>(dto);
			await _userRepo.UpdateAsync(user);
		}
	}
}
