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
CREATE TABLE [dbo].[EnumDeploymentIsolationType](
	[ID] [int] NOT NULL,
	[TypeName] [nvarchar](50) NOT NULL,
	[DisplayValue] [nvarchar](50) NOT NULL,
	[CreatedByUserName] [nvarchar](50) NOT NULL,
	[CreatedDateTimeUtc] [datetime2](7) NOT NULL,
	[UpdatedByUserName] [nvarchar](50) NOT NULL,
	[UpdatedDateTimeUtc] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_EnumDeploymentIsolationType] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO

ALTER TABLE [dbo].[EnumDeploymentIsolationType] ADD  CONSTRAINT [DF_EnumDeploymentIsolationType_CreatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [CreatedDateTimeUtc]
GO

ALTER TABLE [dbo].[EnumDeploymentIsolationType] ADD  CONSTRAINT [DF_EnumDeploymentIsolationType_UpdatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [UpdatedDateTimeUtc]
GO

ALTER TABLE dbo.EnumDeploymentIsolationType ADD CONSTRAINT IX_EnumDeploymentIsolationType UNIQUE NONCLUSTERED (TypeName) 
    WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

INSERT INTO EnumDeploymentIsolationType (ID, TypeName, DisplayValue, CreatedDateTimeUtc, CreatedByUserName, UpdatedDateTimeUtc, UpdatedByUserName) 
    VALUES (0, 'IsolatedPerMachine', 'Isolated Per Machine', GETUTCDATE(), 'RoundhousE', GETUTCDATE(), 'RoundhousE')
INSERT INTO EnumDeploymentIsolationType (ID, TypeName, DisplayValue, CreatedDateTimeUtc, CreatedByUserName, UpdatedDateTimeUtc, UpdatedByUserName) 
    VALUES (1, 'IsolatedPerDeployment', 'Isolated Per Deployment', GETUTCDATE(), 'RoundhousE', GETUTCDATE(), 'RoundhousE')
INSERT INTO EnumDeploymentIsolationType (ID, TypeName, DisplayValue, CreatedDateTimeUtc, CreatedByUserName, UpdatedDateTimeUtc, UpdatedByUserName) 
    VALUES (2, 'NoIsolation', 'No Isolation', GETUTCDATE(), 'RoundhousE', GETUTCDATE(), 'RoundhousE')

GO

CREATE TABLE [dbo].[Configuration](
	[ID] [nvarchar](50) NOT NULL,
	[ProjectID] [nvarchar](50) NOT NULL,
	[ConfigurationName] [nvarchar](200) NOT NULL,
	[EnumDeploymentIsolationTypeID] [int] NOT NULL,
	[CreatedDateTimeUtc] [datetime2](7) NOT NULL,
	[CreatedByUserName] [nvarchar](50) NOT NULL,
	[UpdatedDateTimeUtc] [datetime2](7) NOT NULL,
	[UpdatedByUserName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Configuration] PRIMARY KEY 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO

ALTER TABLE [dbo].[Configuration] ADD  CONSTRAINT [DF_Configuration_CreatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [CreatedDateTimeUtc]
GO

ALTER TABLE [dbo].[Configuration] ADD  CONSTRAINT [DF_Configuration_UpdatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [UpdatedDateTimeUtc]
GO

ALTER TABLE [dbo].[Configuration]  WITH CHECK ADD  CONSTRAINT [FK_Configuration_EnumDeploymentIsolationType] FOREIGN KEY([EnumDeploymentIsolationTypeID])
REFERENCES [dbo].[EnumDeploymentIsolationType] ([ID])
GO

ALTER TABLE [dbo].[Configuration] CHECK CONSTRAINT [FK_Configuration_EnumDeploymentIsolationType]
GO

ALTER TABLE [dbo].[Configuration]  WITH CHECK ADD  CONSTRAINT [FK_Configuration_Project] FOREIGN KEY([ProjectID])
REFERENCES [dbo].[Project] ([ID])
GO

ALTER TABLE [dbo].[Configuration] CHECK CONSTRAINT [FK_Configuration_Project]
GO


CREATE TABLE [dbo].[Component](
	[ID] [nvarchar](50) NOT NULL,
	[ProjectID] [nvarchar](50) NOT NULL,
	[ComponentName] [nvarchar](200) NOT NULL,
	[UseConfigurationGroup] [bit] NOT NULL,
	[ConfigurationID] [nvarchar](50) NULL,
	[EnumDeploymentIsolationTypeID] [int] NOT NULL,
	[CreatedDateTimeUtc] [datetime2](7) NOT NULL,
	[CreatedByUserName] [nvarchar](50) NOT NULL,
	[UpdatedDateTimeUtc] [datetime2](7) NOT NULL,
	[UpdatedByUserName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Component] PRIMARY KEY 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO

ALTER TABLE [dbo].[Component] ADD  CONSTRAINT [DF_Component_CreatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [CreatedDateTimeUtc]
GO

ALTER TABLE [dbo].[Component] ADD  CONSTRAINT [DF_Component_UpdatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [UpdatedDateTimeUtc]
GO

ALTER TABLE [dbo].[Component]  WITH CHECK ADD  CONSTRAINT [FK_Component_Configuration] FOREIGN KEY([ConfigurationID])
REFERENCES [dbo].[Configuration] ([ID])
GO

ALTER TABLE [dbo].[Component] CHECK CONSTRAINT [FK_Component_Configuration]
GO

ALTER TABLE [dbo].[Component]  WITH CHECK ADD  CONSTRAINT [FK_Component_EnumDeploymentIsolationType] FOREIGN KEY([EnumDeploymentIsolationTypeID])
REFERENCES [dbo].[EnumDeploymentIsolationType] ([ID])
GO

ALTER TABLE [dbo].[Component] CHECK CONSTRAINT [FK_Component_EnumDeploymentIsolationType]
GO

ALTER TABLE [dbo].[Component]  WITH CHECK ADD  CONSTRAINT [FK_Component_Project] FOREIGN KEY([ProjectID])
REFERENCES [dbo].[Project] ([ID])
GO

ALTER TABLE [dbo].[Component] CHECK CONSTRAINT [FK_Component_Project]
GO

