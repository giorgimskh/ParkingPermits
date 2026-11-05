IF DB_ID(N'ParkingPermitsDb') IS NULL
    CREATE DATABASE ParkingPermitsDb;
GO
-- LocalDB turns AUTO_CLOSE on by default, which shuts the database down after every
-- query and makes the next connection slow; keep it open instead
ALTER DATABASE ParkingPermitsDb SET AUTO_CLOSE OFF;
GO
USE ParkingPermitsDb;
GO
IF OBJECT_ID(N'dbo.Permits', N'U') IS NULL
CREATE TABLE Permits
(
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    PlateNumber NVARCHAR(20)  NOT NULL,   -- text, searched
    OwnerName   NVARCHAR(100) NOT NULL,   -- text, searched
    PermitType  NVARCHAR(20)  NOT NULL,   -- Student / Staff / Guest, the filter column
    Zone        NVARCHAR(10)  NOT NULL,   -- A / B / C / D
    ValidUntil  DATE          NOT NULL,   -- a date
    IsPaid      BIT           NOT NULL    -- true / false
);
GO
-- Rule: once a permit is paid it can never be changed back to unpaid.
-- New permits may still be added as unpaid (they wait for payment) and later be paid.
-- CREATE OR ALTER lets this script run twice without an error.
CREATE OR ALTER TRIGGER trg_Permits_PaidCannotBeUndone
ON Permits
AFTER UPDATE
AS
BEGIN
    IF EXISTS (SELECT 1
               FROM inserted
               JOIN deleted ON deleted.Id = inserted.Id
               WHERE deleted.IsPaid = 1 AND inserted.IsPaid = 0)
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50001, N'A paid permit cannot be changed back to unpaid.', 1;
    END
END
GO
-- How the test data is chosen (dates are for the 2026/27 academic year):
--   Student permits end with a semester (31 Jan 2027) or the academic year (30 Jun 2027);
--     last year's student permits ended on 30 Jun 2026.
--   Staff permits end with a calendar year (31 Dec 2026 or 31 Dec 2027).
--   Guest permits are short visits of a few days or weeks.
--   A permit can be used only when it is paid. Unpaid rows are new permits that are
--     still waiting for payment; every expired permit was paid while it was in use.
IF NOT EXISTS (SELECT 1 FROM Permits)
BEGIN
    INSERT INTO Permits (PlateNumber, OwnerName, PermitType, Zone, ValidUntil, IsPaid) VALUES
    (N'AB-123-CD', N'Giorgi Beridze',        N'Student', N'A', '2027-06-30', 1),
    (N'KT-456-MN', N'Nino Beridze',          N'Staff',   N'B', '2027-12-31', 1),
    (N'TB-789-XY', N'Levan Beridze',         N'Guest',   N'C', '2026-11-20', 1),
    (N'QQ-202-AA', N'Mariam Kapanadze',      N'Student', N'A', '2026-06-30', 1),
    (N'GG-555-LL', N'Davit Lomidze',         N'Staff',   N'D', '2027-12-31', 1),
    (N'ZZ-310-BK', N'Ana Gelashvili',        N'Student', N'B', '2027-01-31', 0),
    (N'MK-777-SS', N'Irakli Tsiklauri',      N'Guest',   N'C', '2026-10-15', 1),
    (N'LR-148-TT', N'Tamar Jgharkava',       N'Staff',   N'A', '2026-12-31', 1),
    (N'BC-909-DE', N'Saba Chkheidze',        N'Student', N'D', '2027-01-31', 1),
    (N'NN-321-OP', N'Elene Kvaratskhelia',   N'Student', N'C', '2027-06-30', 1),
    (N'RS-642-UV', N'Zurab Kiknadze',        N'Staff',   N'B', '2027-12-31', 1),
    (N'WX-818-YZ', N'Luka Maisuradze',       N'Guest',   N'A', '2026-11-10', 1),
    (N'HJ-404-KL', N'Salome Gogichaishvili', N'Student', N'B', '2026-06-30', 1),
    (N'PT-250-QR', N'Nika Abashidze',        N'Staff',   N'C', '2027-12-31', 0),
    (N'FD-616-GH', N'Nino Kharadze',         N'Guest',   N'D', '2026-11-28', 1),
    (N'KU-001-TS', N'Ketevan Abashidze',     N'Student', N'A', '2027-01-31', 1),
    (N'EV-512-CH', N'Beka Japaridze',        N'Guest',   N'B', '2026-11-30', 0),
    (N'SM-733-RN', N'Giorgi Lomidze',        N'Staff',   N'D', '2026-12-31', 1);
END
GO
