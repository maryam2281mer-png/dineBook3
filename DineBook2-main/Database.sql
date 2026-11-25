/* =====================================================================
   DineBook - Restaurant Table Reservation Management System
   SQL Server database script (re-runnable: safe to execute more than once)

   How to run:
     1. Open SQL Server Management Studio (or Visual Studio > SQL Server Object Explorer).
     2. Connect to (LocalDB)\MSSQLLocalDB  (or your SQL Server instance).
     3. Open this file and click Execute.
   ===================================================================== */

IF DB_ID(N'DineBookDB') IS NULL
    CREATE DATABASE DineBookDB;
GO

USE DineBookDB;
GO

/* ---------- 1. Users (customers and admin) ---------- */
IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users (
        UserID   INT IDENTITY(1,1) PRIMARY KEY,
        Name     NVARCHAR(100) NOT NULL,
        Email    NVARCHAR(150) NOT NULL UNIQUE,
        Password NVARCHAR(255) NOT NULL,          -- Base64(salt):Base64(PBKDF2 hash), never plain text
        Phone    NVARCHAR(20)  NULL,
        Role     NVARCHAR(20)  NOT NULL CONSTRAINT DF_Users_Role DEFAULT 'User',
        CONSTRAINT CK_Users_Role CHECK (Role IN ('User', 'Admin'))
    );
END
GO

/* ---------- 2. Services (= restaurants) ---------- */
IF OBJECT_ID(N'dbo.Services', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Services (
        ServiceID    INT IDENTITY(1,1) PRIMARY KEY,
        ServiceName  NVARCHAR(150) NOT NULL,
        Description  NVARCHAR(1000) NULL,
        Duration     INT NOT NULL,                  -- reservation length in minutes
        Price        DECIMAL(10,2) NOT NULL,        -- average price
        ServicePhoto NVARCHAR(500) NULL,
        CONSTRAINT CK_Services_Duration CHECK (Duration > 0),
        CONSTRAINT CK_Services_Price    CHECK (Price >= 0)
    );
END
GO

-- Extra column (not part of the required fields): max guests per reservation.
IF COL_LENGTH(N'dbo.Services', N'MaxGuests') IS NULL
BEGIN
    ALTER TABLE dbo.Services
        ADD MaxGuests INT NOT NULL CONSTRAINT DF_Services_MaxGuests DEFAULT 8;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_Services_MaxGuests')
BEGIN
    ALTER TABLE dbo.Services
        ADD CONSTRAINT CK_Services_MaxGuests CHECK (MaxGuests BETWEEN 1 AND 100);
END
GO

/* ---------- 3. Availability (time slots offered by each restaurant) ---------- */
IF OBJECT_ID(N'dbo.Availability', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Availability (
        AvailabilityID INT IDENTITY(1,1) PRIMARY KEY,
        ServiceID      INT  NOT NULL,
        AvailableDate  DATE NOT NULL,
        AvailableTime  TIME(0) NOT NULL,
        CONSTRAINT FK_Availability_Services
            FOREIGN KEY (ServiceID) REFERENCES dbo.Services(ServiceID),
        CONSTRAINT UQ_Availability_Slot
            UNIQUE (ServiceID, AvailableDate, AvailableTime)
    );
END
GO

/* ---------- 4. Appointments (table reservations) ---------- */
IF OBJECT_ID(N'dbo.Appointments', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Appointments (
        AppointmentID   INT IDENTITY(1,1) PRIMARY KEY,
        UserID          INT  NOT NULL,
        ServiceID       INT  NOT NULL,
        AppointmentDate DATE NOT NULL,
        AppointmentTime TIME(0) NOT NULL,

        -- extra reservation details
        NumberOfGuests  INT NOT NULL CONSTRAINT DF_Appointments_Guests DEFAULT 2,
        SpecialRequest  NVARCHAR(500) NULL,
        Status          NVARCHAR(30) NOT NULL CONSTRAINT DF_Appointments_Status DEFAULT 'Confirmed',

        CONSTRAINT FK_Appointments_Users
            FOREIGN KEY (UserID) REFERENCES dbo.Users(UserID),
        CONSTRAINT FK_Appointments_Services
            FOREIGN KEY (ServiceID) REFERENCES dbo.Services(ServiceID),
        CONSTRAINT CK_Appointments_Guests
            CHECK (NumberOfGuests > 0),
        CONSTRAINT CK_Appointments_Status
            CHECK (Status IN ('Confirmed', 'Cancelled', 'Completed'))
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Appointments_UserID')
    CREATE INDEX IX_Appointments_UserID ON dbo.Appointments(UserID);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Appointments_ServiceDate')
    CREATE INDEX IX_Appointments_ServiceDate ON dbo.Appointments(ServiceID, AppointmentDate);
GO

-- Prevents two ACTIVE reservations for the same restaurant, date and time.
-- Cancelled reservations do not block the slot.
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Appointments_ActiveSlot')
    CREATE UNIQUE INDEX UX_Appointments_ActiveSlot
    ON dbo.Appointments (ServiceID, AppointmentDate, AppointmentTime)
    WHERE Status <> 'Cancelled';
GO

/* =====================================================================
   SAMPLE DATA (inserted only when the tables are empty)
   Accounts (DEMO ONLY - change or delete before any real use):
     Admin : admin@dinebook.com  /  DineBook@Admin2026
     User  : demo@dinebook.com   /  User@12345
   ===================================================================== */

IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Email = 'admin@dinebook.com')
    INSERT INTO dbo.Users (Name, Email, Password, Phone, Role)
    VALUES ('DineBook Admin', 'admin@dinebook.com',
            'oTbt5aFQuofUfpMgsVGi3Q==:raJ7NHuA9Y5m4VesjxrowWzcGiLU5hj+lCJ5UyJyg0c=',
            NULL, 'Admin');
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Email = 'demo@dinebook.com')
    INSERT INTO dbo.Users (Name, Email, Password, Phone, Role)
    VALUES ('Demo Customer', 'demo@dinebook.com',
            '9Xsn8o1j+O2vtEkKvh8R9Q==:jcCP6LqSY8fS2E6K5tuq8AsrMIjVaEcJ3dzUv4dYv5c=',
            '0500000000', 'User');
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Services)
BEGIN
    INSERT INTO dbo.Services (ServiceName, Description, Duration, Price, MaxGuests, ServicePhoto)
    VALUES
    ('The Garden Restaurant', 'A relaxing restaurant offering a variety of international dishes.', 120, 150, 8, NULL),
    ('Bella Italia',          'Enjoy authentic Italian food, pasta, and delicious pizza.',          120, 180, 6, NULL),
    ('Ocean View',            'A seafood restaurant with a beautiful dining atmosphere.',           120, 220, 10, NULL),
    ('Al Nakheel Grill',      'Traditional grilled meats and Arabian mezze in a warm setting.',     90,  130, 12, NULL),
    ('Sakura House',          'Fresh sushi and Japanese dishes prepared by expert chefs.',          90,  200, 6, NULL);
END
GO

-- Availability for the next 14 days (generated relative to today, so it never expires)
IF NOT EXISTS (SELECT 1 FROM dbo.Availability)
BEGIN
    INSERT INTO dbo.Availability (ServiceID, AvailableDate, AvailableTime)
    SELECT S.ServiceID,
           DATEADD(DAY, D.n, CAST(GETDATE() AS DATE)),
           T.t
    FROM dbo.Services S
    CROSS JOIN (VALUES (1),(2),(3),(4),(5),(6),(7),(8),(9),(10),(11),(12),(13),(14)) D(n)
    CROSS JOIN (VALUES (CAST('13:00' AS TIME(0))), (CAST('18:00' AS TIME(0))),
                       (CAST('20:00' AS TIME(0)))) T(t);
END
GO

-- One sample reservation for the demo customer (tomorrow at 18:00, first restaurant)
IF NOT EXISTS (SELECT 1 FROM dbo.Appointments)
BEGIN
    INSERT INTO dbo.Appointments
        (UserID, ServiceID, AppointmentDate, AppointmentTime, NumberOfGuests, SpecialRequest, Status)
    SELECT U.UserID, S.ServiceID, DATEADD(DAY, 1, CAST(GETDATE() AS DATE)),
           CAST('18:00' AS TIME(0)), 2, 'Window seat, please.', 'Confirmed'
    FROM dbo.Users U
    CROSS JOIN (SELECT TOP 1 ServiceID FROM dbo.Services ORDER BY ServiceID) S
    WHERE U.Email = 'demo@dinebook.com';
END
GO
