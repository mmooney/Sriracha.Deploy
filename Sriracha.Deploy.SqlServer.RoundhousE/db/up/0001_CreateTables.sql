CREATE TABLE [dbo].[Project](
	[ID] [nvarchar](50) NOT NULL,
	[ProjectName] [nvarchar](200) NOT NULL,
	[UsesSharedComponentConfiguration] [bit] NOT NULL,
	[CreatedByUserName] [nvarchar](100) NOT NULL,
	[CreatedDateTimeUtc] [datetime2](7) NOT NULL,
	[UpdatedByUserName] [nvarchar](100) NOT NULL,
	[UpdatedDateTimeUtc] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Project] PRIMARY KEY 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO

ALTER TABLE [dbo].[Project] ADD  CONSTRAINT [DF_Project_ID]  DEFAULT (newid()) FOR [ID]
GO

ALTER TABLE [dbo].[Project] ADD  CONSTRAINT [DF_Project_UsesSharedComponentConfiguration]  DEFAULT ((0)) FOR [UsesSharedComponentConfiguration]
GO

ALTER TABLE [dbo].[Project] ADD  CONSTRAINT [DF_Project_CreatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [CreatedDateTimeUtc]
GO

ALTER TABLE [dbo].[Project] ADD  CONSTRAINT [DF_Project_UpdatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [UpdatedDateTimeUtc]
GO

CREATE TABLE [dbo].[Branch](
	[ID] [nvarchar](50) NOT NULL,
	[ProjectID] [nvarchar](50) NOT NULL,
	[BranchName] [nvarchar](200) NOT NULL,
	[CreatedByUserName] [nvarchar](50) NOT NULL,
	[CreatedDateTimeUtc] [datetime2](7) NOT NULL,
	[UpdatedByUserName] [nvarchar](50) NOT NULL,
	[UpdatedDateTimeUtc] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Branch] PRIMARY KEY 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO

ALTER TABLE [dbo].[Branch] ADD  CONSTRAINT [DF_Branch_CreatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [CreatedDateTimeUtc]
GO

ALTER TABLE [dbo].[Branch] ADD  CONSTRAINT [DF_Branch_UpdatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [UpdatedDateTimeUtc]
GO

ALTER TABLE [dbo].[Branch]  WITH CHECK ADD  CONSTRAINT [FK_Branch_Project] FOREIGN KEY([ProjectID])
REFERENCES [dbo].[Project] ([ID])
GO

ALTER TABLE [dbo].[Branch] CHECK CONSTRAINT [FK_Branch_Project]
GO
