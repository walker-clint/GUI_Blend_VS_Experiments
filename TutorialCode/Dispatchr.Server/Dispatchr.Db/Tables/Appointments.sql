CREATE TABLE [dispatchr].[Appointments]
(
	[Id] INT NOT NULL PRIMARY KEY identity(1,1),
	[Date] datetime default getdate() not null,
	[Details] varchar(max),
	[Agent] int references dispatchr.Users(Id),
	[Location] varchar(max),
	[Latitude] float NOT NULL,
	[Longitude] float NOT NULL,
	[Phone] varchar(50),
	[StatusId] int references dispatchr.Statuses([Id]),
	[Map] varchar(max),
)

GO

CREATE INDEX ix_Dispatchr_Appointments ON dispatchr.Appointments (Agent)
