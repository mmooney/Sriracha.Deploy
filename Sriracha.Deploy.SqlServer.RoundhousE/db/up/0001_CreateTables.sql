CREATE TABLE [dbo].[DeployProject](
	[ID] [nvarchar](50) NOT NULL,
	[ProjectName] [nvarchar](200) NOT NULL,
	[UsesSharedComponentConfiguration] [bit] NOT NULL,
	[CreatedByUserName] [nvarchar](100) NOT NULL,
	[CreatedDateTimeUtc] [datetime2](7) NOT NULL,
	[UpdatedByUserName] [nvarchar](100) NOT NULL,
	[UpdatedDateTimeUtc] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_DeployProject] PRIMARY KEY 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO

ALTER TABLE [dbo].[DeployProject] ADD  CONSTRAINT [DF_DeployProject_ID]  DEFAULT (newid()) FOR [ID]
GO

ALTER TABLE [dbo].[DeployProject] ADD  CONSTRAINT [DF_DeployProject_UsesSharedComponentConfiguration]  DEFAULT ((0)) FOR [UsesSharedComponentConfiguration]
GO

ALTER TABLE [dbo].[DeployProject] ADD  CONSTRAINT [DF_DeployProject_CreatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [CreatedDateTimeUtc]
GO

ALTER TABLE [dbo].[DeployProject] ADD  CONSTRAINT [DF_DeployProject_UpdatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [UpdatedDateTimeUtc]
GO

CREATE NONCLUSTERED INDEX IX_DeployProject_ProjectName ON dbo.DeployProject (ProjectName) 
    WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


CREATE TABLE [dbo].[DeployBranch](
	[ID] [nvarchar](50) NOT NULL,
	[DeployProjectID] [nvarchar](50) NOT NULL,
	[BranchName] [nvarchar](200) NOT NULL,
	[CreatedByUserName] [nvarchar](50) NOT NULL,
	[CreatedDateTimeUtc] [datetime2](7) NOT NULL,
	[UpdatedByUserName] [nvarchar](50) NOT NULL,
	[UpdatedDateTimeUtc] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_DeployBranch] PRIMARY KEY 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO

ALTER TABLE [dbo].[DeployBranch] ADD  CONSTRAINT [DF_DeployBranch_CreatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [CreatedDateTimeUtc]
GO

ALTER TABLE [dbo].[DeployBranch] ADD  CONSTRAINT [DF_DeployBranch_UpdatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [UpdatedDateTimeUtc]
GO

ALTER TABLE [dbo].[DeployBranch]  WITH CHECK ADD  CONSTRAINT [FK_DeployBranch_DeployProject] FOREIGN KEY([DeployProjectID])
REFERENCES [dbo].[DeployProject] ([ID])
GO

ALTER TABLE [dbo].[DeployBranch] CHECK CONSTRAINT [FK_DeployBranch_DeployProject]
GO

CREATE NONCLUSTERED INDEX IX_DeployBranch_DeployProjectID ON dbo.DeployBranch
	(
	DeployProjectID 
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX IX_DeployBranchDeploy_BranchName ON dbo.DeployBranch
	(
	BranchName
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
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

CREATE TABLE [dbo].[DeployConfiguration](
	[ID] [nvarchar](50) NOT NULL,
	[DeployProjectID] [nvarchar](50) NOT NULL,
	[ConfigurationName] [nvarchar](200) NOT NULL,
	[EnumDeploymentIsolationTypeID] [int] NOT NULL,
	[CreatedDateTimeUtc] [datetime2](7) NOT NULL,
	[CreatedByUserName] [nvarchar](50) NOT NULL,
	[UpdatedDateTimeUtc] [datetime2](7) NOT NULL,
	[UpdatedByUserName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_DeployConfiguration] PRIMARY KEY 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO

ALTER TABLE [dbo].[DeployConfiguration] ADD  CONSTRAINT [DF_DeployConfiguration_CreatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [CreatedDateTimeUtc]
GO

ALTER TABLE [dbo].[DeployConfiguration] ADD  CONSTRAINT [DF_DeployConfiguration_UpdatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [UpdatedDateTimeUtc]
GO

ALTER TABLE [dbo].[DeployConfiguration]  WITH CHECK ADD  CONSTRAINT [FK_DeployConfiguration_EnumDeploymentIsolationType] FOREIGN KEY([EnumDeploymentIsolationTypeID])
REFERENCES [dbo].[EnumDeploymentIsolationType] ([ID])
GO

ALTER TABLE [dbo].[DeployConfiguration] CHECK CONSTRAINT [FK_DeployConfiguration_EnumDeploymentIsolationType]
GO

ALTER TABLE [dbo].[DeployConfiguration]  WITH CHECK ADD  CONSTRAINT [FK_DeployConfiguration_DeployProject] FOREIGN KEY([DeployProjectID])
REFERENCES [dbo].[DeployProject] ([ID])
GO

ALTER TABLE [dbo].[DeployConfiguration] CHECK CONSTRAINT [FK_DeployConfiguration_DeployProject]
GO

CREATE NONCLUSTERED INDEX IX_DeployConfiguration_ConfigurationName ON dbo.DeployConfiguration (ConfigurationName) 
	WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE dbo.DeployConfiguration ADD CONSTRAINT
	IX_DeployConfiguration_ID_DeployProjectID UNIQUE NONCLUSTERED  (ID, DeployProjectID) 
	WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

CREATE TABLE [dbo].[DeployComponent](
	[ID] [nvarchar](50) NOT NULL,
	[DeployProjectID] [nvarchar](50) NOT NULL,
	[ComponentName] [nvarchar](200) NOT NULL,
	[UseConfigurationGroup] [bit] NOT NULL,
	[DeployConfigurationID] [nvarchar](50) NULL,
	[EnumDeploymentIsolationTypeID] [int] NOT NULL,
	[CreatedDateTimeUtc] [datetime2](7) NOT NULL,
	[CreatedByUserName] [nvarchar](50) NOT NULL,
	[UpdatedDateTimeUtc] [datetime2](7) NOT NULL,
	[UpdatedByUserName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_DeployComponent] PRIMARY KEY 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO

ALTER TABLE [dbo].[DeployComponent] ADD  CONSTRAINT [DF_DeployComponent_CreatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [CreatedDateTimeUtc]
GO

ALTER TABLE [dbo].[DeployComponent] ADD  CONSTRAINT [DF_DeployComponent_UpdatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [UpdatedDateTimeUtc]
GO

ALTER TABLE [dbo].[DeployComponent]  WITH CHECK ADD  CONSTRAINT [FK_DeployComponent_DeployConfiguration] FOREIGN KEY([DeployConfigurationID])
REFERENCES [dbo].[DeployConfiguration] ([ID])
GO

ALTER TABLE [dbo].[DeployComponent] CHECK CONSTRAINT [FK_DeployComponent_DeployConfiguration]
GO

ALTER TABLE [dbo].[DeployComponent]  WITH CHECK ADD  CONSTRAINT [FK_DeployComponent_EnumDeploymentIsolationType] FOREIGN KEY([EnumDeploymentIsolationTypeID])
REFERENCES [dbo].[EnumDeploymentIsolationType] ([ID])
GO

ALTER TABLE [dbo].[DeployComponent] CHECK CONSTRAINT [FK_DeployComponent_EnumDeploymentIsolationType]
GO

ALTER TABLE [dbo].[DeployComponent]  WITH CHECK ADD  CONSTRAINT [FK_DeployComponent_DeployProject] FOREIGN KEY([DeployProjectID])
REFERENCES [dbo].[DeployProject] ([ID])
GO

ALTER TABLE [dbo].[DeployComponent] CHECK CONSTRAINT [FK_DeployComponent_DeployProject]
GO

ALTER TABLE dbo.DeployComponent ADD CONSTRAINT
	IX_DeployComponent_ID_DeployProjectID UNIQUE NONCLUSTERED  (ID,DeployProjectID) 
    WITH(STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX IX_DeployComponent_ComponentName ON dbo.DeployComponent (ComponentName) 
    WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE TABLE [dbo].[DeployComponentStep](
	[ID] [nvarchar](50) NOT NULL,
	[DeployProjectID] [nvarchar](50) NOT NULL,
	[DeployComponentID] [nvarchar](50) NOT NULL,
	[StepName] [nvarchar](200) NOT NULL,
	[TaskTypeName] [nvarchar](200) NOT NULL,
	[TaskOptionsJson] [nvarchar](max) NULL,
	[SharedDeploymentStepID] [nvarchar](50) NULL,
	[CreatedDateTimeUtc] [datetime2](7) NOT NULL,
	[CreatedByUserName] [nvarchar](50) NOT NULL,
	[UpdatedDateTimeUtc] [datetime2](7) NOT NULL,
	[UpdatedByUserName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_DeployComponentStep] PRIMARY KEY  
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO

ALTER TABLE [dbo].[DeployComponentStep] ADD  CONSTRAINT [DF_DeployComponentStep_CreatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [CreatedDateTimeUtc]
GO

ALTER TABLE [dbo].[DeployComponentStep] ADD  CONSTRAINT [DF_DeployComponentStep_UpdatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [UpdatedDateTimeUtc]
GO

ALTER TABLE [dbo].[DeployComponentStep]  WITH CHECK ADD  CONSTRAINT [FK_DeployComponentStep_DeployComponent] FOREIGN KEY([DeployComponentID], [DeployProjectID])
REFERENCES [dbo].[DeployComponent] ([ID], [DeployProjectID])
GO

ALTER TABLE [dbo].[DeployComponentStep] CHECK CONSTRAINT [FK_DeployComponentStep_DeployComponent]
GO

ALTER TABLE [dbo].[DeployComponentStep]  WITH CHECK ADD  CONSTRAINT [FK_DeployComponentStep_DeployProject] FOREIGN KEY([DeployProjectID])
REFERENCES [dbo].[DeployProject] ([ID])
GO

ALTER TABLE [dbo].[DeployComponentStep] CHECK CONSTRAINT [FK_DeployComponentStep_DeployProject]
GO

CREATE NONCLUSTERED INDEX IX_DeployComponentStep_DeployProjectID ON dbo.DeployComponentStep (DeployProjectID) 
    WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX IX_DeployComponentStep_DeployComponentID ON dbo.DeployComponentStep (DeployComponentID) 
    WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX IX_DeployComponentStep_SharedDeploymentStepID ON dbo.DeployComponentStep (SharedDeploymentStepID) 
    WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE TABLE [dbo].[DeployConfigurationStep](
	[ID] [nvarchar](50) NOT NULL,
	[DeployProjectID] [nvarchar](50) NOT NULL,
	[DeployConfigurationID] [nvarchar](50) NOT NULL,
	[StepName] [nvarchar](200) NOT NULL,
	[TaskTypeName] [nvarchar](200) NOT NULL,
	[TaskOptionsJson] [nvarchar](max) NULL,
	[CreatedDateTimeUtc] [datetime2](7) NOT NULL,
	[CreatedByUserName] [nvarchar](50) NOT NULL,
	[UpdatedDateTimeUtc] [datetime2](7) NOT NULL,
	[UpdatedByUserName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_DeployConfigurationStep] PRIMARY KEY 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO

ALTER TABLE [dbo].[DeployConfigurationStep] ADD  CONSTRAINT [DF_DeployConfigurationStep_CreatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [CreatedDateTimeUtc]
GO

ALTER TABLE [dbo].[DeployConfigurationStep]  WITH CHECK ADD  CONSTRAINT [FK_DeployConfigurationStep_DeployConfiguration] FOREIGN KEY([DeployConfigurationID], [DeployProjectID])
REFERENCES [dbo].[DeployConfiguration] ([ID], [DeployProjectID])
GO

ALTER TABLE [dbo].[DeployConfigurationStep] CHECK CONSTRAINT [FK_DeployConfigurationStep_DeployConfiguration]
GO

ALTER TABLE [dbo].[DeployConfigurationStep]  WITH CHECK ADD  CONSTRAINT [FK_DeployConfigurationStep_DeployProject] FOREIGN KEY([DeployProjectID])
REFERENCES [dbo].[DeployProject] ([ID])
GO

ALTER TABLE [dbo].[DeployConfigurationStep] CHECK CONSTRAINT [FK_DeployConfigurationStep_DeployProject]
GO


CREATE NONCLUSTERED INDEX IX_DeployConfigurationStep_DeployProjectID ON dbo.DeployConfigurationStep (DeployProjectID) 
    WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX IX_DeployConfigurationStep_DeployConfigurationID ON dbo.DeployConfigurationStep (DeployConfigurationID)
    WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE TABLE [dbo].[DeployEnvironment](
	[ID] [nvarchar](50) NOT NULL,
	[DeployProjectID] [nvarchar](50) NOT NULL,
	[EnvironmentName] [nvarchar](50) NOT NULL,
	[CreatedDateTimeUtc] [datetime2](7) NOT NULL,
	[CreatedByUserName] [nvarchar](50) NOT NULL,
	[UpdatedDateTimeUtc] [datetime2](7) NOT NULL,
	[UpdatedByUserName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Environment] PRIMARY KEY 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO

ALTER TABLE [dbo].[DeployEnvironment] ADD  CONSTRAINT [DF_DeployEnvironment_CreatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [CreatedDateTimeUtc]
GO

ALTER TABLE [dbo].[DeployEnvironment] ADD  CONSTRAINT [DF_DeployEnvironment_UpdatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [UpdatedDateTimeUtc]
GO

ALTER TABLE [dbo].[DeployEnvironment]  WITH CHECK ADD  CONSTRAINT [FK_DeployEnvironment_DeployProject] FOREIGN KEY([DeployProjectID])
REFERENCES [dbo].[DeployProject] ([ID])
GO

ALTER TABLE [dbo].[DeployEnvironment] CHECK CONSTRAINT [FK_DeployEnvironment_DeployProject]
GO


CREATE NONCLUSTERED INDEX IX_DeployEnvironment_DeployProjectID ON dbo.DeployEnvironment (DeployProjectID) 
    WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX IX_DeployEnvironment_EnvironmentName ON dbo.DeployEnvironment (EnvironmentName) 
    WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

ALTER TABLE dbo.DeployEnvironment ADD CONSTRAINT
	IX_DeployEnvironment_ID_DeployProjectID UNIQUE NONCLUSTERED  (ID,DeployProjectID) 
    WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]


CREATE TABLE [dbo].[EnumDeployStepParentType](
	[ID] [int] NOT NULL,
	[TypeName] [nvarchar](50) NOT NULL,
	[DisplayValue] [nvarchar](50) NOT NULL,
	[CreatedDateTimeUtc] [datetime2](7) NOT NULL,
	[CreatedByUserName] [nvarchar](50) NOT NULL,
	[UpdatedDateTimeUtc] [datetime2](7) NOT NULL,
	[UpdatedByUserName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_EnumDeployStepParentType] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO

ALTER TABLE [dbo].[EnumDeployStepParentType] ADD  CONSTRAINT [DF_EnumDeployStepParentType_CreatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [CreatedDateTimeUtc]
GO

ALTER TABLE [dbo].[EnumDeployStepParentType] ADD  CONSTRAINT [DF_EnumDeployStepParentType_UpdatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [UpdatedDateTimeUtc]
GO

INSERT INTO EnumDeployStepParentType (ID, TypeName, DisplayValue, CreatedDateTimeUtc, CreatedByUserName, UpdatedDateTimeUtc, UpdatedByUserName) 
    VALUES (0, 'Unknown', 'Unknown', GETUTCDATE(), 'RoundhousE', GETUTCDATE(), 'RoundhousE')
INSERT INTO EnumDeployStepParentType (ID, TypeName, DisplayValue, CreatedDateTimeUtc, CreatedByUserName, UpdatedDateTimeUtc, UpdatedByUserName) 
    VALUES (1, 'Component', 'Component', GETUTCDATE(), 'RoundhousE', GETUTCDATE(), 'RoundhousE')
INSERT INTO EnumDeployStepParentType (ID, TypeName, DisplayValue, CreatedDateTimeUtc, CreatedByUserName, UpdatedDateTimeUtc, UpdatedByUserName) 
    VALUES (2, 'Configuration', 'Configuration', GETUTCDATE(), 'RoundhousE', GETUTCDATE(), 'RoundhousE')

CREATE TABLE [dbo].[DeployEnvironmentConfiguration](
	[ID] [nvarchar](50) NOT NULL,
	[DeployProjectID] [nvarchar](50) NOT NULL,
	[DeployEnvironmentId] [nvarchar](50) NOT NULL,
	[ParentID] [nvarchar](50) NOT NULL,
	[EnumDeployStepParentTypeID] [int] NOT NULL,
	[DeployCredentialsId] [nvarchar](50) NULL,
	[CreatedDateTimeUtc] [datetime2](7) NOT NULL,
	[CreatedByUserName] [nvarchar](50) NOT NULL,
	[UpdatedDateTimeUtc] [datetime2](7) NOT NULL,
	[UpdatedByUserName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_DeployEnvironmentConfiguration] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO

ALTER TABLE [dbo].[DeployEnvironmentConfiguration] ADD  CONSTRAINT [DF_DeployEnvironmentConfiguration_CreatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [CreatedDateTimeUtc]
GO

ALTER TABLE [dbo].[DeployEnvironmentConfiguration] ADD  CONSTRAINT [DF_DeployEnvironmentConfiguration_UpdatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [UpdatedDateTimeUtc]
GO

ALTER TABLE [dbo].[DeployEnvironmentConfiguration]  WITH CHECK ADD  CONSTRAINT [FK_DeployEnvironmentConfiguration_EnumDeployStepParentType] FOREIGN KEY([EnumDeployStepParentTypeID])
REFERENCES [dbo].[EnumDeployStepParentType] ([ID])
GO

ALTER TABLE [dbo].[DeployEnvironmentConfiguration] CHECK CONSTRAINT [FK_DeployEnvironmentConfiguration_EnumDeployStepParentType]
GO

ALTER TABLE [dbo].[DeployEnvironmentConfiguration]  WITH CHECK ADD  CONSTRAINT [FK_DeployEnvironmentConfiguration_DeployEnvironment] FOREIGN KEY([DeployEnvironmentId], [DeployProjectID])
REFERENCES [dbo].[DeployEnvironment] ([ID], [DeployProjectID])
GO

ALTER TABLE [dbo].[DeployEnvironmentConfiguration] CHECK CONSTRAINT [FK_DeployEnvironmentConfiguration_DeployEnvironment]
GO


