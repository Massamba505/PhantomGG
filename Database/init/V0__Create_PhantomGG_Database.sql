IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'PhantomGG')
BEGIN
    CREATE DATABASE PhantomGG;
END
GO