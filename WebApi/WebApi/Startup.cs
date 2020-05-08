using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApi.Data;
using WebApi.Repositories.Interfaces;
using WebApi.Repositories;
using WebApi.BLs;
using AutoMapper;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;

using WebApi.Interfaces.IRepositories;
using WebApi.BLs.Interfaces;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;

using WebApi.Data.Models;
using WebApi.Extensions;

namespace WebApi
{
	public class Startup
	{
		private IConfiguration Configuration { get; }

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public void ConfigureServices(IServiceCollection services)
		{
			string connectionString;
			if (Environment.GetEnvironmentVariable("DB_CONNECTION_NAME") == "LocalDb")
				connectionString = Configuration["Data:LocalDbConnection:ConnectionString"];
			else
				connectionString = Configuration["Data:DefaultConnection:ConnectionString"];

			services.AddEntityFrameworkSqlServer().AddDbContext<AppDbContext>(options =>
				{
					options.UseSqlServer(
							connectionString,
							providerOpts => providerOpts.EnableRetryOnFailure(3, TimeSpan.FromMilliseconds(500), null));
				});

			//add Indentity
			services.AddIdentity<User, IdentityRole>(setup =>
				{
					setup.Password = new PasswordOptions()
					{
						RequireNonAlphanumeric=false,
						RequireDigit=true,
						RequireLowercase=true,
						RequireUppercase=true,
						RequiredLength=8
					};
					setup.User.RequireUniqueEmail = true;
					setup.User.AllowedUserNameCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890 -_'"; //Allowed chars: A-Z,a-z,0-9, -_'
				})
				.AddEntityFrameworkStores<AppDbContext>()
				.AddDefaultTokenProviders();

			// add token
			JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims
			services
				.AddAuthentication(options =>
				{
					options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
					options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
					options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				})
				.AddJwtBearer(cfg =>
				{
					cfg.RequireHttpsMetadata = false;
					cfg.SaveToken = true;
					cfg.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuer = true,
						ValidateAudience = true,
						ValidateLifetime = true,
						ValidIssuer = Configuration["JwtIssuer"],
						ValidAudience = Configuration["JwtAudience"],
						IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(Configuration["JwtKey"])),
					};
				});

			services.AddAutoMapper(typeof(Startup));
			services.AddCors();

			services.AddControllers();
			services.AddCors(options =>
			{
				options.AddPolicy("AllowAllOrigin", builder => builder.AllowAnyOrigin());
			});

			services.AddScoped<IItemRepository, ItemRepository>();
			services.AddScoped<IItemBl, ItemBl>();
			services.AddScoped<IProjectRepository, ProjectRepository>();
			services.AddScoped<IProjectBl, ProjectBl>();
			services.AddScoped<ISprintRepository, SprintRepository>();
			services.AddScoped<ISprintBl, SprintBl>();

			services.AddScoped<IUserRepository, UserRepository>();
			services.AddScoped<IUserBl, UserBl>();
			services.AddScoped<IAccountRepository, AccountRepository>();
			services.AddScoped<IAccountBl, AccountBl>();
			services.AddScoped<IUserRefreshTokenRepository<UserRefreshToken, int>, UserRefreshTokenRepository>();
			services.AddScoped<IJwtTokenBl, JwtTokenBl>();

			services.AddScoped<IMessageBl, MessageBl>();
			services.AddScoped<INotificationBl, NotificationBl>();
			services.AddScoped<ISmtpServiceBl, SendGridSmtpServiceBl>();

			services.AddScoped<ICommentRepository, CommentRepository>();
			services.AddScoped<ICommentBl, CommentBl>();

			services.AddScoped<IItemTypeBl, ItemTypeBl>();
			services.AddScoped<IItemTypeRepository, ItemTypeRepository>();

            services.AddScoped<IItemRelationRepository, ItemRelationRepository>();
            services.AddScoped<IItemRelationBl, ItemRelationBl>();

            services.AddScoped<IStatusBl, StatusBl>();
            services.AddScoped<IStatusRepository, StatusRepository>();

			services.AddScoped<IAvatarBl, AvatarBl>();
			services.AddScoped<IAvatarRepository, AvatarInDbRepository>();

			services.AddScoped<IMemberBl, MemberBl>();
			services.AddScoped<IProjectUserRepository, ProjectUserRepository>();

			services.AddAutoMapper(typeof(Startup));

			services.AddMvcCore().AddApiExplorer();
			services.AddSwaggerGen(options =>
			{
				options.SwaggerDoc("v2", new OpenApiInfo { Title = "Sprint Controller", Version = "v2" });
				options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n
					  Enter 'Bearer' [space] and then your token in the text input below.
					  \r\n\r\nExample: 'Bearer 12345abcdef'",
					Name = "Authorization",
					In = ParameterLocation.Header,
					Type = SecuritySchemeType.ApiKey,
					Scheme = "Bearer"
				});
				options.AddSecurityRequirement(new OpenApiSecurityRequirement()
				{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Type = ReferenceType.SecurityScheme,
								Id = "Bearer"
							},
							Scheme = "oauth2",
							Name = "Bearer",
							In = ParameterLocation.Header,
						},
						new List<string>()
					}
				});
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseSwagger();
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v2/swagger.json", "My API V2");
			});
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			app.UseErrorHandling(env);
			app.UseRouting();

			app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}