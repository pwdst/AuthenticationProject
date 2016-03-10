CREATE TABLE [security].[UserIdentityVersion] (
	[Id]				[int]		IDENTITY(1,1),
	[HashIterations]	[int]		NOT NULL,
	[Default]			[bit]		NOT NULL,
	CONSTRAINT PK_UserIdentityVersion PRIMARY KEY CLUSTERED ([Id] ASC)
)

GO

CREATE UNIQUE INDEX IX_UserIdentityVersion_Default ON [security].[UserIdentityVersion]([Default]) WHERE [Default] = 1

GO

ALTER TABLE [security].[UserIdentityVersion]
ADD CONSTRAINT [CK_UserIdentityVersion_HashIterations] CHECK ([HashIterations] > 0)

CREATE TABLE [security].[User] (
	[Id]						[uniqueidentifier]	DEFAULT NEWSEQUENTIALID(),
	[UserName]					[varchar](25)		NOT NULL,
	[DisplayName]				[nvarchar](50)		NOT NULL,
	[EmailAddress]				[nvarchar](254)		NOT NULL,
	[PasswordHash]				[nvarchar](100)		NOT	NULL,
	[PasswordSalt]				[nvarchar](100)		NOT NULL,
	[TwitterUserName]			[varchar](15)		NOT NULL,
	[UserIdentityVersion_id]	[int]				NOT NULL,
	CONSTRAINT PK_User PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT AK_UserName UNIQUE([UserName]),
	CONSTRAINT AK_EmailAddress UNIQUE([EmailAddress])
)

GO

CREATE UNIQUE NONCLUSTERED INDEX IX_UserName ON [security].[User]([UserName])

ALTER TABLE [security].[User] WITH CHECK
ADD CONSTRAINT [FK_User_UserIdentityVersion] FOREIGN KEY([UserIdentityVersion_id])
REFERENCES [security].[UserIdentityVersion]