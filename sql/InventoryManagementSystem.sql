USE master

CREATE DATABASE [InventoryManagementSystem]
GO

USE [InventoryManagementSystem]
GO

CREATE TABLE [Role](
        [RoleID] INT IDENTITY(1, 1) NOT NULL
                CONSTRAINT [PK_Role] PRIMARY KEY,

        [RoleName] NVARCHAR(10) NOT NULL
);

CREATE TABLE [Admin](
        [AdminID] INT IDENTITY(1, 1) NOT NULL
                CONSTRAINT [PK_Admin] PRIMARY KEY,

        [RoleID] INT NOT NULL
                CONSTRAINT [FK_Admin_Role] FOREIGN KEY REFERENCES [Role]([RoleID]),

        [Username] NVARCHAR(50) NOT NULL
                CONSTRAINT [UQ_Admin_Username] UNIQUE,

        [Password] CHAR(64) NOT NULL,

        [FullName] NVARCHAR(50) NOT NULL,

        [CreateTime] DATETIME
                CONSTRAINT [DF_Admin_CreateTime] DEFAULT GETDATE()
);

CREATE TABLE [User](
        [UserID] INT IDENTITY(1, 1) NOT NULL
                CONSTRAINT [PK_User] PRIMARY KEY,

        [Username] VARCHAR(50) NOT NULL
                CONSTRAINT [UQ_User_Username] UNIQUE,

        [Email] VARCHAR(100) NOT NULL
                CONSTRAINT [UQ_User_Email] UNIQUE,

        [Password] CHAR(64) NOT NULL,

        [FullName] NVARCHAR(50) NOT NULL,

        [AllowNotification] BIT,

        [Address] NVARCHAR(50),

        [PhoneNumber] VARCHAR(20) NOT NULL
                CONSTRAINT [UQ_User_PhoneNumber] UNIQUE,

        [Gender] CHAR(1)
                CONSTRAINT [CK_User_Gender] CHECK(
                                                  Gender like 'M'
                                                  OR
                                                  Gender like 'F'
                                                  OR
                                                  Gender like 'X'
                                                 ),

        [DateOfBirth] DATE,

        [CreateTime] DATETIME
                CONSTRAINT [DF_User_CreateDate] DEFAULT GETDATE(),

        [ViolationTimes] INT
                CONSTRAINT [DF_User_ViolationTimes] DEFAULT 0,

        [Banned] BIT,

        [LineAccount] VARCHAR(64)
);

CREATE TABLE [EquipCategory](
        [EquipCategoryID] INT IDENTITY(1, 1) NOT NULL
                CONSTRAINT [PK_EquipCategory] PRIMARY KEY,

        [CategoryName] NVARCHAR(50) NOT NULL
);

CREATE TABLE [Equipment](
        [EquipmentID] INT IDENTITY(1, 1) NOT NULL 
                CONSTRAINT [PK_Equipment] PRIMARY KEY,

        [EquipmentCategoryID] INT NOT NULL
                CONSTRAINT [FK_Equipment_EquipCategory] FOREIGN KEY REFERENCES [EquipCategory]([EquipCategoryID]),

        [EquipmentSN] VARCHAR(50) NOT NULL
                CONSTRAINT [UQ_Equipment_EquipmentSN] UNIQUE,

        [EquipmentName] NVARCHAR(50) NOT NULL,

        [Brand] NVARCHAR(50),

        [Model] NVARCHAR(50),

        [UnitPrice] DECIMAL,

        [Description] NVARCHAR(100)
);

CREATE TABLE [Item](
        [ItemID] INT IDENTITY(1, 1) NOT NULL
                CONSTRAINT [PK_Item] PRIMARY KEY,

        [EquipmentID] INT NOT NULL
                CONSTRAINT [FK_Item_Equipment] FOREIGN KEY REFERENCES [Equipment]([EquipmentID]),

        [ConditionID] INT NOT NULL
                CONSTRAINT [FK_Item_Condition] FOREIGN KEY REFERENCES [Condition]([ConditionID]),

        [ItemSN] VARCHAR(50) NOT NULL
                CONSTRAINT [UQ_Item_ItemSN] UNIQUE,

        [Description] NVARCHAR(100)
);

CREATE TABLE [Order](
        [OrderID] INT IDENTITY(1, 1) NOT NULL 
                CONSTRAINT [PK_Order] PRIMARY KEY,

        [UserID] INT NOT NULL
                CONSTRAINT [FK_Order_User] FOREIGN KEY REFERENCES [User]([UserID]),

        [EquipmentID] INT NOT NULL 
                CONSTRAINT [FK_Order_Equipment] FOREIGN KEY REFERENCES [Equipment]([EquipmentID]),

        [Quantity] INT NOT NULL,

        [EstimatedPickupTime] DATETIME NOT NULL,

        [Day] INT NOT NULL,

        [OrderStatusID] CHAR(1) NOT NULL
                CONSTRAINT [FK_Order_Status] FOREIGN KEY REFERENCES [OrderStatus]([OrderStatusID]),

        [OrderTime] DATETIME 
                CONSTRAINT [DF_Item_OrderTime] DEFAULT GETDATE() 
);

CREATE TABLE [OrderStatus](
        [OrderStatusID] CHAR(1) NOT NULL
                CONSTRAINT [PK_OrderStatus] PRIMARY KEY,
        -- P for Pending
        -- A for Approved
        -- D for Denied
        -- E for Ended
        -- C for Canceld

    [StatusName] NVARCHAR(10) NOT NULL
        CONSTRAINT [UQ_OrderStatus_StatusName] UNIQUE
);

CREATE TABLE [CanceledOrder](
        [UserID] INT NOT NULL
                CONSTRAINT [FK_CanceledOrder_User] FOREIGN KEY REFERENCES [User]([UserID]),

        [OrderID] INT NOT NULL
                CONSTRAINT [FK_CanceledOrder_Order] FOREIGN KEY REFERENCES [Order]([OrderID])
                CONSTRAINT [UQ_CenceledOrder_OrderID] UNIQUE,

        [Description] NVARCHAR(100),

        [CancelTime] DATETIME
                CONSTRAINT [DF_Item_CancelTime] DEFAULT GETDATE(),

        CONSTRAINT [PK_CanceledOrder] PRIMARY KEY ([UserID], [OrderID])
);

CREATE TABLE [Response](
        [ResponseID] INT IDENTITY(1, 1) NOT NULL
                CONSTRAINT [PK_Response] PRIMARY KEY,

        [OrderID] INT NOT NULL
                CONSTRAINT [FK_Response_Order] FOREIGN KEY REFERENCES [Order]([OrderID]),

        [AdminID] INT NOT NULL
                CONSTRAINT [FK_Response_Admin] FOREIGN KEY REFERENCES [Admin]([AdminID]),

        [Reply] CHAR(1) NOT NULL
                CONSTRAINT [CK_Response_Reply] CHECK (
                                                      [Reply] like 'Y' -- Y for Yes
                                                      OR
                                                      [Reply] like 'N' -- N for No
                                                     ),

        [ResponseTime] DATETIME
                CONSTRAINT [DF_Response_ResponseTime] DEFAULT GETDATE()
);

CREATE TABLE [Questionnaire](
        [QuestionnaireID] INT IDENTITY(1, 1) NOT NULL
                CONSTRAINT [PK_Questionnaire] PRIMARY KEY,

        [OrderID] INT NOT NULL
                CONSTRAINT [UQ_Questionnaire_Order] FOREIGN KEY REFERENCES [Order]([OrderID]),

        [SatisfactionScore] TINYINT NOT NULL,

        [Feedback] NVARCHAR(200)
);

CREATE TABLE [PaymentCategory](
        [PaymentCategoryID] CHAR(1) NOT NULL
                CONSTRAINT [PK_PaymentCategory] PRIMARY KEY,
                -- P for Purchase
                -- F for Fine

        [PaymentCategoryName] NVARCHAR(50) NOT NULL
);

CREATE TABLE [Payment](
        [PaymentID] INT IDENTITY(1, 1) NOT NULL
                CONSTRAINT [PK_Payment] PRIMARY KEY,

        [PaymentCategoryID] CHAR(1) NOT NULL
                CONSTRAINT [FK_Payment_PaymentCategory] FOREIGN KEY REFERENCES [PaymentCategory]([PaymentCategoryID]),

        [Fee] DECIMAL NOT NULL
);

CREATE TABLE [PaymentOrder](
        [PaymentID] INT NOT NULL
                CONSTRAINT [FK_PaymentOrder_Payment] FOREIGN KEY REFERENCES [Payment]([PaymentID]),

        [OrderID] INT NOT NULL
                CONSTRAINT [FK_PaymentOrder_Order] FOREIGN KEY REFERENCES [Order]([OrderID]),

        CONSTRAINT [PK_PaymentOrder] PRIMARY KEY ([PaymentID], [OrderID])
);

CREATE TABLE [PaymentDetail](
        [PaymentDetail] INT IDENTITY(1, 1) NOT NULL
                CONSTRAINT [PK_PaymentDetail] PRIMARY KEY,

        [PaymentID] INT NOT NULL
                CONSTRAINT [FK_PaymentDetail_Payment] REFERENCES [Payment]([PaymentID]),

        [AmountPaid] DECIMAL NOT NULL,

        [PayTime] DATETIME
                CONSTRAINT [DF_PaymentDetail_PayTime] DEFAULT GETDATE()
);

CREATE TABLE [OrderDetail](
        [OrderDetailID] INT IDENTITY(1, 1) NOT NULL
                CONSTRAINT [PK_OrderDetail] PRIMARY KEY,

        [OrderID] INT NOT NULL
                CONSTRAINT [FK_OrderDetail_Order] FOREIGN KEY REFERENCES [Order]([OrderID]),

        [ItemID] INT NOT NULL
                CONSTRAINT [FK_OrderDetail_Item] FOREIGN KEY REFERENCES [Item]([ItemID])

        [OrderDetailStatusID] CHAR(1) NOT NULL
                CONSTRAINT [FK_OrderDetail_OrderDetailStatus] [OrderDetailStatus]([OrderDetailStatusID])
);

CREATE TABLE [OrderDetailStatus](
        [OrderDetailStatusID] CHAR(1) NOT NULL
                CONSTRAINT [PK_OrderDetailStatus] PRIMARY KEY,
        -- P for Pending
        -- T for Taken
        -- R for Returned
        -- L for Lost

        [StatusName] NVARCHAR(10) NOT NULL
                CONSTRAINT [UQ_OrderDetailStatus_StatusName] UNIQUE
);

CREATE TABLE [Report](
        [ReportID] INT IDENTITY(1, 1) NOT NULL
                CONSTRAINT [PK_Report] PRIMARY KEY,

        [OrderDetailID] INT NOT NULL
                CONSTRAINT [FK_Report_OrderDetail] FOREIGN KEY REFERENCES [OrderDetail]([OrderDetailID]),

        [Description] NVARCHAR(100) NOT NULL,

        [ReportTime] DATETIME
                CONSTRAINT [DF_Report_ReportTime] DEFAULT GETDATE(),

        [CloseTime] DATETIME
);

CREATE TABLE [Condition](
        [ConditionID] CHAR(1) NOT NULL
                CONSTRAINT [PK_Condition] PRIMARY KEY,
                --I for StockIn
                --O for StockOut
                --S for Scrap
                --F for Failure
                --L for Lost

        [ConditionName] NVARCHAR(10) NOT NULL
);

CREATE TABLE [ItemLog](
        [ItemLogID] INT IDENTITY(1, 1) NOT NULL
                CONSTRAINT [PK_ItemLog] PRIMARY KEY,

        [OrderDetailID] INT NULL
                CONSTRAINT [FK_ItemLog_OrderDetail] FOREIGN KEY REFERENCES [OrderDetail]([OrderDetailID]),

        [AdminID] INT NOT NULL
                CONSTRAINT [FK_ItemLog_Admin] FOREIGN KEY REFERENCES [Admin]([AdminID]),

        [ItemID] INT NOT NULL
                CONSTRAINT [FK_ItemLog_Item] FOREIGN KEY REFERENCES [Item]([ItemID]),

        [ConditionID] CHAR(1) NOT NULL
                CONSTRAINT [FK_ItemLog_Condition] FOREIGN KEY REFERENCES [Condition]([ConditionID]),

        [Description] NVARCHAR(100),

        [CreateTime] DATETIME
                CONSTRAINT [DF_ItemLog_CreateTime] DEFAULT GETDATE()
);

