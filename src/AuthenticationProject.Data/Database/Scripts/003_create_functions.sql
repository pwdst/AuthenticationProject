IF OBJECT_ID ( 'security.UserNameExists', 'FN' ) IS NOT NULL 
    DROP FUNCTION [security].[UserNameExists];
GO
CREATE FUNCTION [security].[UserNameExists] 
(
	@userName [varchar](25)
)
RETURNS bit
WITH SCHEMABINDING 
AS
BEGIN
	DECLARE @exists bit;

	IF (EXISTS (SELECT COUNT(1) FROM [security].[User] WHERE [UserName] = @userName))
	BEGIN
		SET @exists = 1;
	END
	ELSE
	BEGIN
		SET @exists = 0;
	END

	RETURN @exists;
END

GO

IF OBJECT_ID ( 'security.EmailAddressExists', 'FN' ) IS NOT NULL 
    DROP FUNCTION [security].[EmailAddressExists];
GO
CREATE FUNCTION [security].[EmailAddressExists] 
(
	@emailAddress [nvarchar](254)
)
RETURNS bit
WITH SCHEMABINDING 
AS
BEGIN
	DECLARE @exists bit;

	IF (EXISTS (SELECT COUNT(1) FROM [security].[User] WHERE [EmailAddress] = @emailAddress))
	BEGIN
		SET @exists = 1;
	END
	ELSE
	BEGIN
		SET @exists = 0;
	END

	RETURN @exists;
END

GO