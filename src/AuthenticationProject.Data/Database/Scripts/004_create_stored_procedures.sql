IF OBJECT_ID ( 'security.CreateUser', 'P' ) IS NOT NULL 
    DROP PROCEDURE [security].[CreateUser];
GO
CREATE PROCEDURE [security].[CreateUser]
(
	@userName [varchar](25),
	@displayName [nvarchar](50),
	@emailAddress [nvarchar](254),
	@passwordHash [nvarchar](100),
	@passwordSalt [nvarchar](100),
	@twitterUserName [varchar](15),
	@userIdentityVersion int
)
AS
    SET NOCOUNT ON;
    
	INSERT INTO [security].[User]
           ([UserName]
           ,[DisplayName]
           ,[EmailAddress]
           ,[PasswordHash]
           ,[PasswordSalt]
           ,[TwitterUserName],
		   [UserIdentityVersion_id])
     VALUES
           (@userName
           ,@displayName
           ,@emailAddress
           ,@passwordHash
           ,@passwordSalt
           ,@twitterUserName
		   ,@userIdentityVersion)
GO

IF OBJECT_ID ( 'security.GetUserCredentials', 'P' ) IS NOT NULL 
    DROP PROCEDURE [security].[GetUserCredentials];
GO
CREATE PROCEDURE [security].[GetUserCredentials]
(
	@userName [nvarchar](25)
)
AS
    SET NOCOUNT ON;

	SELECT u.[PasswordHash], u.[PasswordSalt], uiv.[HashIterations]
	FROM [security].[User] u
	INNER JOIN [security].[UserIdentityVersion] uiv ON u.[UserIdentityVersion_id] = uiv.[Id]
	WHERE [UserName] = @userName
GO

IF OBJECT_ID ( 'security.GetDefaultIdentityVersion', 'P' ) IS NOT NULL 
    DROP PROCEDURE [security].[GetDefaultIdentityVersion];
GO
CREATE PROCEDURE [security].[GetDefaultIdentityVersion]
AS
    SET NOCOUNT ON;

	SELECT TOP(1) 'IdentityVersion' = [Id], 'HashIterations' = [HashIterations] FROM [security].[UserIdentityVersion] 
	WHERE [Default] = 1 
	ORDER BY [Id] DESC
GO

IF OBJECT_ID ( 'security.UpdatePassword', 'P' ) IS NOT NULL 
    DROP PROCEDURE [security].[UpdatePassword];
GO
CREATE PROCEDURE [security].[UpdatePassword]
(
	@userId [uniqueidentifier],
	@passwordHash [nvarchar](100),
	@userIdentityVersion int
)
AS
    SET NOCOUNT ON;
    
	UPDATE [security].[User]
    SET		[PasswordHash] = @passwordHash,
			[UserIdentityVersion_id] = @userIdentityVersion
	WHERE [Id] = @userId
GO