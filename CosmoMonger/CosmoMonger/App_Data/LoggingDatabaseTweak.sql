/*
   Tuesday, March 31, 20093:58:26 PM
   User: 
   Server: ASUSNOTEBOOK
   Database: CosmoMongerLog
   Application: 
*/

/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
CREATE TABLE dbo.Tmp_Log
	(
	LogID int NOT NULL IDENTITY (1, 1),
	Priority int NOT NULL,
	Severity nvarchar(32) NOT NULL,
	Title nvarchar(256) NOT NULL,
	Timestamp datetime NOT NULL,
	ProcessID int NOT NULL,
	ThreadName nvarchar(512) NULL,
	Win32ThreadId int NULL,
	Message nvarchar(1500) NULL,
	FormattedMessage ntext NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO

SET IDENTITY_INSERT dbo.Tmp_Log ON
GO
IF EXISTS(SELECT * FROM dbo.[Log])
	 EXEC('INSERT INTO dbo.Tmp_Log (LogID, Priority, Severity, Title, Timestamp, ProcessID, ThreadName, Win32ThreadId, Message, FormattedMessage)
		SELECT LogID, Priority, Severity, Title, Timestamp, CONVERT(int, ProcessID), ThreadName, CONVERT(int, Win32ThreadId), Message, FormattedMessage FROM dbo.[Log] WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_Log OFF
GO
ALTER TABLE dbo.CategoryLog
	DROP CONSTRAINT FK_CategoryLog_Log
GO
DROP TABLE dbo.[Log]
GO
EXECUTE sp_rename N'dbo.Tmp_Log', N'Log', 'OBJECT' 
GO
ALTER TABLE dbo.[Log] ADD CONSTRAINT
	PK_Log PRIMARY KEY CLUSTERED 
	(
	LogID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.CategoryLog ADD CONSTRAINT
	FK_CategoryLog_Log FOREIGN KEY
	(
	LogID
	) REFERENCES dbo.[Log]
	(
	LogID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

COMMIT
GO
/****** Object:  Stored Procedure dbo.WriteLog    Script Date: 10/1/2004 3:16:36 PM ******/

ALTER PROCEDURE [dbo].[WriteLog]
(
	@EventID int, 
	@Priority int, 
	@Severity nvarchar(32), 
	@Title nvarchar(256), 
	@Timestamp datetime,
	@MachineName nvarchar(32), 
	@AppDomainName nvarchar(512),
	@ProcessID nvarchar(256),
	@ProcessName nvarchar(512),
	@ThreadName nvarchar(512),
	@Win32ThreadId nvarchar(128),
	@Message nvarchar(1500),
	@FormattedMessage ntext,
	@LogId int OUTPUT
)
AS 

	INSERT INTO [Log] (
		Priority,
		Severity,
		Title,
		[Timestamp],
		ProcessID,
		ThreadName,
		Win32ThreadId,
		Message,
		FormattedMessage
	)
	VALUES (
		@Priority, 
		@Severity, 
		@Title, 
		@Timestamp,
		@ProcessID,
		@ThreadName,
		@Win32ThreadId,
		@Message,
		@FormattedMessage)

	SET @LogID = @@IDENTITY
	RETURN @LogID
GO


