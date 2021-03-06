/****** Object:  ForeignKey [FK_Event_Category]    Script Date: 10/13/2011 10:47:23 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Event_Category]') AND parent_object_id = OBJECT_ID(N'[dbo].[Event]'))
ALTER TABLE [dbo].[Event] DROP CONSTRAINT [FK_Event_Category]

/****** Object:  ForeignKey [FK_RuleSet_Rule]    Script Date: 10/13/2011 10:47:23 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_RuleSet_Rule]') AND parent_object_id = OBJECT_ID(N'[dbo].[RuleSet]'))
ALTER TABLE [dbo].[RuleSet] DROP CONSTRAINT [FK_RuleSet_Rule]

/****** Object:  ForeignKey [FK_UserEvent_Event]    Script Date: 10/13/2011 10:47:23 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_UserEvent_Event]') AND parent_object_id = OBJECT_ID(N'[dbo].[UserEvent]'))
ALTER TABLE [dbo].[UserEvent] DROP CONSTRAINT [FK_UserEvent_Event]

/****** Object:  ForeignKey [FK_UserEvent_User]    Script Date: 10/13/2011 10:47:23 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_UserEvent_User]') AND parent_object_id = OBJECT_ID(N'[dbo].[UserEvent]'))
ALTER TABLE [dbo].[UserEvent] DROP CONSTRAINT [FK_UserEvent_User]

/****** Object:  ForeignKey [FK_Winner_Category]    Script Date: 10/13/2011 10:47:23 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Winner_Category]') AND parent_object_id = OBJECT_ID(N'[dbo].[Winner]'))
ALTER TABLE [dbo].[Winner] DROP CONSTRAINT [FK_Winner_Category]

/****** Object:  ForeignKey [FK_Winner_Event]    Script Date: 10/13/2011 10:47:23 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Winner_Event]') AND parent_object_id = OBJECT_ID(N'[dbo].[Winner]'))
ALTER TABLE [dbo].[Winner] DROP CONSTRAINT [FK_Winner_Event]

/****** Object:  ForeignKey [FK_Winner_User]    Script Date: 10/13/2011 10:47:23 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Winner_User]') AND parent_object_id = OBJECT_ID(N'[dbo].[Winner]'))
ALTER TABLE [dbo].[Winner] DROP CONSTRAINT [FK_Winner_User]

/****** Object:  Table [dbo].[UserEvent]    Script Date: 10/13/2011 10:47:23 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserEvent]') AND type in (N'U'))
DROP TABLE [dbo].[UserEvent]

/****** Object:  Table [dbo].[Winner]    Script Date: 10/13/2011 10:47:23 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Winner]') AND type in (N'U'))
DROP TABLE [dbo].[Winner]

/****** Object:  Table [dbo].[Event]    Script Date: 10/13/2011 10:47:23 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Event]') AND type in (N'U'))
DROP TABLE [dbo].[Event]

/****** Object:  Table [dbo].[RuleSet]    Script Date: 10/13/2011 10:47:23 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RuleSet]') AND type in (N'U'))
DROP TABLE [dbo].[RuleSet]

/****** Object:  Table [dbo].[User]    Script Date: 10/13/2011 10:47:23 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[User]') AND type in (N'U'))
DROP TABLE [dbo].[User]

/****** Object:  Table [dbo].[Rule]    Script Date: 10/13/2011 10:47:23 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Rule]') AND type in (N'U'))
DROP TABLE [dbo].[Rule]

/****** Object:  Table [dbo].[Category]    Script Date: 10/13/2011 10:47:23 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Category]') AND type in (N'U'))
DROP TABLE [dbo].[Category]

/****** Object:  Default [DF_Category_IsActive]    Script Date: 10/13/2011 10:47:23 ******/
IF  EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_Category_IsActive]') AND parent_object_id = OBJECT_ID(N'[dbo].[Category]'))
Begin
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Category_IsActive]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Category] DROP CONSTRAINT [DF_Category_IsActive]
END


End

/****** Object:  Table [dbo].[Category]    Script Date: 10/13/2011 10:47:23 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Category]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Category](
	[CategoryId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED 
(
	[CategoryId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END

/****** Object:  Table [dbo].[Rule]    Script Date: 10/13/2011 10:47:23 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Rule]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Rule](
	[RuleId] [int] IDENTITY(1,1) NOT NULL,
	[MethodName] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[ClassName] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[DisplayName] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_Rule] PRIMARY KEY CLUSTERED 
(
	[RuleId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END

/****** Object:  Table [dbo].[User]    Script Date: 10/13/2011 10:47:23 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[User]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[User](
	[Email] [nvarchar](150) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Password] [nvarchar](150) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Name] [nvarchar](120) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END

/****** Object:  Table [dbo].[RuleSet]    Script Date: 10/13/2011 10:47:23 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RuleSet]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[RuleSet](
	[RuleSetId] [int] NOT NULL,
	[RuleId] [int] NOT NULL,
	[Priority] [int] NOT NULL,
	[CateogryId] [int] NULL,
	[EventId] [int] NULL,
	[RuleParameter] [int] NULL,
 CONSTRAINT [PK_RuleSet_1] PRIMARY KEY CLUSTERED 
(
	[RuleSetId] ASC,
	[RuleId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END

/****** Object:  Table [dbo].[Event]    Script Date: 10/13/2011 10:47:23 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Event]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Event](
	[EventId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Created] [datetime] NOT NULL,
	[Creator] [nvarchar](150) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[AvailableSpots] [int] NOT NULL,
	[InformationUrl] [nvarchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Description] [nvarchar](300) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[DeadLine] [datetime] NOT NULL,
	[CategoryId] [int] NOT NULL,
	[StartTime] [datetime] NOT NULL,
 CONSTRAINT [PK_Event] PRIMARY KEY CLUSTERED 
(
	[EventId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END

/****** Object:  Table [dbo].[Winner]    Script Date: 10/13/2011 10:47:23 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Winner]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Winner](
	[EventId] [int] NOT NULL,
	[UserEmail] [nvarchar](150) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[NumberOfSpotsWon] [int] NOT NULL,
	[Date] [datetime] NULL,
	[CatogoryId] [int] NOT NULL,
 CONSTRAINT [PK_Winner] PRIMARY KEY CLUSTERED 
(
	[EventId] ASC,
	[UserEmail] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END

/****** Object:  Table [dbo].[UserEvent]    Script Date: 10/13/2011 10:47:23 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserEvent]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[UserEvent](
	[UserEmail] [nvarchar](150) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[EventId] [int] NOT NULL,
	[NumberOfSpots] [int] NOT NULL,
 CONSTRAINT [PK_UserEvent] PRIMARY KEY CLUSTERED 
(
	[UserEmail] ASC,
	[EventId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END

/****** Object:  Default [DF_Category_IsActive]    Script Date: 10/13/2011 10:47:23 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_Category_IsActive]') AND parent_object_id = OBJECT_ID(N'[dbo].[Category]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Category_IsActive]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Category] ADD  CONSTRAINT [DF_Category_IsActive]  DEFAULT ((1)) FOR [IsActive]
END


End

/****** Object:  ForeignKey [FK_Event_Category]    Script Date: 10/13/2011 10:47:23 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Event_Category]') AND parent_object_id = OBJECT_ID(N'[dbo].[Event]'))
ALTER TABLE [dbo].[Event]  WITH CHECK ADD  CONSTRAINT [FK_Event_Category] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Category] ([CategoryId])

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Event_Category]') AND parent_object_id = OBJECT_ID(N'[dbo].[Event]'))
ALTER TABLE [dbo].[Event] CHECK CONSTRAINT [FK_Event_Category]

/****** Object:  ForeignKey [FK_RuleSet_Rule]    Script Date: 10/13/2011 10:47:23 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_RuleSet_Rule]') AND parent_object_id = OBJECT_ID(N'[dbo].[RuleSet]'))
ALTER TABLE [dbo].[RuleSet]  WITH CHECK ADD  CONSTRAINT [FK_RuleSet_Rule] FOREIGN KEY([RuleId])
REFERENCES [dbo].[Rule] ([RuleId])

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_RuleSet_Rule]') AND parent_object_id = OBJECT_ID(N'[dbo].[RuleSet]'))
ALTER TABLE [dbo].[RuleSet] CHECK CONSTRAINT [FK_RuleSet_Rule]

/****** Object:  ForeignKey [FK_UserEvent_Event]    Script Date: 10/13/2011 10:47:23 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_UserEvent_Event]') AND parent_object_id = OBJECT_ID(N'[dbo].[UserEvent]'))
ALTER TABLE [dbo].[UserEvent]  WITH CHECK ADD  CONSTRAINT [FK_UserEvent_Event] FOREIGN KEY([EventId])
REFERENCES [dbo].[Event] ([EventId])

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_UserEvent_Event]') AND parent_object_id = OBJECT_ID(N'[dbo].[UserEvent]'))
ALTER TABLE [dbo].[UserEvent] CHECK CONSTRAINT [FK_UserEvent_Event]

/****** Object:  ForeignKey [FK_UserEvent_User]    Script Date: 10/13/2011 10:47:23 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_UserEvent_User]') AND parent_object_id = OBJECT_ID(N'[dbo].[UserEvent]'))
ALTER TABLE [dbo].[UserEvent]  WITH CHECK ADD  CONSTRAINT [FK_UserEvent_User] FOREIGN KEY([UserEmail])
REFERENCES [dbo].[User] ([Email])

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_UserEvent_User]') AND parent_object_id = OBJECT_ID(N'[dbo].[UserEvent]'))
ALTER TABLE [dbo].[UserEvent] CHECK CONSTRAINT [FK_UserEvent_User]

/****** Object:  ForeignKey [FK_Winner_Category]    Script Date: 10/13/2011 10:47:23 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Winner_Category]') AND parent_object_id = OBJECT_ID(N'[dbo].[Winner]'))
ALTER TABLE [dbo].[Winner]  WITH CHECK ADD  CONSTRAINT [FK_Winner_Category] FOREIGN KEY([CatogoryId])
REFERENCES [dbo].[Category] ([CategoryId])

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Winner_Category]') AND parent_object_id = OBJECT_ID(N'[dbo].[Winner]'))
ALTER TABLE [dbo].[Winner] CHECK CONSTRAINT [FK_Winner_Category]

/****** Object:  ForeignKey [FK_Winner_Event]    Script Date: 10/13/2011 10:47:23 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Winner_Event]') AND parent_object_id = OBJECT_ID(N'[dbo].[Winner]'))
ALTER TABLE [dbo].[Winner]  WITH CHECK ADD  CONSTRAINT [FK_Winner_Event] FOREIGN KEY([EventId])
REFERENCES [dbo].[Event] ([EventId])

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Winner_Event]') AND parent_object_id = OBJECT_ID(N'[dbo].[Winner]'))
ALTER TABLE [dbo].[Winner] CHECK CONSTRAINT [FK_Winner_Event]

/****** Object:  ForeignKey [FK_Winner_User]    Script Date: 10/13/2011 10:47:23 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Winner_User]') AND parent_object_id = OBJECT_ID(N'[dbo].[Winner]'))
ALTER TABLE [dbo].[Winner]  WITH CHECK ADD  CONSTRAINT [FK_Winner_User] FOREIGN KEY([UserEmail])
REFERENCES [dbo].[User] ([Email])

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Winner_User]') AND parent_object_id = OBJECT_ID(N'[dbo].[Winner]'))
ALTER TABLE [dbo].[Winner] CHECK CONSTRAINT [FK_Winner_User]

ALTER TABLE "Event"
ADD IsAutomaticDrawing bit NOT NULL DEFAULT 0;