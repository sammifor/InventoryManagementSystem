USE InventoryManagementSystem

INSERT INTO [Role]([RoleName])
VALUES
	(N'高級管理員'),
	(N'一般管理員')

INSERT INTO [Admin](RoleID, Username, Password, FullName)
VALUES
	(1, 'sun', '1111', N'太陽陽'),
	(2, 'moon', '0000', N'月亮亮')

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

INSERT INTO [Item](EquipmentID, ItemSN, [Description])
VALUES
	(22, 'SN1', N'好鎚子'),
	(13, 'SN2', N'好槓片'),
	(17, 'SN3', N'好掃把'),
	(10, 'SN4', N'好直尺'),
	(14, 'SN5', N'好槓鈴'),
	(15, 'SN6', N'好啞鈴'),
	(6 , 'SN7', N'好立扇'),
	(15, 'SN8', N'好啞鈴'),
	(17, 'SN9', N'好拖把'),
	(9 , 'SN10', N'好原子筆'),
	(19, 'SN11', N'好螺絲起子'),
	(20, 'SN12', N'好六角扳手'),
	(6 , 'SN13', N'好立扇'),
	(7 , 'SN14', N''),
	(17, 'SN15', N'好拖把'),
	(16, 'SN16', N''),
	(13, 'SN17', N''),
	(21, 'SN18', N''),
	(17, 'SN19', N'好拖把'),
	(15, 'SN20', N'好啞鈴'),
	(6 , 'SN21', N'好立扇'),
	(6 , 'SN22', N'好立扇'),
	(12, 'SN23', N''),
	(11, 'SN24', N''),
	(13, 'SN25', N''),
	(10, 'SN26', N'好直尺'),
	(11, 'SN27', N''),
	(12, 'SN28', N''),
	(12, 'SN29', N''),
	(7 , 'SN30', N''),
	(17, 'SN31', N'好拖把'),
	(22, 'SN32', N''),
	(22, 'SN33', N''),
	(1 , 'SN34', N''),
	(10, 'SN35', N'好直尺'),
	(16, 'SN36', N''),
	(20, 'SN37', N'好六角扳手'),
	(1 , 'SN38', N''),
	(21, 'SN39', N''),
	(22, 'SN40', N''),
	(6 , 'SN41', N'好立扇'),
	(17, 'SN42', N'好拖把'),
	(17, 'SN43', N'好拖把'),
	(17, 'SN44', N'好拖把'),
	(17, 'SN45', N'好拖把'),
	(21, 'SN46', N''),
	(18, 'SN47', N''),
	(7 , 'SN48', N''),
	(7 , 'SN49', N''),
	(16, 'SN50', N''),
	(20, 'SN51', N'好六角扳手'),
	(12, 'SN52', N''),
	(3 , 'SN53', N''),
	(16, 'SN54', N''),
	(13, 'SN55', N''),
	(10, 'SN56', N'好直尺'),
	(10, 'SN57', N'好直尺'),
	(7 , 'SN58', N''),
	(3 , 'SN59', N''),
	(6 , 'SN60', N'好立扇'),
	(16, 'SN61', N''),
	(3 , 'SN62', N''),
	(4 , 'SN63', N''),
	(4 , 'SN64', N''),
	(16, 'SN65', N''),
	(4 , 'SN66', N''),
	(14, 'SN67', N'好槓鈴'),
	(21, 'SN68', N''),
	(15, 'SN69', N'好啞鈴'),
	(16, 'SN70', N''),
	(11, 'SN71', N''),
	(15, 'SN72', N'好啞鈴')

INSERT INTO [Condition](ConditionID, ConditionName)
VALUES
	('I', N'入庫'),
	('O', N'出庫'),
	('S', N'報廢'),
	('F', N'損壞'),
	('L', N'遺失') 

INSERT INTO [ItemLog](AdminID, ItemID, ConditionID)
VALUES
	(1, 1, 1), 
	(1, 2, 1), 
	(1, 3, 1), 
	(1, 4, 1), 
	(1, 5, 1), 
	(1, 6, 1), 
	(1, 7, 1), 
	(1, 8, 1), 
	(1, 9, 1), 
	(1, 10, 1), 
	(1, 11, 1), 
	(1, 12, 1), 
	(1, 13, 1), 
	(1, 14, 1), 
	(1, 15, 1), 
	(1, 16, 1), 
	(1, 17, 1), 
	(1, 18, 1), 
	(1, 19, 1), 
	(1, 20, 1), 
	(1, 21, 1), 
	(1, 22, 1), 
	(1, 23, 1), 
	(1, 24, 1), 
	(1, 25, 1), 
	(1, 26, 1), 
	(1, 27, 1), 
	(1, 28, 1), 
	(1, 29, 1), 
	(1, 30, 1), 
	(1, 31, 1), 
	(1, 32, 1), 
	(1, 33, 1), 
	(1, 34, 1), 
	(1, 35, 1), 
	(1, 36, 1), 
	(1, 37, 1), 
	(1, 38, 1), 
	(1, 39, 1), 
	(1, 40, 1), 
	(1, 41, 1), 
	(1, 42, 1), 
	(1, 43, 1), 
	(1, 44, 1), 
	(1, 45, 1), 
	(1, 46, 1), 
	(1, 47, 1), 
	(1, 48, 1), 
	(1, 49, 1), 
	(1, 50, 1), 
	(1, 51, 1), 
	(1, 52, 1), 
	(1, 53, 1), 
	(1, 54, 1), 
	(1, 55, 1), 
	(1, 56, 1), 
	(1, 57, 1), 
	(1, 58, 1), 
	(1, 59, 1), 
	(1, 60, 1), 
	(1, 61, 1), 
	(1, 62, 1), 
	(1, 63, 1), 
	(1, 64, 1), 
	(1, 65, 1), 
	(1, 66, 1), 
	(1, 67, 1), 
	(1, 68, 1), 
	(1, 69, 1), 
	(1, 70, 1), 
	(1, 71, 1), 
	(1, 72, 1) 

INSERT INTO [OrderStatus]([OrderStatusID], [StatusName])
VALUES
        ('P', N'處理中'),
        ('A', N'已成立'),
        ('D', N'不成立'),
        ('E', N'已結束'),
        ('C', N'已取消')

INSERT INTO [PaymentCategory]([PaymentCategoryID], [PaymentCategoryName])
VALUES
        ('P', '租賃費'),        
        ('F', '違規費')

INSERT INTO [OrderDetailStatus]([OrderDetailStatusID], [StatusName])
VALUES
        ('P', '待領取'),
        ('T', '已領取'),
        ('R', '已歸還'),
        ('L', '遺失')
