# DineBook

Restaurant Table Reservation Management System (MIS 366).

ASP.NET Web Forms (.NET Framework 4.8) · C# · SQL Server · ADO.NET · CSS · Master Page · Session

## Requirements (Windows)
- Visual Studio 2019 or 2022 with the **ASP.NET and web development** workload
  (this also installs IIS Express and SQL Server LocalDB)
- .NET Framework 4.8 Developer Pack (included with recent Visual Studio versions)

## Run it step by step
1. **Create the database.** In Visual Studio open *View > SQL Server Object Explorer*,
   expand *(localdb)\MSSQLLocalDB*, right-click it > *New Query*, paste the contents of
   `Database.sql`, and click *Execute* (or run the file in SQL Server Management Studio).
   The script is re-runnable and creates tables, constraints, indexes and sample data.
2. **Open the project.** Double-click `DineBook.csproj` (or *File > Open > Project*).
3. **Check the connection string** in `Web.config` (name `DineBookConnection`).
   - LocalDB (default): `Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=DineBookDB;Integrated Security=True`
   - SQL Server Express: change `Data Source` to `.\SQLEXPRESS`
   - Full SQL Server: use your server name.
4. **Build and run.** *Build > Rebuild Solution*, then press **F5** (IIS Express).
   If Visual Studio asks to create the IIS Express site, click **Yes**.

## Demo accounts (sample data only - change or delete before any real use)
| Role  | Email                | Password            |
|-------|----------------------|---------------------|
| Admin | admin@dinebook.com   | DineBook@Admin2026  |
| User  | demo@dinebook.com    | User@12345          |

Admin sign-in page: `/Admin/AdminLogin.aspx`. There is no public admin registration.

## Features
**Customers:** register, log in/out, browse restaurants, view details, pick a date, pick an
available time, choose guests (limited per restaurant), add a special request, book,
view and cancel their own reservations, edit their profile.

**Admin:** dashboard with statistics; full create/read/update/delete for users, restaurants
and availability; view, change status of, and delete reservations.

## Database
Required tables and fields are unchanged: `Users`, `Services` (restaurants),
`Appointments` (reservations), `Availability`.
Extra columns: `Appointments.NumberOfGuests / SpecialRequest / Status`, `Services.MaxGuests`.

Double-booking is blocked by the filtered unique index `UX_Appointments_ActiveSlot`
(restaurant + date + time, ignoring cancelled reservations), and the booking page only
offers times that exist in `Availability` and are not already booked.

## Security notes
- All SQL uses parameters (no string concatenation).
- Passwords are stored as PBKDF2 hashes with a random salt (`Helpers/PasswordHelper.cs`).
- Every user page checks the session; every admin page checks for the `Admin` role,
  so typing a URL directly does not bypass login.
- Set `<customErrors mode="On" />` in `Web.config` when deploying.

## Design
Background: #FAF6F0 · Primary: #9B1C27
