CREATE TABLE YourTable (
    Id INT PRIMARY KEY IDENTITY(1,1),  -- Auto-incrementing primary key
    Column1 NVARCHAR(100) NOT NULL,    -- A string column that cannot be null
    CreatedAt DATETIME DEFAULT GETDATE() -- Timestamp of when the record was created
);

INSERT INTO YourTable (Column1) VALUES ('Sample Data');	



Server=tcp:abaranwal-sql-srv.database.windows.net,1433;Initial Catalog=abaranwal-primary;Persist Security Info=False;User ID={your_username};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Authentication="Active Directory Integrated";
