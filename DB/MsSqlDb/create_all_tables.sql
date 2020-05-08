SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Users](
	[Id] [int] NOT NULL IDENTITY(1,1),
	[Login] [nvarchar](20) NOT NULL,
	[Password] [nvarchar](20) NOT NULL,
	[FirstName] [nvarchar](20) NULL,
	[LastName] [nvarchar](20) NULL,
	[Email] [nvarchar](30) NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Projects](
	[Id] [int] NOT NULL IDENTITY(1,1),
	[Name] [nvarchar](40) NOT NULL,
	[Description] [nvarchar](max) NULL,
 CONSTRAINT [PK_Projects] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[ProjectsUsers](
	[ProjectId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[UserRole] [int] NOT NULL default (1),
	primary key ([ProjectId], [UserId]))
GO

ALTER TABLE [dbo].[ProjectsUsers]  WITH CHECK ADD  CONSTRAINT [FK_Project] FOREIGN KEY([ProjectId])
REFERENCES [dbo].[Projects] ([Id])
GO

ALTER TABLE [dbo].[ProjectsUsers] CHECK CONSTRAINT [FK_Project]
GO

ALTER TABLE [dbo].[ProjectsUsers]  WITH CHECK ADD  CONSTRAINT [FK_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO

ALTER TABLE [dbo].[ProjectsUsers] CHECK CONSTRAINT [FK_User]
GO

CREATE TABLE [dbo].[Sprints](
	[Id] [int] NOT NULL IDENTITY(1,1),
	[ProjectId] [int] NOT NULL,
	[StartDate] [datetime] NULL,
	[ExpireDate] [datetime] NULL,
 CONSTRAINT [PK_Sprints] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Sprints]  WITH CHECK ADD  CONSTRAINT [FK_SProject] FOREIGN KEY([ProjectId])
REFERENCES [dbo].[Projects] ([Id])
GO

ALTER TABLE [dbo].[Sprints] CHECK CONSTRAINT [FK_SProject]
GO

CREATE TABLE [dbo].[Statuses](
	[Id] [int] NOT NULL IDENTITY(1,1),
	[Name] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_Statuses] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[ItemTypes](
	[Id] [int] NOT NULL IDENTITY(1,1),
	[Name] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_ItemTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Items](
	[Id] [int] NOT NULL IDENTITY(1,1),
	[SprintId] [int] NOT NULL,
	[AssignedUserId] [int] NULL,
	[Name] [nvarchar](30)  NOT NULL,
	[Description] [nvarchar](max) NULL,
	[StatusId] [int] NOT NULL default (1),
	[TypeId] [int] NOT NULL default (1),
 CONSTRAINT [PK_Items] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Items]  WITH CHECK ADD  CONSTRAINT [FK_Sprint] FOREIGN KEY([SprintId])
REFERENCES [dbo].[Sprints] ([Id])
GO

ALTER TABLE [dbo].[Items] CHECK CONSTRAINT [FK_Sprint]
GO

ALTER TABLE [dbo].[Items]  WITH CHECK ADD  CONSTRAINT [FK_Status] FOREIGN KEY([StatusId])
REFERENCES [dbo].[Statuses] ([Id])
GO

ALTER TABLE [dbo].[Items] CHECK CONSTRAINT [FK_Status]
GO

ALTER TABLE [dbo].[Items]  WITH CHECK ADD  CONSTRAINT [FK_ItemType] FOREIGN KEY([TypeId])
REFERENCES [dbo].[ItemTypes] ([Id])
GO

ALTER TABLE [dbo].[Items] CHECK CONSTRAINT [FK_Status]
GO

ALTER TABLE [dbo].[Items]  WITH CHECK ADD  CONSTRAINT [FK_AsignedUser] FOREIGN KEY([AssignedUserId])
REFERENCES [dbo].[Users] ([Id])
GO

ALTER TABLE [dbo].[Items] CHECK CONSTRAINT [FK_AsignedUser]
GO


