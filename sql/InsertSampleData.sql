USE InventoryManagementSystem

INSERT INTO [User](Username, [Password], FullName, Email, PhoneNumber, CreateDate)
VALUES
	('user1', '1111', N'一一一', 'user1@mail.com', '0900000001', '2021-06-21 18:04:05.111'),
	('user2', '2222', N'二二二', 'user2@mail.com', '0900000002', '2021-06-23 18:03:05.112'),
	('user3', '3333', N'三三三', 'user3@mail.com', '0900000003', '2021-06-23 18:04:05.113'),
	('user4', '4444', N'四四四', 'user4@mail.com', '0900000004', '2021-06-24 10:04:05.114'),
	('user5', '5555', N'五五五', 'user5@mail.com', '0900000005', '2021-06-24 11:04:05.115'),
	('user6', '6666', N'六六六', 'user6@mail.com', '0900000006', '2021-06-24 12:04:05.116'),
	('user7', '7777', N'七七七', 'user7@mail.com', '0900000007', '2021-06-26 15:04:05.117'),
	('user8', '8888', N'八八八', 'user8@mail.com', '0900000008', '2021-06-27 18:04:05.118'),
	('user9', '9999', N'九九九', 'user9@mail.com', '0900000009', '2021-06-29 18:04:05.119')

	INSERT INTO [EquipCategory] (CategoryName)
	VALUES
		(N'3C'),
		(N'文具'),
		(N'清潔'),
		(N'數位'),
		(N'生活'),
		(N'運動')


