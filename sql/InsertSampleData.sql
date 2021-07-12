USE InventoryManagementSystem

INSERT INTO [Role]([RoleName])
VALUES
	(N'高級管理員'),
	(N'一般管理員')

INSERT INTO [Admin](RoleID, Username, Password, FullName)
VALUES
	(1, 'sun', '1111', N'太陽陽'),
	(2, 'moon', '0000', N'月亮亮')

INSERT INTO [User](Username, [Password], FullName, Email, PhoneNumber, CreateTime)
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
	(N'3C'),	--1
	(N'文具'),	--2
	(N'清潔'),	--3
	(N'電器'),	--4
	(N'生活'),	--5
	(N'運動')	--6

INSERT INTO [Equipment](EquipmentCategoryID, EquipmentSN, EquipmentName, Brand, Model, UnitPrice)
VALUES
	(1, 'SN1', N'滑鼠', N'Brand1', N'Model1', 200),
	(1, 'SN2', N'滑鼠', N'Brand1', N'Model2', 300),
	(1, 'SN3', N'滑鼠', N'Brand2', N'Model3', 300),
	(1, 'SN4', N'充電線', N'Brand3', N'Model4', 300),
	(1, 'SN5', N'轉接頭', N'Brand4', N'Model5', 300),
	(4, 'SN6', N'立扇', N'Brand1', N'Model6', 1300),
	(4, 'SN7', N'工業用電扇', N'Brand3', N'Model7', 300),
	(1, 'SN8', N'延長線', N'Brand6', N'Model8', 700),
	(2, 'SN9', N'原子筆', N'Brand7', N'Model9', 30),
	(2, 'SN10', N'直尺', N'Brand3', N'Model10', 30),
	(2, 'SN11', N'圓規', N'Brand7', N'Model11', 60),
	(1, 'SN12', N'筆電', N'Brand1', N'Model12', 30000),
	(6, 'SN13', N'槓片', N'Brand4', N'Model13', 3000),
	(6, 'SN14', N'槓鈴', N'Brand5', N'Model14', 6000),
	(6, 'SN15', N'啞鈴', N'Brand8', N'Model15', 2500),
	(3, 'SN16', N'掃把', N'Brand9', N'Model16', 100),
	(3, 'SN17', N'拖把', N'Brand2', N'Model17', 150),
	(5, 'SN18', N'鎖頭', N'Brand4', N'Model18', 250),
	(5, 'SN19', N'螺絲起子', N'Brand9', N'Model19', 300),
	(5, 'SN20', N'六角扳手', N'Brand5', N'Model20', 300),
	(5, 'SN21', N'捲尺', N'Brand8', N'Model21', 300),
	(5, 'SN22', N'鎚子', N'Brand9', N'Model22', 300)

INSERT INTO [Condition](ConditionID, ConditionName)
VALUES
	('I', N'入庫'),
	('O', N'出庫'),
	('S', N'報廢'),
	('F', N'損壞'),
	('L', N'遺失'),
        ('P', N'待領'),
        ('D', N'刪除')

INSERT INTO [Item](ConditionID, EquipmentID, ItemSN, [Description])
VALUES
	('I', 22, 'SN1', N'好鎚子'),
	('I', 13, 'SN2', N'好槓片'),
	('I', 17, 'SN3', N'好掃把'),
	('I', 10, 'SN4', N'好直尺'),
	('I', 14, 'SN5', N'好槓鈴'),
	('I', 15, 'SN6', N'好啞鈴'),
	('I', 6 , 'SN7', N'好立扇'),
	('I', 15, 'SN8', N'好啞鈴'),
	('I', 17, 'SN9', N'好拖把'),
	('I', 9 , 'SN10', N'好原子筆'),
	('I', 19, 'SN11', N'好螺絲起子'),
	('I', 20, 'SN12', N'好六角扳手'),
	('I', 6 , 'SN13', N'好立扇'),
	('I', 7 , 'SN14', N''),
	('I', 17, 'SN15', N'好拖把'),
	('I', 16, 'SN16', N''),
	('I', 13, 'SN17', N''),
	('I', 21, 'SN18', N''),
	('I', 17, 'SN19', N'好拖把'),
	('I', 15, 'SN20', N'好啞鈴'),
	('I', 6 , 'SN21', N'好立扇'),
	('I', 6 , 'SN22', N'好立扇'),
	('I', 12, 'SN23', N''),
	('I', 11, 'SN24', N''),
	('I', 13, 'SN25', N''),
	('I', 10, 'SN26', N'好直尺'),
	('I', 11, 'SN27', N''),
	('I', 12, 'SN28', N''),
	('I', 12, 'SN29', N''),
	('I', 7 , 'SN30', N''),
	('I', 17, 'SN31', N'好拖把'),
	('I', 22, 'SN32', N''),
	('I', 22, 'SN33', N''),
	('I', 1 , 'SN34', N''),
	('I', 10, 'SN35', N'好直尺'),
	('I', 16, 'SN36', N''),
	('I', 20, 'SN37', N'好六角扳手'),
	('I', 1 , 'SN38', N''),
	('I', 21, 'SN39', N''),
	('I', 22, 'SN40', N''),
	('I', 6 , 'SN41', N'好立扇'),
	('I', 17, 'SN42', N'好拖把'),
	('I', 17, 'SN43', N'好拖把'),
	('I', 17, 'SN44', N'好拖把'),
	('I', 17, 'SN45', N'好拖把'),
	('I', 21, 'SN46', N''),
	('I', 18, 'SN47', N''),
	('I', 7 , 'SN48', N''),
	('I', 7 , 'SN49', N''),
	('I', 16, 'SN50', N''),
	('I', 20, 'SN51', N'好六角扳手'),
	('I', 12, 'SN52', N''),
	('I', 3 , 'SN53', N''),
	('I', 16, 'SN54', N''),
	('I', 13, 'SN55', N''),
	('I', 10, 'SN56', N'好直尺'),
	('I', 10, 'SN57', N'好直尺'),
	('I', 7 , 'SN58', N''),
	('I', 3 , 'SN59', N''),
	('I', 6 , 'SN60', N'好立扇'),
	('I', 16, 'SN61', N''),
	('I', 3 , 'SN62', N''),
	('I', 4 , 'SN63', N''),
	('I', 4 , 'SN64', N''),
	('I', 16, 'SN65', N''),
	('I', 4 , 'SN66', N''),
	('I', 14, 'SN67', N'好槓鈴'),
	('I', 21, 'SN68', N''),
	('I', 15, 'SN69', N'好啞鈴'),
	('I', 16, 'SN70', N''),
	('I', 11, 'SN71', N''),
	('I', 15, 'SN72', N'好啞鈴')

INSERT INTO [ItemLog](AdminID, ItemID, ConditionID)
VALUES
	(1, 1, 'I'), 
	(1, 2, 'I'), 
	(1, 3, 'I'), 
	(1, 4, 'I'), 
	(1, 5, 'I'), 
	(1, 6, 'I'), 
	(1, 7, 'I'), 
	(1, 8, 'I'), 
	(1, 9, 'I'), 
	(1, 10, 'I'), 
	(1, 11, 'I'), 
	(1, 12, 'I'), 
	(1, 13, 'I'), 
	(1, 14, 'I'), 
	(1, 15, 'I'), 
	(1, 16, 'I'), 
	(1, 17, 'I'), 
	(1, 18, 'I'), 
	(1, 19, 'I'), 
	(1, 20, 'I'), 
	(1, 21, 'I'), 
	(1, 22, 'I'), 
	(1, 23, 'I'), 
	(1, 24, 'I'), 
	(1, 25, 'I'), 
	(1, 26, 'I'), 
	(1, 27, 'I'), 
	(1, 28, 'I'), 
	(1, 29, 'I'), 
	(1, 30, 'I'), 
	(1, 31, 'I'), 
	(1, 32, 'I'), 
	(1, 33, 'I'), 
	(1, 34, 'I'), 
	(1, 35, 'I'), 
	(1, 36, 'I'), 
	(1, 37, 'I'), 
	(1, 38, 'I'), 
	(1, 39, 'I'), 
	(1, 40, 'I'), 
	(1, 41, 'I'), 
	(1, 42, 'I'), 
	(1, 43, 'I'), 
	(1, 44, 'I'), 
	(1, 45, 'I'), 
	(1, 46, 'I'), 
	(1, 47, 'I'), 
	(1, 48, 'I'), 
	(1, 49, 'I'), 
	(1, 50, 'I'), 
	(1, 51, 'I'), 
	(1, 52, 'I'), 
	(1, 53, 'I'), 
	(1, 54, 'I'), 
	(1, 55, 'I'), 
	(1, 56, 'I'), 
	(1, 57, 'I'), 
	(1, 58, 'I'), 
	(1, 59, 'I'), 
	(1, 60, 'I'), 
	(1, 61, 'I'), 
	(1, 62, 'I'), 
	(1, 63, 'I'), 
	(1, 64, 'I'), 
	(1, 65, 'I'), 
	(1, 66, 'I'), 
	(1, 67, 'I'), 
	(1, 68, 'I'), 
	(1, 69, 'I'), 
	(1, 70, 'I'), 
	(1, 71, 'I'), 
	(1, 72, 'I') 

INSERT INTO [OrderStatus]([OrderStatusID], [StatusName])
VALUES
        ('P', N'處理中'),
        ('A', N'已成立'),
        ('D', N'不成立'),
        ('E', N'已結束'),
        ('C', N'已取消')

INSERT INTO [PaymentCategory]([PaymentCategoryID], [PaymentCategoryName])
VALUES
        ('P', N'租賃費'),        
        ('F', N'違規費')

INSERT INTO [OrderDetailStatus]([OrderDetailStatusID], [StatusName])
VALUES
        ('P', N'待領取'),
        ('T', N'已領取'),
        ('R', N'已歸還'),
        ('L', N'遺失'),
        ('C', N'取消')
