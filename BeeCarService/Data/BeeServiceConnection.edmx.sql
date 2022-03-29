
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 03/08/2022 08:24:31
-- Generated from EDMX file: C:\BeeCar\BeeCarService\BeeCarService\Data\BeeServiceConnection.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [BeeService];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_AddOn_ServiceType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AddOn] DROP CONSTRAINT [FK_AddOn_ServiceType];
GO
IF OBJECT_ID(N'[dbo].[FK_BeeUser_Landmark]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[BeeUser] DROP CONSTRAINT [FK_BeeUser_Landmark];
GO
IF OBJECT_ID(N'[dbo].[FK_ServiceAddOn_AddOn]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ServiceAddOn] DROP CONSTRAINT [FK_ServiceAddOn_AddOn];
GO
IF OBJECT_ID(N'[dbo].[FK_ServiceAddOn_ServiceRequestVehicle]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ServiceAddOn] DROP CONSTRAINT [FK_ServiceAddOn_ServiceRequestVehicle];
GO
IF OBJECT_ID(N'[dbo].[FK_ServiceRequest_BeeUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ServiceRequest] DROP CONSTRAINT [FK_ServiceRequest_BeeUser];
GO
IF OBJECT_ID(N'[dbo].[FK_ServiceRequest_ServiceTeam]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ServiceRequest] DROP CONSTRAINT [FK_ServiceRequest_ServiceTeam];
GO
IF OBJECT_ID(N'[dbo].[FK_ServiceRequestVehicle_ServiceRequest]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ServiceRequestVehicle] DROP CONSTRAINT [FK_ServiceRequestVehicle_ServiceRequest];
GO
IF OBJECT_ID(N'[dbo].[FK_ServiceRequestVehicle_ServiceType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ServiceRequestVehicle] DROP CONSTRAINT [FK_ServiceRequestVehicle_ServiceType];
GO
IF OBJECT_ID(N'[dbo].[FK_ServiceRequestVehicle_VehicleType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ServiceRequestVehicle] DROP CONSTRAINT [FK_ServiceRequestVehicle_VehicleType];
GO
IF OBJECT_ID(N'[dbo].[FK_ServiceType_VehicleClass]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ServiceType] DROP CONSTRAINT [FK_ServiceType_VehicleClass];
GO
IF OBJECT_ID(N'[dbo].[FK_VehicleClass_VehicleType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[VehicleClass] DROP CONSTRAINT [FK_VehicleClass_VehicleType];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[AddOn]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AddOn];
GO
IF OBJECT_ID(N'[dbo].[BeeUser]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BeeUser];
GO
IF OBJECT_ID(N'[dbo].[Landmark]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Landmark];
GO
IF OBJECT_ID(N'[dbo].[ServiceAddOn]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ServiceAddOn];
GO
IF OBJECT_ID(N'[dbo].[ServiceRequest]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ServiceRequest];
GO
IF OBJECT_ID(N'[dbo].[ServiceRequestVehicle]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ServiceRequestVehicle];
GO
IF OBJECT_ID(N'[dbo].[ServiceTeam]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ServiceTeam];
GO
IF OBJECT_ID(N'[dbo].[ServiceType]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ServiceType];
GO
IF OBJECT_ID(N'[dbo].[sysdiagrams]', 'U') IS NOT NULL
    DROP TABLE [dbo].[sysdiagrams];
GO
IF OBJECT_ID(N'[dbo].[VehicleClass]', 'U') IS NOT NULL
    DROP TABLE [dbo].[VehicleClass];
GO
IF OBJECT_ID(N'[dbo].[VehicleType]', 'U') IS NOT NULL
    DROP TABLE [dbo].[VehicleType];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'AddOns'
CREATE TABLE [dbo].[AddOns] (
    [ID] int  NOT NULL,
    [AddOn1] nvarchar(30)  NOT NULL,
    [ServiceTypeID] int  NOT NULL,
    [Cost] decimal(6,3)  NULL,
    [Duration] int  NULL,
    [Status] smallint  NOT NULL
);
GO

-- Creating table 'BeeUsers'
CREATE TABLE [dbo].[BeeUsers] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Username] nvarchar(50)  NOT NULL,
    [Password] nvarchar(max)  NOT NULL,
    [RegDate] datetime  NOT NULL,
    [Email] nvarchar(50)  NOT NULL,
    [FullName] nvarchar(100)  NOT NULL,
    [Address] nvarchar(200)  NULL,
    [Phone] nvarchar(20)  NULL,
    [LandmarkID] int  NULL,
    [Message] nchar(200)  NULL,
    [ContactPreference] smallint  NOT NULL,
    [TextNotifications] bit  NOT NULL,
    [PaymentMode] smallint  NOT NULL
);
GO

-- Creating table 'Landmarks'
CREATE TABLE [dbo].[Landmarks] (
    [ID] int  NOT NULL,
    [LandmarkLocation] nvarchar(200)  NOT NULL,
    [Status] smallint  NOT NULL
);
GO

-- Creating table 'ServiceAddOns'
CREATE TABLE [dbo].[ServiceAddOns] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [ServiceRequestVehicleID] int  NOT NULL,
    [AddOnID] int  NULL
);
GO

-- Creating table 'ServiceRequests'
CREATE TABLE [dbo].[ServiceRequests] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [CustomerID] int  NOT NULL,
    [ServiceStartTime] datetime  NOT NULL,
    [ServiceEndTime] datetime  NULL,
    [ServiceDuration] int  NULL,
    [Status] smallint  NOT NULL,
    [ServiceTeamID] int  NULL,
    [ServiceCost] decimal(7,3)  NULL
);
GO

-- Creating table 'ServiceRequestVehicles'
CREATE TABLE [dbo].[ServiceRequestVehicles] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [ServiceRequestID] int  NOT NULL,
    [VehicleTypeID] int  NOT NULL,
    [VehicleClassID] int  NOT NULL,
    [ServiceTypeID] int  NOT NULL
);
GO

-- Creating table 'ServiceTypes'
CREATE TABLE [dbo].[ServiceTypes] (
    [ID] int  NOT NULL,
    [ServiceType1] nvarchar(30)  NOT NULL,
    [VehicleClassID] int  NOT NULL,
    [Cost] decimal(6,3)  NULL,
    [Duration] int  NULL,
    [Status] smallint  NOT NULL
);
GO

-- Creating table 'sysdiagrams'
CREATE TABLE [dbo].[sysdiagrams] (
    [name] nvarchar(128)  NOT NULL,
    [principal_id] int  NOT NULL,
    [diagram_id] int IDENTITY(1,1) NOT NULL,
    [version] int  NULL,
    [definition] varbinary(max)  NULL
);
GO

-- Creating table 'VehicleClasses'
CREATE TABLE [dbo].[VehicleClasses] (
    [ID] int  NOT NULL,
    [Class] nvarchar(20)  NOT NULL,
    [VehichleTypeID] int  NOT NULL,
    [Status] smallint  NOT NULL
);
GO

-- Creating table 'VehicleTypes'
CREATE TABLE [dbo].[VehicleTypes] (
    [ID] int  NOT NULL,
    [TYPE] nvarchar(10)  NOT NULL,
    [Status] smallint  NOT NULL
);
GO

-- Creating table 'ServiceTeams'
CREATE TABLE [dbo].[ServiceTeams] (
    [ID] int  NOT NULL,
    [TeamName] nvarchar(100)  NOT NULL,
    [Status] smallint  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [ID] in table 'AddOns'
ALTER TABLE [dbo].[AddOns]
ADD CONSTRAINT [PK_AddOns]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [Id] in table 'BeeUsers'
ALTER TABLE [dbo].[BeeUsers]
ADD CONSTRAINT [PK_BeeUsers]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [ID] in table 'Landmarks'
ALTER TABLE [dbo].[Landmarks]
ADD CONSTRAINT [PK_Landmarks]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'ServiceAddOns'
ALTER TABLE [dbo].[ServiceAddOns]
ADD CONSTRAINT [PK_ServiceAddOns]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'ServiceRequests'
ALTER TABLE [dbo].[ServiceRequests]
ADD CONSTRAINT [PK_ServiceRequests]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'ServiceRequestVehicles'
ALTER TABLE [dbo].[ServiceRequestVehicles]
ADD CONSTRAINT [PK_ServiceRequestVehicles]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'ServiceTypes'
ALTER TABLE [dbo].[ServiceTypes]
ADD CONSTRAINT [PK_ServiceTypes]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [diagram_id] in table 'sysdiagrams'
ALTER TABLE [dbo].[sysdiagrams]
ADD CONSTRAINT [PK_sysdiagrams]
    PRIMARY KEY CLUSTERED ([diagram_id] ASC);
GO

-- Creating primary key on [ID] in table 'VehicleClasses'
ALTER TABLE [dbo].[VehicleClasses]
ADD CONSTRAINT [PK_VehicleClasses]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'VehicleTypes'
ALTER TABLE [dbo].[VehicleTypes]
ADD CONSTRAINT [PK_VehicleTypes]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'ServiceTeams'
ALTER TABLE [dbo].[ServiceTeams]
ADD CONSTRAINT [PK_ServiceTeams]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [ServiceTypeID] in table 'AddOns'
ALTER TABLE [dbo].[AddOns]
ADD CONSTRAINT [FK_AddOn_ServiceType]
    FOREIGN KEY ([ServiceTypeID])
    REFERENCES [dbo].[ServiceTypes]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AddOn_ServiceType'
CREATE INDEX [IX_FK_AddOn_ServiceType]
ON [dbo].[AddOns]
    ([ServiceTypeID]);
GO

-- Creating foreign key on [AddOnID] in table 'ServiceAddOns'
ALTER TABLE [dbo].[ServiceAddOns]
ADD CONSTRAINT [FK_ServiceAddOn_AddOn]
    FOREIGN KEY ([AddOnID])
    REFERENCES [dbo].[AddOns]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ServiceAddOn_AddOn'
CREATE INDEX [IX_FK_ServiceAddOn_AddOn]
ON [dbo].[ServiceAddOns]
    ([AddOnID]);
GO

-- Creating foreign key on [LandmarkID] in table 'BeeUsers'
ALTER TABLE [dbo].[BeeUsers]
ADD CONSTRAINT [FK_BeeUser_Landmark]
    FOREIGN KEY ([LandmarkID])
    REFERENCES [dbo].[Landmarks]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_BeeUser_Landmark'
CREATE INDEX [IX_FK_BeeUser_Landmark]
ON [dbo].[BeeUsers]
    ([LandmarkID]);
GO

-- Creating foreign key on [CustomerID] in table 'ServiceRequests'
ALTER TABLE [dbo].[ServiceRequests]
ADD CONSTRAINT [FK_ServiceRequest_BeeUser]
    FOREIGN KEY ([CustomerID])
    REFERENCES [dbo].[BeeUsers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ServiceRequest_BeeUser'
CREATE INDEX [IX_FK_ServiceRequest_BeeUser]
ON [dbo].[ServiceRequests]
    ([CustomerID]);
GO

-- Creating foreign key on [ServiceRequestVehicleID] in table 'ServiceAddOns'
ALTER TABLE [dbo].[ServiceAddOns]
ADD CONSTRAINT [FK_ServiceAddOn_ServiceRequestVehicle]
    FOREIGN KEY ([ServiceRequestVehicleID])
    REFERENCES [dbo].[ServiceRequestVehicles]
        ([ID])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ServiceAddOn_ServiceRequestVehicle'
CREATE INDEX [IX_FK_ServiceAddOn_ServiceRequestVehicle]
ON [dbo].[ServiceAddOns]
    ([ServiceRequestVehicleID]);
GO

-- Creating foreign key on [ServiceRequestID] in table 'ServiceRequestVehicles'
ALTER TABLE [dbo].[ServiceRequestVehicles]
ADD CONSTRAINT [FK_ServiceRequestVehicle_ServiceRequest]
    FOREIGN KEY ([ServiceRequestID])
    REFERENCES [dbo].[ServiceRequests]
        ([ID])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ServiceRequestVehicle_ServiceRequest'
CREATE INDEX [IX_FK_ServiceRequestVehicle_ServiceRequest]
ON [dbo].[ServiceRequestVehicles]
    ([ServiceRequestID]);
GO

-- Creating foreign key on [ServiceTypeID] in table 'ServiceRequestVehicles'
ALTER TABLE [dbo].[ServiceRequestVehicles]
ADD CONSTRAINT [FK_ServiceRequestVehicle_ServiceType]
    FOREIGN KEY ([ServiceTypeID])
    REFERENCES [dbo].[ServiceTypes]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ServiceRequestVehicle_ServiceType'
CREATE INDEX [IX_FK_ServiceRequestVehicle_ServiceType]
ON [dbo].[ServiceRequestVehicles]
    ([ServiceTypeID]);
GO

-- Creating foreign key on [VehicleTypeID] in table 'ServiceRequestVehicles'
ALTER TABLE [dbo].[ServiceRequestVehicles]
ADD CONSTRAINT [FK_ServiceRequestVehicle_VehicleType]
    FOREIGN KEY ([VehicleTypeID])
    REFERENCES [dbo].[VehicleTypes]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ServiceRequestVehicle_VehicleType'
CREATE INDEX [IX_FK_ServiceRequestVehicle_VehicleType]
ON [dbo].[ServiceRequestVehicles]
    ([VehicleTypeID]);
GO

-- Creating foreign key on [VehicleClassID] in table 'ServiceTypes'
ALTER TABLE [dbo].[ServiceTypes]
ADD CONSTRAINT [FK_ServiceType_VehicleClass]
    FOREIGN KEY ([VehicleClassID])
    REFERENCES [dbo].[VehicleClasses]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ServiceType_VehicleClass'
CREATE INDEX [IX_FK_ServiceType_VehicleClass]
ON [dbo].[ServiceTypes]
    ([VehicleClassID]);
GO

-- Creating foreign key on [VehichleTypeID] in table 'VehicleClasses'
ALTER TABLE [dbo].[VehicleClasses]
ADD CONSTRAINT [FK_VehicleClass_VehicleType]
    FOREIGN KEY ([VehichleTypeID])
    REFERENCES [dbo].[VehicleTypes]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_VehicleClass_VehicleType'
CREATE INDEX [IX_FK_VehicleClass_VehicleType]
ON [dbo].[VehicleClasses]
    ([VehichleTypeID]);
GO

-- Creating foreign key on [ServiceTeamID] in table 'ServiceRequests'
ALTER TABLE [dbo].[ServiceRequests]
ADD CONSTRAINT [FK_ServiceRequest_ServiceTeam]
    FOREIGN KEY ([ServiceTeamID])
    REFERENCES [dbo].[ServiceTeams]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ServiceRequest_ServiceTeam'
CREATE INDEX [IX_FK_ServiceRequest_ServiceTeam]
ON [dbo].[ServiceRequests]
    ([ServiceTeamID]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------