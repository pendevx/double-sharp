IF OBJECT_ID('Roles', 'U') IS NOT NULL
BEGIN
    
DECLARE @userRole NCHAR(4) = 'User';
DECLARE @adminRole NCHAR(5) = 'Admin';

DECLARE @userExists BIT = (IIF(EXISTS(SELECT Id FROM Roles WHERE Name = @userRole), 1, 0));
DECLARE @adminExists BIT = (IIF(EXISTS(SELECT Id FROM Roles WHERE Name = @adminRole), 1, 0));
    
IF (@userExists = 0)
    BEGIN
       INSERT INTO Roles (Name, CreatedOn) VALUES (@userRole, SYSUTCDATETIME()); 
    END

IF (@adminExists = 0)
    BEGIN
        INSERT INTO Roles (Name, CreatedOn) VALUES (@adminRole, SYSUTCDATETIME());
    END
    
END
