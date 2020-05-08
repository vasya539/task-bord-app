using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using WebApi.Data.Models;
using WebApi.Data;
using WebApi.Repositories.Interfaces;


namespace WebApi.Repositories
{
	public class UserRepository : IUserRepository
	{
		private readonly AppDbContext _context;

		public UserRepository(AppDbContext context)
		{
			_context = context;
		}

		/// <summary>
		/// Selector to extract min version of user
		/// </summary>
		public static System.Linq.Expressions.Expression<Func<User, User>>
			UserSelector = (User u) =>
				new User
				{
					Id = u.Id,
					UserName = u.UserName,
					Email = u.Email,
					FirstName = u.FirstName,
					LastName = u.LastName,
					AvatarTail = u.AvatarTail
				};

		/// <summary>
		/// Selector to extract min version of user + Info field
		/// </summary>
		public static System.Linq.Expressions.Expression<Func<User, User>>
			UserDetailedSelector = (User u) =>
				new User
				{
					Id = u.Id,
					UserName = u.UserName,
					Email = u.Email,
					FirstName = u.FirstName,
					LastName = u.LastName,
					AvatarTail = u.AvatarTail,
					Info = u.Info
				};


		public async Task<IEnumerable<User>> GetAllAsync()
		{
			return await _context.Users.ToListAsync();
		}

		public async Task<User> GetByIdAsync(string id)
		{
			id = id.ToLower();
			return await GetUserWhereAsync(u => u.Id == id, UserSelector);
		}

		public async Task<User> GetDetailedByIdAsync(string id)
		{
			id = id.ToLower();
			return await GetUserWhereAsync(u => u.Id == id, UserDetailedSelector);
		}

		public async Task<User> GetByUserNameAsync(string userName, bool detailed)
		{
			string userNameInUpperCase = userName.ToUpper();
			User user;
			if(detailed)
				user = await GetUserWhereAsync(u => u.NormalizedUserName == userNameInUpperCase, UserDetailedSelector);
			else
				user = await GetUserWhereAsync(u => u.NormalizedUserName == userNameInUpperCase, UserSelector);

			return user;
		}

		/// <summary>
		/// Returns user from Db if func returns true
		/// </summary>
		/// <param name="func">given predicate</param>
		/// <param name="selector">selector to specify which fields should be extracted</param>
		/// <returns></returns>
		private async Task<User> GetUserWhereAsync(
			System.Linq.Expressions.Expression<Func<User, bool>> func,
			System.Linq.Expressions.Expression<Func<User, User>> selector)
		{
			return await _context.Users
				.Where(func)
				.Select(selector)
				.SingleOrDefaultAsync();
		}

		public async Task UpdateAsync(User user)
		{
			User u = await _context.Users.FindAsync(user.Id);

			u.FirstName = user.FirstName;
			u.LastName = user.LastName;
			u.Email = user.Email;
			u.NormalizedEmail = user.Email.ToUpper();
			u.Info = user.Info;

			_context.Users.Update(u);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(string id)
		{
			User user = await _context.Users.FindAsync(id);// await GetByIdAsync(id);
			if (user != null)
			{
				var rts = _context.UserRefreshTokens.Where(t => t.UserId == id).ToList();
				_context.UserRefreshTokens.RemoveRange(rts);
				_context.Users.Remove(user);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<bool> ExistsWithId(string id)
		{
			return await _context.Users.AnyAsync<User>(u => u.Id == id.ToString());
		}

		public async Task<IEnumerable<UserFoundModel>> FindAdvancedPlusOneRowAsync(string template, int page, int count)
		{
			var users = await
				_context.FoundUsers.FromSqlRaw(
						$"FindUsersByTemplatePlusOneRow @template = N'{template}', @pageOffset = {page-1}, @count = {count}")
				.ToListAsync();
			return users;
		}

		public async Task<IEnumerable<UserFoundModel>> FindAdvancedByUserNamePlusOneRowAsync(string template, int page, int count)
		{
			var users = await
				_context.FoundUsers.FromSqlRaw(
						$"FindUsersByUserNameTemplatePlusOneRow @template = N'{template}', @pageOffset = {page - 1}, @count = {count}")
				.ToListAsync();
			return users;
		}


		public async Task<IEnumerable<UserFoundModel>> FindAdvancedByFullNamePlusOneRowAsync(string firstName, string lastName, int page, int count)
		{
			var users = await
				_context.FoundUsers.FromSqlRaw(
						$"FindUsersByFullNameTemplatePlusOneRow " +
						$"@firstName = N'{firstName}', @lastName = N'{lastName}', @pageOffset = {page - 1}, @count = {count}")
				.ToListAsync();
			return users;
		}


		public async Task<bool> ExistsWithUserName(string userName)
		{
			string userNameInUpperCase = userName.ToUpper();
			return await _context.Users.AnyAsync<User>(u => u.NormalizedUserName == userNameInUpperCase);
		}


		public async Task UpdateAvatarTailAsync(string userId, int? value)
		{
			User user = await _context.Users.FindAsync(userId);
			if (user != null)
			{
				user.AvatarTail = value;
				await _context.SaveChangesAsync();
			}
		}

	}
}
