USE [master]
GO

USE [BEEDB]
GO
/****** Object:  Table [dbo].[AddOn]    Script Date: 05-10-2015 21:26:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AddOn](
	[ID] [int] NOT NULL,
	[AddOn] [nvarchar](30) NOT NULL,
	[ServiceTypeID] [int] NOT NULL,
	[Cost] [numeric](6, 2) NULL,
 CONSTRAINT [PK_VehicleAddOn] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[BeeUser]    Script Date: 05-10-2015 21:26:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BeeUser](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](50) NOT NULL,
	[Password] [nvarchar](max) NOT NULL,
	[RegDate] [datetime] NOT NULL CONSTRAINT [DF__BeeUsers__RegDat__72C60C4A]  DEFAULT (getdate()),
	[Email] [nvarchar](50) NOT NULL,
	[FullName] [nvarchar](100) NOT NULL CONSTRAINT [DF_BeeUser_FullName]  DEFAULT (''),
	[Address] [nvarchar](200) NULL,
	[Phone] [nvarchar](20) NULL,
	[LandmarkID] [int] NULL,
	[Message] [nchar](200) NULL,
	[ContactPreference] [smallint] NOT NULL CONSTRAINT [DF_BeeUser_ContactByPhone]  DEFAULT ((1)),
	[TextNotifications] [bit] NOT NULL CONSTRAINT [DF_BeeUser_TextNotifications]  DEFAULT ((0)),
	[PaymentMode] [smallint] NOT NULL CONSTRAINT [DF_BeeUser_PaymentMode]  DEFAULT ((1)),
 CONSTRAINT [PK__BeeUsers__3214EC07B93EE9DD] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Landmark]    Script Date: 05-10-2015 21:26:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Landmark](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[LandmarkLocation] [nvarchar](200) NOT NULL,
 CONSTRAINT [PK_Landmark] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ServiceAddOn]    Script Date: 05-10-2015 21:26:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ServiceAddOn](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ServiceRequestVehicleID] [int] NOT NULL,
	[AddOnID] [int] NULL,
 CONSTRAINT [PK_ServiceAddOn] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ServiceRequest]    Script Date: 05-10-2015 21:26:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ServiceRequest](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CustomerID] [int] NOT NULL,
	[ServiceStartTime] [datetime] NOT NULL,
	[ServiceEndTime] [datetime] NULL,
	[ServiceDuration] [int] NULL,
	[Status] [smallint] NOT NULL CONSTRAINT [DF_ServiceRequest_Status]  DEFAULT ((0)),
	[ServiceTeamID] [int] NULL,
 CONSTRAINT [PK_ServiceRequest] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ServiceRequestVehicle]    Script Date: 05-10-2015 21:26:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ServiceRequestVehicle](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ServiceRequestID] [int] NOT NULL,
	[VehicleTypeID] [int] NOT NULL,
	[VehicleClassID] [int] NOT NULL,
	[ServiceTypeID] [int] NOT NULL,
 CONSTRAINT [PK_ServiceRequestVehicle] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ServiceTeam]    Script Date: 05-10-2015 21:26:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ServiceTeam](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[TeamName] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_ServiceTeam] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ServiceType]    Script Date: 05-10-2015 21:26:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ServiceType](
	[ID] [int] NOT NULL,
	[ServiceType] [nvarchar](30) NOT NULL,
	[VehicleClassID] [int] NOT NULL,
	[Cost] [numeric](6, 2) NULL,
	[Duration] [int] NULL,
 CONSTRAINT [PK_ServiceType] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[VehicleClass]    Script Date: 05-10-2015 21:26:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VehicleClass](
	[ID] [int] NOT NULL,
	[Class] [nvarchar](20) NOT NULL,
	[VehichleTypeID] [int] NOT NULL,
 CONSTRAINT [PK_VehicleClass] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[VehicleType]    Script Date: 05-10-2015 21:26:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VehicleType](
	[ID] [int] NOT NULL,
	[TYPE] [nvarchar](10) NOT NULL,
 CONSTRAINT [PK_VehicleType] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_System_Users_Username]    Script Date: 05-10-2015 21:26:55 ******/
CREATE NONCLUSTERED INDEX [IX_System_Users_Username] ON [dbo].[BeeUser]
(
	[Username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AddOn]  WITH CHECK ADD  CONSTRAINT [FK_AddOn_ServiceType] FOREIGN KEY([ServiceTypeID])
REFERENCES [dbo].[ServiceType] ([ID])
GO
ALTER TABLE [dbo].[AddOn] CHECK CONSTRAINT [FK_AddOn_ServiceType]
GO
ALTER TABLE [dbo].[BeeUser]  WITH CHECK ADD  CONSTRAINT [FK_BeeUser_Landmark] FOREIGN KEY([LandmarkID])
REFERENCES [dbo].[Landmark] ([ID])
GO
ALTER TABLE [dbo].[BeeUser] CHECK CONSTRAINT [FK_BeeUser_Landmark]
GO
ALTER TABLE [dbo].[ServiceAddOn]  WITH CHECK ADD  CONSTRAINT [FK_ServiceAddOn_AddOn] FOREIGN KEY([AddOnID])
REFERENCES [dbo].[AddOn] ([ID])
GO
ALTER TABLE [dbo].[ServiceAddOn] CHECK CONSTRAINT [FK_ServiceAddOn_AddOn]
GO
ALTER TABLE [dbo].[ServiceAddOn]  WITH CHECK ADD  CONSTRAINT [FK_ServiceAddOn_ServiceRequestVehicle] FOREIGN KEY([ServiceRequestVehicleID])
REFERENCES [dbo].[ServiceRequestVehicle] ([ID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ServiceAddOn] CHECK CONSTRAINT [FK_ServiceAddOn_ServiceRequestVehicle]
GO
ALTER TABLE [dbo].[ServiceRequest]  WITH CHECK ADD  CONSTRAINT [FK_ServiceRequest_BeeUser] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[BeeUser] ([Id])
GO
ALTER TABLE [dbo].[ServiceRequest] CHECK CONSTRAINT [FK_ServiceRequest_BeeUser]
GO
ALTER TABLE [dbo].[ServiceRequest]  WITH CHECK ADD  CONSTRAINT [FK_ServiceRequest_ServiceTeam] FOREIGN KEY([ServiceTeamID])
REFERENCES [dbo].[ServiceTeam] ([ID])
GO
ALTER TABLE [dbo].[ServiceRequest] CHECK CONSTRAINT [FK_ServiceRequest_ServiceTeam]
GO
ALTER TABLE [dbo].[ServiceRequestVehicle]  WITH CHECK ADD  CONSTRAINT [FK_ServiceRequestVehicle_ServiceRequest] FOREIGN KEY([ServiceRequestID])
REFERENCES [dbo].[ServiceRequest] ([ID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ServiceRequestVehicle] CHECK CONSTRAINT [FK_ServiceRequestVehicle_ServiceRequest]
GO
ALTER TABLE [dbo].[ServiceRequestVehicle]  WITH CHECK ADD  CONSTRAINT [FK_ServiceRequestVehicle_ServiceType] FOREIGN KEY([ServiceTypeID])
REFERENCES [dbo].[ServiceType] ([ID])
GO
ALTER TABLE [dbo].[ServiceRequestVehicle] CHECK CONSTRAINT [FK_ServiceRequestVehicle_ServiceType]
GO
ALTER TABLE [dbo].[ServiceRequestVehicle]  WITH CHECK ADD  CONSTRAINT [FK_ServiceRequestVehicle_VehicleType] FOREIGN KEY([VehicleTypeID])
REFERENCES [dbo].[VehicleType] ([ID])
GO
ALTER TABLE [dbo].[ServiceRequestVehicle] CHECK CONSTRAINT [FK_ServiceRequestVehicle_VehicleType]
GO
ALTER TABLE [dbo].[ServiceType]  WITH CHECK ADD  CONSTRAINT [FK_ServiceType_VehicleClass] FOREIGN KEY([VehicleClassID])
REFERENCES [dbo].[VehicleClass] ([ID])
GO
ALTER TABLE [dbo].[ServiceType] CHECK CONSTRAINT [FK_ServiceType_VehicleClass]
GO
ALTER TABLE [dbo].[VehicleClass]  WITH CHECK ADD  CONSTRAINT [FK_VehicleClass_VehicleType] FOREIGN KEY([VehichleTypeID])
REFERENCES [dbo].[VehicleType] ([ID])
GO
ALTER TABLE [dbo].[VehicleClass] CHECK CONSTRAINT [FK_VehicleClass_VehicleType]
GO
USE [master]
GO
ALTER DATABASE [BEEDB] SET  READ_WRITE 
GO
