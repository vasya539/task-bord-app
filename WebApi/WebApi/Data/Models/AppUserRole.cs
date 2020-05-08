using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Data.Models
{
	[Table("AppUserRoles")]
	public class AppUserRole
	{
		public static readonly AppUserRole None = new AppUserRole(0, "NoMember", true);
		public static readonly AppUserRole Owner = new AppUserRole(1, "Owner", true);
		public static readonly AppUserRole ScrumMaster = new AppUserRole(2, "Scrum-Master", true);
		public static readonly AppUserRole Developer = new AppUserRole(3, "Developer", true);
		public static readonly AppUserRole Observer = new AppUserRole(5, "Observer", true);

		private static readonly Dictionary<int, AppUserRole> _roles = new Dictionary<int, AppUserRole>();


		private readonly bool _isReadonly;
		private int _id;
		private string _name;

		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int Id
		{
			get
			{
				return _id;
			}
			set
			{
				if(!_isReadonly)
					_id = value;
			}
		}

		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				if (!_isReadonly)
					_name = value;
			}
		}


		public ICollection<ProjectUser> ProjectUserRecords { get; set; }


		private AppUserRole(int id, string name, bool isReadonly = false)
		{
			this.Id = id;
			this.Name = name;

			_isReadonly = isReadonly;
		}
		public AppUserRole()
			: this(AppUserRole.None.Id, AppUserRole.None.Name)
		{ }

		static AppUserRole()
		{
			RegisterUserRole(None);
			RegisterUserRoles(Owner, ScrumMaster, Developer, Observer);
		}

		private static void RegisterUserRoles(params AppUserRole [] roles)
		{
			foreach (var role in roles)
				RegisterUserRole(role);
		}

		private static void RegisterUserRole(AppUserRole role)
		{
			if (!_roles.ContainsKey(role.Id))
				_roles.Add(role.Id, role);
		}


		public static AppUserRole GetUserRoleById(int id)
		{
			if (_roles.ContainsKey(id))
				return _roles[id];

			return AppUserRole.None;
		}
		

		public override string ToString()
		{
			return $"UserRole: {{ id: {this.Id}; Name: {Name} }}";
		}

		public static IEnumerable<AppUserRole> GetAllRoles()
		{
			var list = new List<AppUserRole>();
			foreach (var kv in _roles)
				list.Add(kv.Value);
			return list;
		}




		public bool IsNone() => this.Id == AppUserRole.None.Id;
		public bool IsOwner() => this.Id == AppUserRole.Owner.Id;
		public bool IsScrumMaster() => this.Id == AppUserRole.ScrumMaster.Id;
		public bool IsDeveloper() => this.Id == AppUserRole.Developer.Id;
		public bool IsObserver() => this.Id == AppUserRole.Observer.Id;


	}
}
