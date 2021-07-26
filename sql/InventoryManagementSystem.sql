USE master

CREATE DATABASE [InventoryManagementSystem]
GO

USE [InventoryManagementSystem]
GO

CREATE TABLE [Role](
        [RoleID] UNIQUEIDENTIFIER NOT NULL
                CONSTRAINT [PK_Role] PRIMARY KEY NONCLUSTERED,

        [RoleName] NVARCHAR(10)
);

CREATE UNIQUE NONCLUSTERED INDEX [UQ_Role_RoleName]
ON [Role]([RoleName])
WHERE [RoleName] IS NOT NULL

CREATE TABLE [Admin](
        [AdminID] UNIQUEIDENTIFIER NOT NULL
                CONSTRAINT [PK_Admin] PRIMARY KEY NONCLUSTERED,

        [RoleID] UNIQUEIDENTIFIER NOT NULL
                CONSTRAINT [FK_Admin_Role] FOREIGN KEY REFERENCES [Role]([RoleID]),

        [Username] NVARCHAR(50),

        [HashedPassword] BINARY(32) NOT NULL,

        [Salt] BINARY(32) NOT NULL,

        [FullName] NVARCHAR(50) NOT NULL,

        [CreateTime] DATETIME
                CONSTRAINT [DF_Admin_CreateTime] DEFAULT GETDATE()
);

CREATE UNIQUE NONCLUSTERED INDEX [UQ_Admin_Username]
ON [Admin]([Username])
WHERE [Username] IS NOT NULL

CREATE TABLE [User](
        [UserID] UNIQUEIDENTIFIER NOT NULL
                CONSTRAINT [PK_User] PRIMARY KEY NONCLUSTERED,

        [UserSN] INT IDENTITY(1, 1) NOT NULL
                CONSTRAINT [UQ_User_UserSN] UNIQUE,

        [Username] VARCHAR(50),

        [Email] VARCHAR(100),

        [HashedPassword] BINARY(32) NOT NULL,

        [Salt] BINARY(32) NOT NULL,

        [FullName] NVARCHAR(50) NOT NULL,

        [AllowNotification] BIT,

        [Address] NVARCHAR(50),

        [PhoneNumber] VARCHAR(20),

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

        [LineAccount] VARCHAR(64),

        [Deleted] BIT
                CONSTRAINT [DF_User_Deleted] DEFAULT 0
);

CREATE UNIQUE NONCLUSTERED INDEX [UQ_User_Username]
ON [User]([Username])
WHERE [Username] IS NOT NULL

CREATE UNIQUE NONCLUSTERED INDEX [UQ_User_Email]
ON [User]([Email])
WHERE [Email] IS NOT NULL

CREATE UNIQUE NONCLUSTERED INDEX [UQ_User_PhoneNumber]
ON [User]([PhoneNumber])
WHERE [PhoneNumber] IS NOT NULL

CREATE TABLE [EquipCategory](
        [EquipCategoryID] UNIQUEIDENTIFIER NOT NULL
                CONSTRAINT [PK_EquipCategory] PRIMARY KEY NONCLUSTERED,

        [CategoryName] NVARCHAR(50)
);

CREATE UNIQUE NONCLUSTERED INDEX [UQ_EquipCategory_CategoryName]
ON [EquipCategory]([CategoryName])
WHERE [CategoryName] IS NOT NULL

CREATE TABLE [Equipment](
        [EquipmentID] UNIQUEIDENTIFIER NOT NULL
                CONSTRAINT [PK_Equipment] PRIMARY KEY NONCLUSTERED,

        [EquipmentCategoryID] UNIQUEIDENTIFIER NOT NULL
                CONSTRAINT [FK_Equipment_EquipCategory] FOREIGN KEY REFERENCES [EquipCategory]([EquipCategoryID]),

        [EquipmentSN] VARCHAR(50),

        [EquipmentName] NVARCHAR(50) NOT NULL,

        [Brand] NVARCHAR(50),

        [Model] NVARCHAR(50),

        [UnitPrice] DECIMAL NOT NULL,

        [Description] NVARCHAR(100)
);

CREATE UNIQUE NONCLUSTERED INDEX [UQ_Equipment_EquipmentSN]
ON [Equipment]([EquipmentSN])
WHERE [EquipmentSN] IS NOT NULL

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

CREATE TABLE [Item](
        [ItemID] UNIQUEIDENTIFIER NOT NULL
                CONSTRAINT [PK_Item] PRIMARY KEY NONCLUSTERED,

        [EquipmentID] UNIQUEIDENTIFIER NOT NULL
                CONSTRAINT [FK_Item_Equipment] FOREIGN KEY REFERENCES [Equipment]([EquipmentID]),

        [ConditionID] CHAR(1) NOT NULL
                CONSTRAINT [FK_Item_Condition] FOREIGN KEY REFERENCES [Condition]([ConditionID]),

        [ItemSN] VARCHAR(50),

        [Description] NVARCHAR(100)
);

CREATE UNIQUE NONCLUSTERED INDEX [UQ_Item_ItemSN]
ON [Item]([ItemSN])
WHERE [ItemSN] IS NOT NULL

CREATE TABLE [OrderStatus](
        [OrderStatusID] CHAR(1) NOT NULL
                CONSTRAINT [PK_OrderStatus] PRIMARY KEY,
        -- P for Pending
        -- A for Approved
        -- D for Denied
        -- E for Ended
        -- C for Canceled

    [StatusName] NVARCHAR(10) NOT NULL
        CONSTRAINT [UQ_OrderStatus_StatusName] UNIQUE
);

CREATE TABLE [Order](
        [OrderID] UNIQUEIDENTIFIER NOT NULL 
                CONSTRAINT [PK_Order] PRIMARY KEY NONCLUSTERED,

        [OrderSN] INT IDENTITY(1, 1) NOT NULL
                CONSTRAINT [UQ_Order_OrderSN] UNIQUE,

        [UserID] UNIQUEIDENTIFIER NOT NULL
                CONSTRAINT [FK_Order_User] FOREIGN KEY REFERENCES [User]([UserID]),

        [EquipmentID] UNIQUEIDENTIFIER NOT NULL 
                CONSTRAINT [FK_Order_Equipment] FOREIGN KEY REFERENCES [Equipment]([EquipmentID]),

        [Quantity] INT NOT NULL,

        [EstimatedPickupTime] DATETIME NOT NULL,

        [Day] INT NOT NULL,

        [OrderStatusID] CHAR(1) NOT NULL
                CONSTRAINT [DF_Order_OrderStatusID] DEFAULT 'P'
                CONSTRAINT [FK_Order_OrderStatus] FOREIGN KEY REFERENCES [OrderStatus]([OrderStatusID]),

        [OrderTime] DATETIME 
                CONSTRAINT [DF_Item_OrderTime] DEFAULT GETDATE() 
);

CREATE TABLE [CanceledOrder](
        [UserID] UNIQUEIDENTIFIER NOT NULL
                CONSTRAINT [FK_CanceledOrder_User] FOREIGN KEY REFERENCES [User]([UserID]),

        [OrderID] UNIQUEIDENTIFIER NOT NULL
                CONSTRAINT [FK_CanceledOrder_Order] FOREIGN KEY REFERENCES [Order]([OrderID])
                CONSTRAINT [UQ_CenceledOrder_OrderID] UNIQUE,

        [Description] NVARCHAR(100),

        [CancelTime] DATETIME
                CONSTRAINT [DF_Item_CancelTime] DEFAULT GETDATE(),

        CONSTRAINT [PK_CanceledOrder] PRIMARY KEY ([UserID], [OrderID])
);

CREATE TABLE [Response](
        [ResponseID] UNIQUEIDENTIFIER NOT NULL
                CONSTRAINT [PK_Response] PRIMARY KEY NONCLUSTERED,

        [OrderID] UNIQUEIDENTIFIER NOT NULL
                CONSTRAINT [FK_Response_Order] FOREIGN KEY REFERENCES [Order]([OrderID]),

        [AdminID] UNIQUEIDENTIFIER NOT NULL
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
        [QuestionnaireID] UNIQUEIDENTIFIER NOT NULL
                CONSTRAINT [PK_Questionnaire] PRIMARY KEY NONCLUSTERED,

        [OrderID] UNIQUEIDENTIFIER NOT NULL
                CONSTRAINT [UQ_Questionnaire_Order] FOREIGN KEY REFERENCES [Order]([OrderID]),

        [SatisfactionScore] TINYINT NOT NULL,

        [Feedback] NVARCHAR(200)
);

CREATE TABLE [FeeCategory](
        [FeeCategoryID] CHAR(1) NOT NULL
                CONSTRAINT [PK_FeeCategory] PRIMARY KEY,
                -- R for RentalFee
                -- E for ExtraFee

        [Name] NVARCHAR(50) NOT NULL
);

CREATE TABLE [Payment](
        [PaymentID] UNIQUEIDENTIFIER NOT NULL
                CONSTRAINT [PK_Payment] PRIMARY KEY NONCLUSTERED,

        [PaymentSN] VARCHAR(16) NOT NULL
                CONSTRAINT [UQ_Payment_PaymentSN] UNIQUE,

        [RentalFee] DECIMAL NOT NULL,

        [ExtraFee] DECIMAL
);

CREATE TABLE [PaymentLog](
        [PaymentLogID] UNIQUEIDENTIFIER NOT NULL
                CONSTRAINT [PK_PaymentLog] PRIMARY KEY NONCLUSTERED,

        [FeeCategoryID] CHAR(1) NOT NULL
                CONSTRAINT [FK_PaymentLog_FeeCategory] FOREIGN KEY REFERENCES [FeeCategory]([FeeCategoryID]),

        [PaymentID] UNIQUEIDENTIFIER NOT NULL
                CONSTRAINT [FK_PaymentLog_Payment] FOREIGN KEY REFERENCES [Payment]([PaymentID]),

        [Fee] DECIMAL NOT NULL,

        [Description] NVARCHAR(200)
);

CREATE TABLE [PaymentOrder](
        [PaymentID] UNIQUEIDENTIFIER NOT NULL
                CONSTRAINT [FK_PaymentOrder_Payment] FOREIGN KEY REFERENCES [Payment]([PaymentID]),

        [OrderID] UNIQUEIDENTIFIER NOT NULL
                CONSTRAINT [FK_PaymentOrder_Order] FOREIGN KEY REFERENCES [Order]([OrderID])
                CONSTRAINT [UQ_PaymentOrder_OrderID] UNIQUE,

        CONSTRAINT [PK_PaymentOrder] PRIMARY KEY ([PaymentID], [OrderID])
);

CREATE TABLE [PaymentDetail](
        [PaymentDetailID] UNIQUEIDENTIFIER NOT NULL
                CONSTRAINT [PK_PaymentDetail] PRIMARY KEY NONCLUSTERED,

        [PaymentDetailSN] CHAR(18) NOT NULL
                CONSTRAINT [UQ_PaymentDetail_PaymentDetailSN] UNIQUE,

        [PaymentID] UNIQUEIDENTIFIER NOT NULL
                CONSTRAINT [FK_PaymentDetail_Payment] REFERENCES [Payment]([PaymentID]),

        [AmountPaid] DECIMAL NOT NULL,

        [TradeNo] VARCHAR(20) NOT NULL,

        [IP] VARCHAR(15) NOT NULL,

        [PayTime] DATETIME
                CONSTRAINT [DF_PaymentDetail_PayTime] DEFAULT GETDATE()
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

CREATE TABLE [OrderDetail](
        [OrderDetailID] UNIQUEIDENTIFIER NOT NULL
                CONSTRAINT [PK_OrderDetail] PRIMARY KEY NONCLUSTERED,

        [OrderDetailSN] INT IDENTITY(1, 1) NOT NULL
                CONSTRAINT [UQ_OrderDetail_OrderDetailSN] UNIQUE,

        [OrderID] UNIQUEIDENTIFIER NOT NULL
                CONSTRAINT [FK_OrderDetail_Order] FOREIGN KEY REFERENCES [Order]([OrderID]),

        [ItemID] UNIQUEIDENTIFIER NOT NULL
                CONSTRAINT [FK_OrderDetail_Item] FOREIGN KEY REFERENCES [Item]([ItemID]),

        [OrderDetailStatusID] CHAR(1) NOT NULL
                CONSTRAINT [FK_OrderDetail_OrderDetailStatus] FOREIGN KEY REFERENCES [OrderDetailStatus]([OrderDetailStatusID])
);

CREATE TABLE [Report](
        [ReportID] UNIQUEIDENTIFIER NOT NULL
                CONSTRAINT [PK_Report] PRIMARY KEY NONCLUSTERED,

        [OrderDetailID] UNIQUEIDENTIFIER NOT NULL
                CONSTRAINT [FK_Report_OrderDetail] FOREIGN KEY REFERENCES [OrderDetail]([OrderDetailID]),

        [Description] NVARCHAR(100) NOT NULL,

        [ReportTime] DATETIME
                CONSTRAINT [DF_Report_ReportTime] DEFAULT GETDATE(),

        [CloseTime] DATETIME
);

CREATE TABLE [ItemLog](
        [ItemLogID] UNIQUEIDENTIFIER NOT NULL
                CONSTRAINT [PK_ItemLog] PRIMARY KEY NONCLUSTERED,

        [OrderDetailID] UNIQUEIDENTIFIER NULL
                CONSTRAINT [FK_ItemLog_OrderDetail] FOREIGN KEY REFERENCES [OrderDetail]([OrderDetailID]),

        [AdminID] UNIQUEIDENTIFIER NULL
                CONSTRAINT [FK_ItemLog_Admin] FOREIGN KEY REFERENCES [Admin]([AdminID]),

        [ItemID] UNIQUEIDENTIFIER NOT NULL
                CONSTRAINT [FK_ItemLog_Item] FOREIGN KEY REFERENCES [Item]([ItemID]),

        [ConditionID] CHAR(1) NOT NULL
                CONSTRAINT [FK_ItemLog_Condition] FOREIGN KEY REFERENCES [Condition]([ConditionID]),

        [Description] NVARCHAR(100),

        [CreateTime] DATETIME
                CONSTRAINT [DF_ItemLog_CreateTime] DEFAULT GETDATE()
);

CREATE TABLE [LineNotification](
        [LineNotificationID] UNIQUEIDENTIFIER NOT NULL
                CONSTRAINT [PK_LineNotification] PRIMARY KEY NONCLUSTERED,

        [OrderDetailID] UNIQUEIDENTIFIER NOT NULL
                CONSTRAINT [FK_LineNotification_OrderDetail] FOREIGN KEY REFERENCES [OrderDetail]([OrderDetailID]),

        [CreateTime] DATETIME
                CONSTRAINT [DF_LineNotification_CreateTime] DEFAULT GETDATE(),

        [Message] NVARCHAR(200) NOT NULL
);

CREATE TABLE [NewPayingAttempt](
        [PaymentDetailSN] CHAR(18) NOT NULL,

        [OrderSN] INT NOT NULL,

        CONSTRAINT [PK_NewPayingAttempt] PRIMARY KEY NONCLUSTERED (PaymentDetailSN, OrderSN)
);

CREATE TABLE [PayingAttempt](
        [PaymentDetailSN] CHAR(18) NOT NULL,

        [PaymentID] UNIQUEIDENTIFIER NOT NULL,

        CONSTRAINT [PK_PayingAttempt] PRIMARY KEY NONCLUSTERED (PaymentDetailSN, PaymentID)
);
