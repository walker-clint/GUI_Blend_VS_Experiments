CREATE TABLE [dispatchr].[Photos]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[AppointmentId] int references dispatchr.Appointments([Id]),
	[Path] varchar(max),
)
