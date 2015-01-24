
print('insert into dispatchr.Users...');
insert into dispatchr.Users 
	([Name], [Email], [Enabled])
values 
	('JERRY NIXON', 'jerry.nixon@solarizr.com', 1),
	('DAREN MAY', 'daren.may@solarizr.com', 1),
	('JAMES SMITH', 'james.smith@solarizr.com', 1),
	('JOHN JOHNSON', 'john@solarizr.onmicrosoft.com', 1),
	('ROBERT WILLIAMS', 'robert.williams@solarizr.com', 1),
	('MARCUS PARSONS', 'marcus.parsons@solarizr.com', 0);

print('insert into dispatchr.Statuses');
insert into dispatchr.Statuses 
	([Name])
values 
	('Pending'), 
	('Approved'), 
	('Denied');

print('insert into dispatchr.Appointments');
declare @i int = 0
while @i < 40 begin 
    set @i = @i + 1
	insert into dispatchr.Appointments 
		(
			 [Date]
			,[Agent]
			,[Location]
			,[Latitude]
			,[Longitude]
			,[Map]
			,[Phone]
			,[StatusId]
		)
	values 
		(
			 dateadd(hh, @i * 6, getdate())
			,4 -- agent
			,'123 Main Street, Denver, CO 80202'
			,39.7392
			,-104.9847
			,'ms-appx:///SampleData/DesignHeroMap.png'
			,'303-555-1234'
			,1 -- status
		);
end
