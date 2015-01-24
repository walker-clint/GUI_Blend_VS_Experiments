CREATE TABLE dispatchr.Users
(
	[Id] INT NOT NULL PRIMARY KEY identity(1,1),
	[Name] varchar(250),
	[Email] varchar(250) unique,
	[Enabled] bit default 1
)
