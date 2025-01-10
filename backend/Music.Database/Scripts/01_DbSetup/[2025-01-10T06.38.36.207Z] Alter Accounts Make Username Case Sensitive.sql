IF COL_LENGTH('Accounts', 'Username') IS NOT NULL
BEGIN
    ALTER TABLE Accounts DROP CONSTRAINT Accounts_Username_Unique
    
    ALTER TABLE Accounts
    ALTER COLUMN Username VARCHAR(256) COLLATE Latin1_General_CS_AS NOT NULL
    
    ALTER TABLE Accounts ADD CONSTRAINT Accounts_Username_Unique UNIQUE(Username)
END
