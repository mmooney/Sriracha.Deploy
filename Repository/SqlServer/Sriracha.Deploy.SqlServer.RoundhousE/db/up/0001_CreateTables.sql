CREATE TABLE [dbo].[DeployProject](
	[ID] [nvarchar](50) NOT NULL,
	[ProjectName] [nvarchar](200) NOT NULL,
	[UsesSharedComponentConfiguration] [bit] NOT NULL,
	[CreatedByUserName] [nvarchar](100) NOT NULL,
	[CreatedDateTimeUtc] [datetime2](7) NOT NULL,
	[UpdatedByUserName] [nvarchar](100) NOT NULL,
	[UpdatedDateTimeUtc] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_DeployProject] PRIMARY KEY NONCLUSTERED
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
 CONSTRAINT [PK_DeployBranch] PRIMARY KEY NONCLUSTERED
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
 CONSTRAINT [PK_DeployConfiguration] PRIMARY KEY NONCLUSTERED 
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
CREATE NONCLUSTERED INDEX IX_DeployConfiguration_DeployProjectID ON dbo.DeployConfiguration (DeployProjectID) 
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
 CONSTRAINT [PK_DeployComponent] PRIMARY KEY NONCLUSTERED 
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
CREATE NONCLUSTERED INDEX IX_DeployComponent_DeployProjectID ON dbo.DeployComponent (DeployProjectID) 
    WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE TABLE [dbo].[DeployComponentStep](
	[ID] [nvarchar](50) NOT NULL,
	[DeployProjectID] [nvarchar](50) NOT NULL,
	[DeployComponentID] [nvarchar](50) NOT NULL,
	[StepName] [nvarchar](200) NOT NULL,
	[TaskTypeName] [nvarchar](200) NOT NULL,
	[TaskOptionsJson] [nvarchar](max) NULL,
    [OrderNumber] [int] NOT NULL,
	[SharedDeploymentStepID] [nvarchar](50) NULL,
	[CreatedDateTimeUtc] [datetime2](7) NOT NULL,
	[CreatedByUserName] [nvarchar](50) NOT NULL,
	[UpdatedDateTimeUtc] [datetime2](7) NOT NULL,
	[UpdatedByUserName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_DeployComponentStep] PRIMARY KEY NONCLUSTERED  
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
    [OrderNumber] [int] NOT NULL,
	[CreatedDateTimeUtc] [datetime2](7) NOT NULL,
	[CreatedByUserName] [nvarchar](50) NOT NULL,
	[UpdatedDateTimeUtc] [datetime2](7) NOT NULL,
	[UpdatedByUserName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_DeployConfigurationStep] PRIMARY KEY NONCLUSTERED 
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
	[EnvironmentName] [nvarchar](200) NOT NULL,
	[CreatedDateTimeUtc] [datetime2](7) NOT NULL,
	[CreatedByUserName] [nvarchar](50) NOT NULL,
	[UpdatedDateTimeUtc] [datetime2](7) NOT NULL,
	[UpdatedByUserName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Environment] PRIMARY KEY NONCLUSTERED 
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
	[DeployEnvironmentID] [nvarchar](50) NOT NULL,
	[ParentID] [nvarchar](50) NOT NULL,
	[EnumDeployStepParentTypeID] [int] NOT NULL,
	[DeployCredentialsId] [nvarchar](50) NULL,
	[CreatedDateTimeUtc] [datetime2](7) NOT NULL,
	[CreatedByUserName] [nvarchar](50) NOT NULL,
	[UpdatedDateTimeUtc] [datetime2](7) NOT NULL,
	[UpdatedByUserName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_DeployEnvironmentConfiguration] PRIMARY KEY NONCLUSTERED 
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

ALTER TABLE [dbo].[DeployEnvironmentConfiguration]  WITH CHECK ADD  CONSTRAINT [FK_DeployEnvironmentConfiguration_DeployEnvironment] FOREIGN KEY([DeployEnvironmentID], [DeployProjectID])
REFERENCES [dbo].[DeployEnvironment] ([ID], [DeployProjectID])
GO

ALTER TABLE [dbo].[DeployEnvironmentConfiguration] CHECK CONSTRAINT [FK_DeployEnvironmentConfiguration_DeployEnvironment]
GO


CREATE NONCLUSTERED INDEX IX_DeployEnvironmentConfiguration_DeployProjectID ON dbo.DeployEnvironmentConfiguration (DeployProjectID) 
    WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX IX_DeployEnvironmentConfiguration_DeployEnvironmentID ON dbo.DeployEnvironmentConfiguration (DeployEnvironmentID) 
    WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

ALTER TABLE dbo.DeployEnvironmentConfiguration ADD CONSTRAINT
	IX_DeployEnvironmentConfiguration_ID_DeployProjectID_DeployEnvironmentID UNIQUE NONCLUSTERED  (ID, DeployProjectID,DeployEnvironmentID) 
    WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

CREATE TABLE [dbo].[DeployMachine](
	[ID] [nvarchar](50) NOT NULL,
	[DeployProjectID] [nvarchar](50) NOT NULL,
	[DeployEnvironmentID] [nvarchar](50) NOT NULL,
	[EnvironmentName] [nvarchar](200) NOT NULL,
	[DeployEnvironmentConfigurationID] [nvarchar](50) NOT NULL,
	[MachineName] [nvarchar](200) NOT NULL,
	[CreatedDateTimeUtc] [datetime2](7) NOT NULL,
	[CreatedByUserName] [nvarchar](50) NOT NULL,
	[UpdatedDateTimeUtc] [datetime2](7) NOT NULL,
	[UpdatedByUserName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_DeployMachine] PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO

ALTER TABLE [dbo].[DeployMachine] ADD  CONSTRAINT [DF_DeployMachine_CreatedByUserName]  DEFAULT (getutcdate()) FOR [CreatedByUserName]
GO

ALTER TABLE [dbo].[DeployMachine] ADD  CONSTRAINT [DF_DeployMachine_UpdatedByUserName]  DEFAULT (getutcdate()) FOR [UpdatedByUserName]
GO

ALTER TABLE [dbo].[DeployMachine]  WITH CHECK ADD  CONSTRAINT [FK_DeployMachine_DeployEnvironmentConfiguration] FOREIGN KEY([DeployEnvironmentConfigurationID], [DeployProjectID], [DeployEnvironmentID])
REFERENCES [dbo].[DeployEnvironmentConfiguration] ([ID], [DeployProjectID], [DeployEnvironmentID])
GO

ALTER TABLE [dbo].[DeployMachine] CHECK CONSTRAINT [FK_DeployMachine_DeployEnvironmentConfiguration]
GO

CREATE NONCLUSTERED INDEX IX_DeployMachine_DeployProjectID ON dbo.DeployMachine (DeployProjectID) 
    WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX IX_DeployMachine_DeployEnvironmentID ON dbo.DeployMachine (DeployEnvironmentID) 
    WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX IX_DeployMachine_DeployEnvironmentConfigurationID ON dbo.DeployMachine (DeployEnvironmentConfigurationID) 
    WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE TABLE [dbo].[DeployMachineConfigurationValue](
	[ID] [nvarchar](50) NOT NULL,
	[DeployMachineID] [nvarchar](50) NOT NULL,
	[ConfigurationName] [nvarchar](200) NOT NULL,
	[ConfigurationValue] [nvarchar](500) NOT NULL,
	[CreatedByUserName] [nvarchar](50) NOT NULL,
	[CreatedDateTimeUtc] [datetime2](7) NOT NULL,
	[UpdatedByUserName] [nvarchar](50) NOT NULL,
	[UpdatedDateTimeUtc] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_DeployMachineConfigurationValue] PRIMARY KEY NONCLUSTERED
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO

ALTER TABLE [dbo].[DeployMachineConfigurationValue] ADD  CONSTRAINT [DF_DeployMachineConfigurationValue_CreatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [CreatedDateTimeUtc]
GO

ALTER TABLE [dbo].[DeployMachineConfigurationValue] ADD  CONSTRAINT [DF_DeployMachineConfigurationValue_UpdatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [UpdatedDateTimeUtc]
GO

ALTER TABLE [dbo].[DeployMachineConfigurationValue]  WITH CHECK ADD  CONSTRAINT [FK_DeployMachineConfigurationValue_DeployMachine] FOREIGN KEY([DeployMachineID])
REFERENCES [dbo].[DeployMachine] ([ID])
GO

ALTER TABLE [dbo].[DeployMachineConfigurationValue] CHECK CONSTRAINT [FK_DeployMachineConfigurationValue_DeployMachine]
GO

CREATE NONCLUSTERED INDEX IX_DeployMachineConfigurationValue_DeployMachineID ON dbo.DeployMachineConfigurationValue (DeployMachineID) 
    WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

ALTER TABLE dbo.DeployMachineConfigurationValue ADD CONSTRAINT
	IX_DeployMachineConfigurationValue_UniqueName UNIQUE NONCLUSTERED  (DeployMachineID,ConfigurationName) 
    WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

CREATE TABLE [dbo].[DeployEnvironmentConfigurationValue](
	[ID] [nvarchar](50) NOT NULL,
	[DeployEnvironmentConfigurationID] [nvarchar](50) NOT NULL,
	[ConfigurationName] [nvarchar](200) NOT NULL,
	[ConfigurationValue] [nvarchar](500) NOT NULL,
	[CreatedByUserName] [nvarchar](50) NOT NULL,
	[CreatedDateTimeUtc] [datetime2](7) NOT NULL,
	[UpdatedByUserName] [nvarchar](50) NOT NULL,
	[UpdatedDateTimeUtc] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_DeployEnvironmentConfigurationValue] PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO

ALTER TABLE [dbo].[DeployEnvironmentConfigurationValue] ADD  CONSTRAINT [DF_DeployEnvironmentConfigurationValue_CreatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [CreatedDateTimeUtc]
GO

ALTER TABLE [dbo].[DeployEnvironmentConfigurationValue] ADD  CONSTRAINT [DF_DeployEnvironmentConfigurationValue_UpdatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [UpdatedDateTimeUtc]
GO

ALTER TABLE [dbo].[DeployEnvironmentConfigurationValue]  WITH CHECK ADD  CONSTRAINT [FK_DeployEnvironmentConfigurationValue_DeployEnvironmentConfiguration] FOREIGN KEY([DeployEnvironmentConfigurationID])
REFERENCES [dbo].[DeployEnvironmentConfiguration] ([ID])
GO

ALTER TABLE [dbo].[DeployEnvironmentConfigurationValue] CHECK CONSTRAINT [FK_DeployEnvironmentConfigurationValue_DeployEnvironmentConfiguration]
GO

CREATE NONCLUSTERED INDEX IX_DeployEnvironmentConfigurationValue_DeployEnvironmentConfigurationValue ON dbo.DeployEnvironmentConfigurationValue (DeployEnvironmentConfigurationID) 
    WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

ALTER TABLE dbo.DeployEnvironmentConfigurationValue ADD CONSTRAINT
	IX_DeployEnvironmentConfigurationValue_UniqueName UNIQUE NONCLUSTERED  (DeployEnvironmentConfigurationID,ConfigurationName) 
    WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO


--------------Build

CREATE TABLE [dbo].[DeployBuild](
	[ID] [nvarchar](50) NOT NULL,
	[ProjectID] [nvarchar](50) NOT NULL,
	[ProjectName] [nvarchar](200) NOT NULL,
	[ProjectBranchID] [nvarchar](50) NOT NULL,
	[ProjectBranchName] [nvarchar](200) NOT NULL,
	[ProjectComponentID] [nvarchar](50) NOT NULL,
	[ProjectComponentName] [nvarchar](200) NOT NULL,
	[FileID] [nvarchar](50) NOT NULL,
	[Version] [nvarchar](50) NOT NULL,
	[CreatedDateTimeUtc] [datetime2](7) NOT NULL,
	[CreatedByUserName] [nvarchar](50) NOT NULL,
	[UpdatedDateTimeUtc] [datetime2](7) NOT NULL,
	[UpdatedByUserName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_DeployBuild] PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO

ALTER TABLE [dbo].[DeployBuild] ADD  CONSTRAINT [DF_DeployBuild_CreatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [CreatedDateTimeUtc]
GO

ALTER TABLE [dbo].[DeployBuild] ADD  CONSTRAINT [DF_DeployBuild_UpdatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [UpdatedDateTimeUtc]
GO


CREATE NONCLUSTERED INDEX IDX_DeployBuild_IDFields
ON [dbo].[DeployBuild] ([ProjectID],[ProjectBranchID],[ProjectComponentID],[Version])
GO
--------------------
CREATE TABLE [dbo].[EnumDeployStatus](
	[ID] [int] NOT NULL,
	[TypeName] [nvarchar](50) NOT NULL,
	[DisplayValue] [nvarchar](50) NOT NULL,
	[CreatedByUserName] [nvarchar](50) NOT NULL,
	[CreatedDateTimeUtc] [datetime2](7) NOT NULL,
	[UpdatedByUserName] [nvarchar](50) NOT NULL,
	[UpdatedDateTimeUtc] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_EnumDeployStatus] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO

ALTER TABLE [dbo].[EnumDeployStatus] ADD  CONSTRAINT [DF_EnumDeployStatus_CreatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [CreatedDateTimeUtc]
GO

ALTER TABLE [dbo].[EnumDeployStatus] ADD  CONSTRAINT [DF_EnumDeployStatus_UpdatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [UpdatedDateTimeUtc]
GO

INSERT INTO EnumDeployStatus (ID, TypeName, DisplayValue, CreatedDateTimeUtc, CreatedByUserName, UpdatedDateTimeUtc, UpdatedByUserName)
    VALUES (0, 'Unknown', '', GETUTCDATE(), 'RoundhousE', GETUTCDATE(), 'RoundhousE')
INSERT INTO EnumDeployStatus (ID, TypeName, DisplayValue, CreatedDateTimeUtc, CreatedByUserName, UpdatedDateTimeUtc, UpdatedByUserName)
    VALUES (1, 'Requested', 'Requested', GETUTCDATE(), 'RoundhousE', GETUTCDATE(), 'RoundhousE')
INSERT INTO EnumDeployStatus (ID, TypeName, DisplayValue, CreatedDateTimeUtc, CreatedByUserName, UpdatedDateTimeUtc, UpdatedByUserName)
    VALUES (2, 'Approved', 'Approved', GETUTCDATE(), 'RoundhousE', GETUTCDATE(), 'RoundhousE')
INSERT INTO EnumDeployStatus (ID, TypeName, DisplayValue, CreatedDateTimeUtc, CreatedByUserName, UpdatedDateTimeUtc, UpdatedByUserName)
    VALUES (3, 'Rejected', 'Rejected', GETUTCDATE(), 'RoundhousE', GETUTCDATE(), 'RoundhousE')
INSERT INTO EnumDeployStatus (ID, TypeName, DisplayValue, CreatedDateTimeUtc, CreatedByUserName, UpdatedDateTimeUtc, UpdatedByUserName)
    VALUES (4, 'NotStarted', 'Not Started', GETUTCDATE(), 'RoundhousE', GETUTCDATE(), 'RoundhousE')
INSERT INTO EnumDeployStatus (ID, TypeName, DisplayValue, CreatedDateTimeUtc, CreatedByUserName, UpdatedDateTimeUtc, UpdatedByUserName)
    VALUES (5, 'InProcess', 'In Process', GETUTCDATE(), 'RoundhousE', GETUTCDATE(), 'RoundhousE')
INSERT INTO EnumDeployStatus (ID, TypeName, DisplayValue, CreatedDateTimeUtc, CreatedByUserName, UpdatedDateTimeUtc, UpdatedByUserName)
    VALUES (6, 'Warning', 'Warning', GETUTCDATE(), 'RoundhousE', GETUTCDATE(), 'RoundhousE')
INSERT INTO EnumDeployStatus (ID, TypeName, DisplayValue, CreatedDateTimeUtc, CreatedByUserName, UpdatedDateTimeUtc, UpdatedByUserName)
    VALUES (7, 'Success', 'Success', GETUTCDATE(), 'RoundhousE', GETUTCDATE(), 'RoundhousE')
INSERT INTO EnumDeployStatus (ID, TypeName, DisplayValue, CreatedDateTimeUtc, CreatedByUserName, UpdatedDateTimeUtc, UpdatedByUserName)
    VALUES (8, 'Error', 'Error', GETUTCDATE(), 'RoundhousE', GETUTCDATE(), 'RoundhousE')
INSERT INTO EnumDeployStatus (ID, TypeName, DisplayValue, CreatedDateTimeUtc, CreatedByUserName, UpdatedDateTimeUtc, UpdatedByUserName)
    VALUES (9, 'Cancelled', 'Cancelled', GETUTCDATE(), 'RoundhousE', GETUTCDATE(), 'RoundhousE')
INSERT INTO EnumDeployStatus (ID, TypeName, DisplayValue, CreatedDateTimeUtc, CreatedByUserName, UpdatedDateTimeUtc, UpdatedByUserName)
    VALUES (10, 'OfflineRequested', 'Offline Requested', GETUTCDATE(), 'RoundhousE', GETUTCDATE(), 'RoundhousE')
INSERT INTO EnumDeployStatus (ID, TypeName, DisplayValue, CreatedDateTimeUtc, CreatedByUserName, UpdatedDateTimeUtc, UpdatedByUserName)
    VALUES (11, 'OfflineComplete', 'Offline Deployment Complete', GETUTCDATE(), 'RoundhousE', GETUTCDATE(), 'RoundhousE')
GO

CREATE TABLE [dbo].[DeployState](
	[ID] [nvarchar](50) NOT NULL,
	[DeployBatchRequestItemID] [nvarchar](50) NOT NULL,
	[EnumDeployStatusID] [int] NOT NULL,
	[ProjectID] [nvarchar](50) NOT NULL,
	[BranchID] [nvarchar](50) NOT NULL,
	[BuildID] [nvarchar](50) NOT NULL,
	[EnvironmentID] [nvarchar](50) NOT NULL,
	[EnvironmentName] [nvarchar](200) NOT NULL,
	[ComponentID] [nvarchar](50) NOT NULL,
	[BranchJson] [nvarchar](max) NOT NULL,
	[BuildJson] [nvarchar](max) NOT NULL,
	[EnvironmentJson] [nvarchar](max) NOT NULL,
	[ComponentJson] [nvarchar](max) NOT NULL,
	[MessageListJson] [nvarchar](max) NULL,
	[SubmittedDateTimeUtc] [datetime2](7) NOT NULL,
	[DeploymentStartedDateTimeUtc] [datetime2](7) NULL,
	[DeploymentCompleteDateTimeUtc] [datetime2](7) NULL,
	[ErrorDetails] [nvarchar](max) NULL,
	[SortableVersion] [nvarchar](200) NOT NULL,
	[CreatedByUserName] [nvarchar](50) NOT NULL,
	[CreatedDateTimeUtc] [datetime2](7) NOT NULL,
	[UpdatedByUserName] [nvarchar](50) NOT NULL,
	[UpdatedDateTimeUtc] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_DeployState] PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO

ALTER TABLE [dbo].[DeployState] ADD  CONSTRAINT [DF_DeployState_CreatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [CreatedDateTimeUtc]
GO

ALTER TABLE [dbo].[DeployState] ADD  CONSTRAINT [DF_DeployState_UpdatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [UpdatedDateTimeUtc]
GO

ALTER TABLE [dbo].[DeployState]  WITH CHECK ADD  CONSTRAINT [FK_DeployState_EnumDeployStatus] FOREIGN KEY([EnumDeployStatusID])
REFERENCES [dbo].[EnumDeployStatus] ([ID])
GO

ALTER TABLE [dbo].[DeployState] CHECK CONSTRAINT [FK_DeployState_EnumDeployStatus]
GO

CREATE NONCLUSTERED INDEX IDX_DeployState_ProjectID
ON [dbo].[DeployState] ([ProjectID])
GO


CREATE TABLE [dbo].[DeployStateMachine](
	[ID] [nvarchar](50) NOT NULL,
	[DeployStateID] [nvarchar](50) NOT NULL,
	[MachineID] [nvarchar](50) NOT NULL,
	[MachineName] [nvarchar](200) NOT NULL,
	[MachineJson] [nvarchar](max) NOT NULL,
	[CreatedByUserName] [nvarchar](50) NOT NULL,
	[CreatedDateTimeUtc] [datetime2](7) NOT NULL,
	[UpdatedByUserName] [nvarchar](50) NOT NULL,
	[UpdatedDateTimeUtc] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_DeployStateMachine] PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO

ALTER TABLE [dbo].[DeployStateMachine] ADD  CONSTRAINT [DF_DeployStateMachine_CreatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [CreatedDateTimeUtc]
GO

ALTER TABLE [dbo].[DeployStateMachine] ADD  CONSTRAINT [DF_DeployStateMachine_UpdatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [UpdatedDateTimeUtc]
GO

ALTER TABLE [dbo].[DeployStateMachine]  WITH CHECK ADD  CONSTRAINT [FK_DeployStateMachine_DeployState] FOREIGN KEY([DeployStateID])
REFERENCES [dbo].[DeployState] ([ID])
GO

ALTER TABLE [dbo].[DeployStateMachine] CHECK CONSTRAINT [FK_DeployStateMachine_DeployState]
GO


CREATE NONCLUSTERED INDEX IX_DeployStateMachine_DeployStateID ON dbo.DeployStateMachine
	(
	DeployStateID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX IDX_DeployStateMachine_MachineName
    ON [dbo].[DeployStateMachine] ([MachineName])
GO

CREATE TABLE [dbo].[DeployBuildPurgeRule](
	[ID] [nvarchar](50) NOT NULL,
	[ProjectID] [nvarchar](50) NULL,
	[BuildRetentionMinutes] [int] NULL,
	[EnvironmentIdListJson] [ntext] NULL,
	[EnvironmentNameListJson] [ntext] NULL,
	[MachineIdListJson] [ntext] NULL,
	[MachineNameListJson] [ntext] NULL,
	[CreatedDateTimeUtc] [datetime2](7) NOT NULL,
	[CreatedByUserName] [nvarchar](50) NOT NULL,
	[UpdatedDateTimeUtc] [datetime2](7) NOT NULL,
	[UpdatedByUserName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_DeployBuildPurgeRule] PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)
GO

ALTER TABLE [dbo].[DeployBuildPurgeRule] ADD  CONSTRAINT [DF_DeployBuildPurgeRule_ID]  DEFAULT (newid()) FOR [ID]
GO

ALTER TABLE [dbo].[DeployBuildPurgeRule] ADD  CONSTRAINT [DF_DeployBuildPurgeRule_CreatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [CreatedDateTimeUtc]
GO

ALTER TABLE [dbo].[DeployBuildPurgeRule] ADD  CONSTRAINT [DF_DeployBuildPurgeRule_UpdatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [UpdatedDateTimeUtc]
GO


CREATE TABLE [dbo].[EnumCleanupTaskType](
	[ID] [int] NOT NULL,
	[TypeName] [nvarchar](50) NOT NULL,
	[DisplayValue] [nvarchar](50) NOT NULL,
	[CreatedByUserName] [nvarchar](50) NOT NULL,
	[CreatedDateTimeUtc] [datetime2](7) NOT NULL,
	[UpdatedByUserName] [nvarchar](50) NOT NULL,
	[UpdatedDateTimeUtc] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_EnumCleanupTaskType] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO

ALTER TABLE [dbo].[EnumCleanupTaskType] ADD  CONSTRAINT [DF_EnumCleanupTaskType_CreatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [CreatedDateTimeUtc]
GO

ALTER TABLE [dbo].[EnumCleanupTaskType] ADD  CONSTRAINT [DF_EnumCleanupTaskType_UpdatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [UpdatedDateTimeUtc]
GO

INSERT INTO EnumCleanupTaskType (ID, TypeName, DisplayValue, CreatedByUserName, CreatedDateTimeUtc, UpdatedByUserName, UpdatedDateTimeUtc)
    VALUES (0, 'Folder', 'Folder', 'RoundhousE', GETUTCDATE(), 'RoundhousE', GETUTCDATE())
GO

CREATE TABLE [dbo].[EnumQueueStatus](
	[ID] [int] NOT NULL,
	[TypeName] [nvarchar](50) NOT NULL,
	[DisplayValue] [nvarchar](50) NOT NULL,
	[CreatedByUserName] [nvarchar](50) NOT NULL,
	[CreatedDateTimeUtc] [datetime2](7) NOT NULL,
	[UpdatedByUserName] [nvarchar](50) NOT NULL,
	[UpdatedDateTimeUtc] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_EnumQueueStatus] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO

ALTER TABLE [dbo].[EnumQueueStatus] ADD  CONSTRAINT [DF_EnumQueueStatus_CreatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [CreatedDateTimeUtc]
GO

ALTER TABLE [dbo].[EnumQueueStatus] ADD  CONSTRAINT [DF_EnumQueueStatus_UpdatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [UpdatedDateTimeUtc]
GO

INSERT INTO EnumQueueStatus (ID, TypeName, DisplayValue, CreatedByUserName, CreatedDateTimeUtc, UpdatedByUserName, UpdatedDateTimeUtc)
    VALUES (0, 'New ', 'New', 'RoundhousE', GETUTCDATE(), 'RoundhousE', GETUTCDATE())
INSERT INTO EnumQueueStatus (ID, TypeName, DisplayValue, CreatedByUserName, CreatedDateTimeUtc, UpdatedByUserName, UpdatedDateTimeUtc)
    VALUES (1, 'InProcess', 'InProcess', 'RoundhousE', GETUTCDATE(), 'RoundhousE', GETUTCDATE())
INSERT INTO EnumQueueStatus (ID, TypeName, DisplayValue, CreatedByUserName, CreatedDateTimeUtc, UpdatedByUserName, UpdatedDateTimeUtc)
    VALUES (2, 'Completed', 'Completed', 'RoundhousE', GETUTCDATE(), 'RoundhousE', GETUTCDATE())
INSERT INTO EnumQueueStatus (ID, TypeName, DisplayValue, CreatedByUserName, CreatedDateTimeUtc, UpdatedByUserName, UpdatedDateTimeUtc)
    VALUES (3, 'Error', 'Error', 'RoundhousE', GETUTCDATE(), 'RoundhousE', GETUTCDATE())
GO

CREATE TABLE [dbo].[DeployCleanupTaskData](
	[ID] [nvarchar](50) NOT NULL,
	[EnumCleanupTaskTypeID] [int] NOT NULL,
	[MachineName] [nvarchar](50) NOT NULL,
	[FolderPath] [nvarchar](500) NOT NULL,
	[AgeMinutes] [int] NOT NULL,
	[TargetCleanupDateTimeUtc] [datetime2](7) NOT NULL,
	[EnumQueueStatusID] [int] NOT NULL,
	[StartedDateTimeUtc] [datetime2](7) NULL,
	[CompletedDateTimeUtc] [datetime2](7) NULL,
	[ErrorDetails] [ntext] NULL,
	[CreatedDateTimeUtc] [datetime2](7) NOT NULL,
	[CreatedByUserName] [nvarchar](50) NOT NULL,
	[UpdatedDateTimeUtc] [datetime2](7) NOT NULL,
	[UpdatedByUserName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_DeployCleanupTaskData] PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO

ALTER TABLE [dbo].[DeployCleanupTaskData] ADD  CONSTRAINT [DF_DeployCleanupTaskData_CreatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [CreatedDateTimeUtc]
GO

ALTER TABLE [dbo].[DeployCleanupTaskData] ADD  CONSTRAINT [DF_DeployCleanupTaskData_UpdatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [UpdatedDateTimeUtc]
GO

ALTER TABLE dbo.DeployCleanupTaskData ADD CONSTRAINT
	FK_DeployCleanupTaskData_EnumCleanupTaskType FOREIGN KEY
	(EnumCleanupTaskTypeID) REFERENCES dbo.EnumCleanupTaskType
	(ID) ON UPDATE  NO ACTION 
    ON DELETE  NO ACTION 
GO

ALTER TABLE dbo.DeployCleanupTaskData ADD CONSTRAINT
	FK_DeployCleanupTaskData_EnumDeployStatus FOREIGN KEY
	(EnumQueueStatusID) REFERENCES dbo.EnumDeployStatus
	(ID) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
GO

CREATE TABLE [dbo].[DeployCredential](
	[ID] [nvarchar](50) NOT NULL,
	[Domain] [nvarchar](50) NOT NULL,
	[UserName] [nvarchar](50) NOT NULL,
	[EncryptedPassword] [nvarchar](500) NOT NULL,
	[CreatedDateTimeUtc] [datetime2](7) NOT NULL,
	[CreatedByUserName] [nvarchar](50) NOT NULL,
	[UpdatedDateTimeUtc] [datetime2](7) NOT NULL,
	[UpdatedByUserName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_DeployCredential] PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO

ALTER TABLE [dbo].[DeployCredential] ADD  CONSTRAINT [DF_DeployCredential_CreatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [CreatedDateTimeUtc]
GO

ALTER TABLE [dbo].[DeployCredential] ADD  CONSTRAINT [DF_DeployCredential_UpdatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [UpdatedDateTimeUtc]
GO

CREATE TABLE [dbo].[DeployBatchRequest](
	[ID] [nvarchar](50) NOT NULL,
	[SubmittedDateTimeUtc] [datetime2](7) NOT NULL,
	[SubmittedByUserName] [nvarchar](50) NOT NULL,
	[ItemListJson] [ntext] NOT NULL,
	[EnumDeployStatusID] [int] NOT NULL,
	[StartedDateTimeUtc] [datetime2](7) NULL,
	[CompleteDateTimeUtc] [datetime2](7) NULL,
	[ErrorDetails] [ntext] NULL,
	[LastStatusMessage] [nvarchar](500) NULL,
	[DeploymentLabel] [nvarchar](200) NULL,
	[CancelRequested] [bit] NOT NULL,
	[CancelMessage] [nvarchar](500) NULL,
	[ResumeRequested] [bit] NOT NULL,
	[ResumeMessage] [nvarchar](200) NULL,
	[MessageListJson] [ntext] NULL,
	[CreatedDateTimeUtc] [datetime2](7) NOT NULL,
	[CreatedByUserName] [nvarchar](50) NOT NULL,
	[UpdatedDateTimeUtc] [datetime2](7) NOT NULL,
	[UpdatedByUserName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_DeployBatchRequest] PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO

ALTER TABLE [dbo].[DeployBatchRequest] ADD  CONSTRAINT [DF_DeployBatchRequest_CancelRequested]  DEFAULT ((0)) FOR [CancelRequested]
GO

ALTER TABLE [dbo].[DeployBatchRequest] ADD  CONSTRAINT [DF_DeployBatchRequest_ResumeRequested]  DEFAULT ((0)) FOR [ResumeRequested]
GO

ALTER TABLE [dbo].[DeployBatchRequest] ADD  CONSTRAINT [DF_DeployBatchRequest_CreatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [CreatedDateTimeUtc]
GO

ALTER TABLE [dbo].[DeployBatchRequest] ADD  CONSTRAINT [DF_DeployBatchRequest_UpdatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [UpdatedDateTimeUtc]
GO

ALTER TABLE dbo.DeployBatchRequest ADD CONSTRAINT
	FK_DeployBatchRequest_EnumDeployStatus FOREIGN KEY
	(EnumDeployStatusID) REFERENCES dbo.EnumDeployStatus
	(ID) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO


CREATE TABLE [dbo].[SrirachaEmailMessage](
	[ID] [nvarchar](50) NOT NULL,
	[Subject] [nvarchar](200) NOT NULL,
	[DataObjectJson] [ntext] NULL,
	[RazorView] [ntext] NULL,
	[EnumQueueStatusID] [int] NOT NULL,
	[StartedDateTimeUtc] [datetime2](7) NULL,
	[QueueDateTimeUtc] [datetime2](7) NULL,
	[EmailAddressListJson] [ntext] NULL,
	[RecipientResultListJson] [ntext] NULL,
	[CreatedDateTimeUtc] [datetime2](7) NOT NULL,
	[CreatedByUserName] [nvarchar](50) NOT NULL,
	[UpdatedDateTimeUtc] [datetime2](7) NOT NULL,
	[UpdatedByUserName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_SrirachaEmailMessage] PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO

ALTER TABLE [dbo].[SrirachaEmailMessage] ADD  CONSTRAINT [DF_SrirachaEmailMessage_CreatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [CreatedDateTimeUtc]
GO

ALTER TABLE [dbo].[SrirachaEmailMessage] ADD  CONSTRAINT [DF_SrirachaEmailMessage_UpdatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [UpdatedDateTimeUtc]
GO


ALTER TABLE dbo.SrirachaEmailMessage ADD CONSTRAINT
	FK_SrirachaEmailMessage_EnumQueueStatus FOREIGN KEY
	(EnumQueueStatusID	) REFERENCES dbo.EnumQueueStatus
	(ID) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

CREATE TABLE [dbo].[DeployFile](
	[ID] [nvarchar](50) NOT NULL,
	[FileName] [nvarchar](200) NOT NULL,
	[FileStorageID] [nvarchar](50) NOT NULL,
	[FileManifestJson] [ntext] NULL,
	[CreatedByUserName] [nvarchar](50) NOT NULL,
	[CreatedDateTimeUtc] [datetime2](7) NOT NULL,
	[UpdatedByUserName] [nvarchar](50) NOT NULL,
	[UpdatedDateTimeUtc] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_DeployFile] PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO

ALTER TABLE [dbo].[DeployFile] ADD  CONSTRAINT [DF_DeployFile_CreatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [CreatedDateTimeUtc]
GO

ALTER TABLE [dbo].[DeployFile] ADD  CONSTRAINT [DF_DeployFile_UpdatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [UpdatedDateTimeUtc]
GO


CREATE TABLE [dbo].[DeployFileStorage](
	[ID] [nvarchar](50) NOT NULL,
	[FileData] [varbinary](max) NOT NULL,
 CONSTRAINT [PK_DeployFileStorage] PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)
GO
CREATE TABLE [dbo].[EnumSystemLogType](
	[ID] [int] NOT NULL,
	[TypeName] [nvarchar](50) NOT NULL,
	[DisplayValue] [nvarchar](50) NOT NULL,
	[CreatedByUserName] [nvarchar](50) NOT NULL,
	[CreatedDateTimeUtc] [datetime2](7) NOT NULL,
	[UpdatedByUserName] [nvarchar](50) NOT NULL,
	[UpdatedDateTimeUtc] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_EnumSystemLogType] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO

ALTER TABLE [dbo].[EnumSystemLogType] ADD  CONSTRAINT [DF_EnumSystemLogType_CreatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [CreatedDateTimeUtc]
GO

ALTER TABLE [dbo].[EnumSystemLogType] ADD  CONSTRAINT [DF_EnumSystemLogType_UpdatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [UpdatedDateTimeUtc]
GO

ALTER TABLE dbo.EnumSystemLogType ADD CONSTRAINT IX_EnumSystemLogType UNIQUE NONCLUSTERED (TypeName) 
    WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

INSERT INTO EnumSystemLogType (ID, TypeName, DisplayValue, CreatedDateTimeUtc, CreatedByUserName, UpdatedDateTimeUtc, UpdatedByUserName) 
    VALUES (0, 'Trace', 'Trace', GETUTCDATE(), 'RoundhousE', GETUTCDATE(), 'RoundhousE')
INSERT INTO EnumSystemLogType (ID, TypeName, DisplayValue, CreatedDateTimeUtc, CreatedByUserName, UpdatedDateTimeUtc, UpdatedByUserName) 
    VALUES (1, 'Debug', 'Debug', GETUTCDATE(), 'RoundhousE', GETUTCDATE(), 'RoundhousE')
INSERT INTO EnumSystemLogType (ID, TypeName, DisplayValue, CreatedDateTimeUtc, CreatedByUserName, UpdatedDateTimeUtc, UpdatedByUserName) 
    VALUES (2, 'Info', 'Info', GETUTCDATE(), 'RoundhousE', GETUTCDATE(), 'RoundhousE')
INSERT INTO EnumSystemLogType (ID, TypeName, DisplayValue, CreatedDateTimeUtc, CreatedByUserName, UpdatedDateTimeUtc, UpdatedByUserName) 
    VALUES (3, 'Warn', 'Warn', GETUTCDATE(), 'RoundhousE', GETUTCDATE(), 'RoundhousE')
INSERT INTO EnumSystemLogType (ID, TypeName, DisplayValue, CreatedDateTimeUtc, CreatedByUserName, UpdatedDateTimeUtc, UpdatedByUserName) 
    VALUES (4, 'Error', 'Error', GETUTCDATE(), 'RoundhousE', GETUTCDATE(), 'RoundhousE')
INSERT INTO EnumSystemLogType (ID, TypeName, DisplayValue, CreatedDateTimeUtc, CreatedByUserName, UpdatedDateTimeUtc, UpdatedByUserName) 
    VALUES (5, 'Fatal', 'Fatal', GETUTCDATE(), 'RoundhousE', GETUTCDATE(), 'RoundhousE')
INSERT INTO EnumSystemLogType (ID, TypeName, DisplayValue, CreatedDateTimeUtc, CreatedByUserName, UpdatedDateTimeUtc, UpdatedByUserName) 
    VALUES (6, 'Off', 'Off', GETUTCDATE(), 'RoundhousE', GETUTCDATE(), 'RoundhousE')

GO

CREATE TABLE [dbo].[SystemLog](
	[ID] [nvarchar](50) NOT NULL,
	[MessageText] [nvarchar](max) NOT NULL,
	[EnumSystemLogTypeID] [int] NOT NULL,
	[UserName] [nvarchar](50) NOT NULL,
	[MessageDateTimeUtc] [datetime2](7) NOT NULL,
	[LoggerName] [nvarchar](50) NULL,
 CONSTRAINT [PK_SystemLog] PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)
GO

CREATE NONCLUSTERED INDEX [IX_SystemLog_EnumSystemLogTypeID] ON [dbo].[SystemLog]
(
	[EnumSystemLogTypeID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE NONCLUSTERED INDEX [IX_SystemLog_MessageDateTimeUtc] ON [dbo].[SystemLog]
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

ALTER TABLE [dbo].[SystemLog] ADD  CONSTRAINT [DF_SystemLog_ID]  DEFAULT (newid()) FOR [ID]
GO

ALTER TABLE [dbo].[SystemLog]  WITH CHECK ADD  CONSTRAINT [FK_SystemLog_EnumSystemLogType] FOREIGN KEY([EnumSystemLogTypeID])
REFERENCES [dbo].[EnumSystemLogType] ([ID])
GO

ALTER TABLE [dbo].[SystemLog] CHECK CONSTRAINT [FK_SystemLog_EnumSystemLogType]
GO

CREATE TABLE [dbo].[SrirachaUser](
	[ID] [nvarchar](50) NOT NULL,
	[UserName] [nvarchar](50) NOT NULL,
	[UserGuid] [uniqueidentifier] NOT NULL,
	[EmailAddress] [nvarchar](500) NOT NULL,
	[EncryptedPassword] [nvarchar](1000) NULL,
	[LastPasswordChangedDateTimeUtc] [datetime2](7) NULL,
	[PasswordQuestion] [nvarchar](500) NULL,
	[PasswordAnswer] [nvarchar](500) NULL,
	[LastLockoutDateTimeUtc] [datetime2](7) NULL,
	[LastLoginDateDateTimeUtc] [datetime2](7) NULL,
	[LockedIndicator] [bit] NOT NULL,
	[MustChangePasswordIndicator] [bit] NOT NULL,
	[LastActivityDateTimeUtc] [datetime2](7) NOT NULL,
	[ProjectNotificationItemListJson] [nvarchar](max) NULL,
	[CreatedByUserName] [nvarchar](50) NOT NULL,
	[CreatedDateTimeUtc] [datetime2](7) NOT NULL,
	[UpdatedByUserName] [nvarchar](50) NOT NULL,
	[UpdatedDateTimeUtc] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_SrirachaUser] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO

ALTER TABLE [dbo].[SrirachaUser] ADD  CONSTRAINT [DF_SrirachaUser_LockedIndicator]  DEFAULT ((0)) FOR [LockedIndicator]
GO

ALTER TABLE [dbo].[SrirachaUser] ADD  CONSTRAINT [DF_SrirachaUser_MustChangePasswordIndicator]  DEFAULT ((0)) FOR [MustChangePasswordIndicator]
GO

ALTER TABLE [dbo].[SrirachaUser] ADD  CONSTRAINT [DF_SrirachaUser_LastActivityDateTimeUtc]  DEFAULT (getutcdate()) FOR [LastActivityDateTimeUtc]
GO

ALTER TABLE [dbo].[SrirachaUser] ADD  CONSTRAINT [DF_SrirachaUser_CreatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [CreatedDateTimeUtc]
GO


CREATE TABLE [dbo].[SystemSettings](
	[ID] [nvarchar](50) NOT NULL,
	[ActiveIndicator] [bit] NOT NULL,
	[SettingsJson] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_SystemSettings] PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO

ALTER TABLE [dbo].[SystemSettings] ADD  CONSTRAINT [DF_SystemSettings_ActiveIndicator]  DEFAULT ((0)) FOR [ActiveIndicator]
GO

CREATE TABLE [dbo].[RazorTemplate](
	[ID] [nvarchar](50) NOT NULL,
	[ViewName] [nvarchar](100) NOT NULL,
	[ViewData] [nvarchar](max) NOT NULL,
	[CreatedByUserName] [nvarchar](50) NOT NULL,
	[CreatedDateTimeUtc] [datetime2](7) NOT NULL,
	[UpdatedByUserName] [nvarchar](50) NOT NULL,
	[UpdatedDateTimeUtc] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_RazorTemplate] PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO

ALTER TABLE [dbo].[RazorTemplate] ADD  CONSTRAINT [DF_RazorTemplate_CreatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [CreatedDateTimeUtc]
GO

ALTER TABLE [dbo].[RazorTemplate] ADD  CONSTRAINT [DF_RazorTemplate_UpdatedDateTimeUtc]  DEFAULT (getutcdate()) FOR [UpdatedDateTimeUtc]
GO

CREATE UNIQUE NONCLUSTERED INDEX IX_RazorTemplate_ViewName ON dbo.RazorTemplate
	(
	ViewName
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
