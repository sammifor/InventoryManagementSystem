using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace InventoryManagementSystem.Models.EF
{
    public partial class InventoryManagementSystemContext : DbContext
    {
        public InventoryManagementSystemContext()
        {
        }

        public InventoryManagementSystemContext(DbContextOptions<InventoryManagementSystemContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<CanceledOrder> CanceledOrders { get; set; }
        public virtual DbSet<Condition> Conditions { get; set; }
        public virtual DbSet<EquipCategory> EquipCategories { get; set; }
        public virtual DbSet<Equipment> Equipment { get; set; }
        public virtual DbSet<ExtraFee> ExtraFees { get; set; }
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<ItemLog> ItemLogs { get; set; }
        public virtual DbSet<NewPayingAttempt> NewPayingAttempts { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<OrderDetailStatus> OrderDetailStatuses { get; set; }
        public virtual DbSet<OrderStatus> OrderStatuses { get; set; }
        public virtual DbSet<PayingAttempt> PayingAttempts { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<PaymentDetail> PaymentDetails { get; set; }
        public virtual DbSet<PaymentOrder> PaymentOrders { get; set; }
        public virtual DbSet<Questionnaire> Questionnaires { get; set; }
        public virtual DbSet<QuestionnaireToken> QuestionnaireTokens { get; set; }
        public virtual DbSet<Report> Reports { get; set; }
        public virtual DbSet<ResetPasswordToken> ResetPasswordTokens { get; set; }
        public virtual DbSet<Response> Responses { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Name=InventoryDB");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Chinese_Taiwan_Stroke_CI_AS");

            modelBuilder.Entity<Admin>(entity =>
            {
                entity.HasKey(e => e.AdminId)
                    .IsClustered(false);

                entity.ToTable("Admin");

                entity.HasIndex(e => e.AdminSn, "UQ_Admin_AdminSN")
                    .IsUnique()
                    .IsClustered();

                entity.HasIndex(e => e.Username, "UQ_Admin_Username")
                    .IsUnique()
                    .HasFilter("([Username] IS NOT NULL)");

                entity.Property(e => e.AdminId)
                    .ValueGeneratedNever()
                    .HasColumnName("AdminID")
                    .HasComment("管理員編號，資料庫內部唯一識別用");

                entity.Property(e => e.AdminSn)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("AdminSN")
                    .HasComment("管理員流水號，應用程式識別用");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("新增時間");

                entity.Property(e => e.Deleted).HasComment("管理員是否被刪除");

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasComment("管理員姓名");

                entity.Property(e => e.HashedPassword)
                    .IsRequired()
                    .HasMaxLength(64)
                    .IsFixedLength(true)
                    .HasComment("密碼經過 PBKDF2 而得的雜湊碼");

                entity.Property(e => e.RoleId)
                    .HasColumnName("RoleID")
                    .HasComment("管理員角色");

                entity.Property(e => e.Salt)
                    .IsRequired()
                    .HasMaxLength(64)
                    .IsFixedLength(true)
                    .HasComment("用於密碼加鹽");

                entity.Property(e => e.Username)
                    .HasMaxLength(50)
                    .HasComment("管理員帳號");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Admins)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Admin_Role");
            });

            modelBuilder.Entity<CanceledOrder>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.OrderId });

                entity.ToTable("CanceledOrder");

                entity.HasIndex(e => e.OrderId, "UQ_CenceledOrder_OrderID")
                    .IsUnique();

                entity.Property(e => e.UserId)
                    .HasColumnName("UserID")
                    .HasComment("取消訂單所屬使用者");

                entity.Property(e => e.OrderId)
                    .HasColumnName("OrderID")
                    .HasComment("使用者所取消的訂單");

                entity.Property(e => e.CancelTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("取消訂單的時間");

                entity.Property(e => e.Description)
                    .HasMaxLength(100)
                    .HasComment("取消訂單的原因");

                entity.HasOne(d => d.Order)
                    .WithOne(p => p.CanceledOrder)
                    .HasForeignKey<CanceledOrder>(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CanceledOrder_Order");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.CanceledOrders)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CanceledOrder_User");
            });

            modelBuilder.Entity<Condition>(entity =>
            {
                entity.ToTable("Condition");

                entity.Property(e => e.ConditionId)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("ConditionID")
                    .IsFixedLength(true)
                    .HasComment("物品（Item）的狀態代號");

                entity.Property(e => e.ConditionName)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasComment("物品（Item）的狀態中文名稱");
            });

            modelBuilder.Entity<EquipCategory>(entity =>
            {
                entity.HasKey(e => e.EquipCategoryId)
                    .IsClustered(false);

                entity.ToTable("EquipCategory");

                entity.HasIndex(e => e.CategoryName, "UQ_EquipCategory_CategoryName")
                    .IsUnique()
                    .HasFilter("([CategoryName] IS NOT NULL)");

                entity.Property(e => e.EquipCategoryId)
                    .ValueGeneratedNever()
                    .HasColumnName("EquipCategoryID")
                    .HasComment("設備（Equipment）分類的編號，資料庫內部用");

                entity.Property(e => e.CategoryName)
                    .HasMaxLength(50)
                    .HasComment("設備（Equipment）分類的中文名稱");
            });

            modelBuilder.Entity<Equipment>(entity =>
            {
                entity.HasKey(e => e.EquipmentId)
                    .IsClustered(false);

                entity.HasIndex(e => e.EquipmentSn, "UQ_Equipment_EquipmentSN")
                    .IsUnique()
                    .HasFilter("([EquipmentSN] IS NOT NULL)");

                entity.Property(e => e.EquipmentId)
                    .ValueGeneratedNever()
                    .HasColumnName("EquipmentID")
                    .HasComment("設備的編號，資料庫內部唯一識別用");

                entity.Property(e => e.Brand)
                    .HasMaxLength(50)
                    .HasComment("品牌名稱");

                entity.Property(e => e.Deleted).HasComment("設備是否被刪除");

                entity.Property(e => e.Description)
                    .HasMaxLength(100)
                    .HasComment("設備描述");

                entity.Property(e => e.EquipmentCategoryId)
                    .HasColumnName("EquipmentCategoryID")
                    .HasComment("設備分類");

                entity.Property(e => e.EquipmentName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasComment("設備名稱");

                entity.Property(e => e.EquipmentSn)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("EquipmentSN")
                    .HasComment("設備的流水號，應用程式唯一識別用");

                entity.Property(e => e.Model)
                    .HasMaxLength(50)
                    .HasComment("型號名稱");

                entity.Property(e => e.UnitPrice)
                    .HasColumnType("decimal(18, 0)")
                    .HasComment("單價");

                entity.HasOne(d => d.EquipmentCategory)
                    .WithMany(p => p.Equipment)
                    .HasForeignKey(d => d.EquipmentCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Equipment_EquipCategory");
            });

            modelBuilder.Entity<ExtraFee>(entity =>
            {
                entity.HasKey(e => e.ExtraFeeId)
                    .IsClustered(false);

                entity.ToTable("ExtraFee");

                entity.HasIndex(e => e.ExtraFeeSn, "UQ_ExtraFee_ExtraFeeSN")
                    .IsUnique()
                    .IsClustered();

                entity.Property(e => e.ExtraFeeId)
                    .ValueGeneratedNever()
                    .HasColumnName("ExtraFeeID")
                    .HasComment("額外費用編號，資料庫內部唯一識別用");

                entity.Property(e => e.Description)
                    .HasMaxLength(100)
                    .HasComment("費用說明、原因");

                entity.Property(e => e.ExtraFeeSn)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ExtraFeeSN")
                    .HasComment("額外費用流水號，應用程式唯一識別用");

                entity.Property(e => e.Fee)
                    .HasColumnType("decimal(18, 0)")
                    .HasComment("額外費用");

                entity.Property(e => e.OrderDetailId)
                    .HasColumnName("OrderDetailID")
                    .HasComment("此筆額外費用所屬的訂單明細（Order Detail）");

                entity.HasOne(d => d.OrderDetail)
                    .WithMany(p => p.ExtraFees)
                    .HasForeignKey(d => d.OrderDetailId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ExtraFee_OrderDetail");
            });

            modelBuilder.Entity<Item>(entity =>
            {
                entity.HasKey(e => e.ItemId)
                    .IsClustered(false);

                entity.ToTable("Item");

                entity.HasIndex(e => e.ItemSn, "UQ_Item_ItemSN")
                    .IsUnique()
                    .HasFilter("([ItemSN] IS NOT NULL)");

                entity.Property(e => e.ItemId)
                    .ValueGeneratedNever()
                    .HasColumnName("ItemID")
                    .HasComment("物品編號，資料庫內部唯一識別用");

                entity.Property(e => e.ConditionId)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("ConditionID")
                    .IsFixedLength(true)
                    .HasComment("此筆物品目前狀態");

                entity.Property(e => e.Description)
                    .HasMaxLength(100)
                    .HasComment("個別物品描述");

                entity.Property(e => e.EquipmentId)
                    .HasColumnName("EquipmentID")
                    .HasComment("此筆物品所屬設備（Equipment）編號");

                entity.Property(e => e.ItemSn)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("ItemSN")
                    .HasComment("物品流水號，應用程式唯一識別用");

                entity.HasOne(d => d.Condition)
                    .WithMany(p => p.Items)
                    .HasForeignKey(d => d.ConditionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Item_Condition");

                entity.HasOne(d => d.Equipment)
                    .WithMany(p => p.Items)
                    .HasForeignKey(d => d.EquipmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Item_Equipment");
            });

            modelBuilder.Entity<ItemLog>(entity =>
            {
                entity.HasKey(e => e.ItemLogId)
                    .IsClustered(false);

                entity.ToTable("ItemLog");

                entity.HasIndex(e => e.ItemLogSn, "UQ_ItemLog_ItemLogSN")
                    .IsUnique()
                    .IsClustered();

                entity.Property(e => e.ItemLogId)
                    .ValueGeneratedNever()
                    .HasColumnName("ItemLogID")
                    .HasComment("物品記錄編號，資料庫內部唯一識別用");

                entity.Property(e => e.AdminId)
                    .HasColumnName("AdminID")
                    .HasComment("此筆記錄是哪位管理員（Admin）所登記。若為空則非管理員所致。");

                entity.Property(e => e.ConditionId)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("ConditionID")
                    .IsFixedLength(true)
                    .HasComment("物品當下被記錄的狀態");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("物品記錄產生時間");

                entity.Property(e => e.Description)
                    .HasMaxLength(100)
                    .HasComment("物品當下的描述");

                entity.Property(e => e.ItemId)
                    .HasColumnName("ItemID")
                    .HasComment("此筆記錄所屬物品（Item）");

                entity.Property(e => e.ItemLogSn)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ItemLogSN")
                    .HasComment("物品記錄流水號，應用程式唯一識別用");

                entity.Property(e => e.OrderDetailId)
                    .HasColumnName("OrderDetailID")
                    .HasComment("此筆記錄在哪筆訂單明細（Order Detail）產生。若空則非訂單明細所致。");

                entity.HasOne(d => d.Admin)
                    .WithMany(p => p.ItemLogs)
                    .HasForeignKey(d => d.AdminId)
                    .HasConstraintName("FK_ItemLog_Admin");

                entity.HasOne(d => d.Condition)
                    .WithMany(p => p.ItemLogs)
                    .HasForeignKey(d => d.ConditionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ItemLog_Condition");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.ItemLogs)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ItemLog_Item");

                entity.HasOne(d => d.OrderDetail)
                    .WithMany(p => p.ItemLogs)
                    .HasForeignKey(d => d.OrderDetailId)
                    .HasConstraintName("FK_ItemLog_OrderDetail");
            });

            modelBuilder.Entity<NewPayingAttempt>(entity =>
            {
                entity.HasKey(e => new { e.PaymentDetailSn, e.OrderSn })
                    .IsClustered(false);

                entity.ToTable("NewPayingAttempt");

                entity.Property(e => e.PaymentDetailSn)
                    .HasMaxLength(18)
                    .IsUnicode(false)
                    .HasColumnName("PaymentDetailSN")
                    .IsFixedLength(true)
                    .HasComment("第一次付款（Payment）時，若付款成功則付款明細（Payment Detail）流水號會有的值。");

                entity.Property(e => e.OrderSn)
                    .HasColumnName("OrderSN")
                    .HasComment("第一次付款預備要付的訂單（Order）");

                entity.HasOne(d => d.OrderSnNavigation)
                    .WithMany(p => p.NewPayingAttempts)
                    .HasPrincipalKey(p => p.OrderSn)
                    .HasForeignKey(d => d.OrderSn)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NewPayingAttempt_Order");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.NotificationId)
                    .IsClustered(false);

                entity.ToTable("Notification");

                entity.Property(e => e.NotificationId)
                    .ValueGeneratedNever()
                    .HasColumnName("NotificationID")
                    .HasComment("通知記錄編號，資料庫內部唯一識別用。");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("此訊息傳送時間");

                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasComment("訊息內容");

                entity.Property(e => e.UserId)
                    .HasColumnName("UserID")
                    .HasComment("此通知所傳給的使用者（User）");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Notification_User");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.OrderId)
                    .IsClustered(false);

                entity.ToTable("Order");

                entity.HasIndex(e => e.OrderSn, "UQ_Order_OrderSN")
                    .IsUnique();

                entity.Property(e => e.OrderId)
                    .ValueGeneratedNever()
                    .HasColumnName("OrderID")
                    .HasComment("訂單編號，資料庫內部唯一識別用");

                entity.Property(e => e.Day).HasComment("租借天數");

                entity.Property(e => e.EquipmentId)
                    .HasColumnName("EquipmentID")
                    .HasComment("此筆訂單所要租借的設備（Equipment）");

                entity.Property(e => e.EstimatedPickupTime)
                    .HasColumnType("datetime")
                    .HasComment("預計取貨時間");

                entity.Property(e => e.OrderSn)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("OrderSN")
                    .HasComment("訂單流水號，應用程式唯一識別用");

                entity.Property(e => e.OrderStatusId)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("OrderStatusID")
                    .HasDefaultValueSql("('P')")
                    .IsFixedLength(true)
                    .HasComment("訂單狀態代號");

                entity.Property(e => e.OrderTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("下訂單的時間");

                entity.Property(e => e.Quantity).HasComment("租借的數量");

                entity.Property(e => e.UserId)
                    .HasColumnName("UserID")
                    .HasComment("下此筆訂單的使用者（User）");

                entity.HasOne(d => d.Equipment)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.EquipmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_Equipment");

                entity.HasOne(d => d.OrderStatus)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.OrderStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_OrderStatus");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_User");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(e => e.OrderDetailId)
                    .IsClustered(false);

                entity.ToTable("OrderDetail");

                entity.HasIndex(e => e.OrderDetailSn, "UQ_OrderDetail_OrderDetailSN")
                    .IsUnique();

                entity.Property(e => e.OrderDetailId)
                    .ValueGeneratedNever()
                    .HasColumnName("OrderDetailID")
                    .HasComment("訂單明細編號，資料庫內部唯一識別用");

                entity.Property(e => e.ItemId)
                    .HasColumnName("ItemID")
                    .HasComment("訂單明細所對應的物品（Item）");

                entity.Property(e => e.OrderDetailSn)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("OrderDetailSN")
                    .HasComment("訂單明細流水號，應用程式唯一識別用");

                entity.Property(e => e.OrderDetailStatusId)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("OrderDetailStatusID")
                    .IsFixedLength(true)
                    .HasComment("訂單明細的狀態代號");

                entity.Property(e => e.OrderId)
                    .HasColumnName("OrderID")
                    .HasComment("訂單明細所屬訂單（Order）");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderDetail_Item");

                entity.HasOne(d => d.OrderDetailStatus)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderDetailStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderDetail_OrderDetailStatus");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderDetail_Order");
            });

            modelBuilder.Entity<OrderDetailStatus>(entity =>
            {
                entity.ToTable("OrderDetailStatus");

                entity.HasIndex(e => e.StatusName, "UQ_OrderDetailStatus_StatusName")
                    .IsUnique();

                entity.Property(e => e.OrderDetailStatusId)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("OrderDetailStatusID")
                    .IsFixedLength(true)
                    .HasComment("訂單明細的狀態代號");

                entity.Property(e => e.StatusName)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasComment("訂單明細的狀態名稱");
            });

            modelBuilder.Entity<OrderStatus>(entity =>
            {
                entity.ToTable("OrderStatus");

                entity.HasIndex(e => e.StatusName, "UQ_OrderStatus_StatusName")
                    .IsUnique();

                entity.Property(e => e.OrderStatusId)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("OrderStatusID")
                    .IsFixedLength(true)
                    .HasComment("訂單狀態代號");

                entity.Property(e => e.StatusName)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasComment("訂單狀態名稱");
            });

            modelBuilder.Entity<PayingAttempt>(entity =>
            {
                entity.HasKey(e => new { e.PaymentDetailSn, e.PaymentId })
                    .IsClustered(false);

                entity.ToTable("PayingAttempt");

                entity.Property(e => e.PaymentDetailSn)
                    .HasMaxLength(18)
                    .IsUnicode(false)
                    .HasColumnName("PaymentDetailSN")
                    .IsFixedLength(true)
                    .HasComment("非首次付款（Payment）時，若付款成功則付款明細（Payment Detail）流水號會有的值。");

                entity.Property(e => e.PaymentId)
                    .HasColumnName("PaymentID")
                    .HasComment("非首次付款預備要結清的付款（Payment）");

                entity.HasOne(d => d.Payment)
                    .WithMany(p => p.PayingAttempts)
                    .HasForeignKey(d => d.PaymentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PayingAttempt_Payment");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.PaymentId)
                    .IsClustered(false);

                entity.ToTable("Payment");

                entity.HasIndex(e => e.PaymentSn, "UQ_Payment_PaymentSN")
                    .IsUnique();

                entity.Property(e => e.PaymentId)
                    .ValueGeneratedNever()
                    .HasColumnName("PaymentID")
                    .HasComment("付款的編號，資料庫內部唯一識別用");

                entity.Property(e => e.PaymentSn)
                    .IsRequired()
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("PaymentSN")
                    .HasComment("付款的有規則編號，應用程式唯一識別用");

                entity.Property(e => e.RentalFee)
                    .HasColumnType("decimal(18, 0)")
                    .HasComment("付款總額");
            });

            modelBuilder.Entity<PaymentDetail>(entity =>
            {
                entity.HasKey(e => e.PaymentDetailId)
                    .IsClustered(false);

                entity.ToTable("PaymentDetail");

                entity.HasIndex(e => e.PaymentDetailSn, "UQ_PaymentDetail_PaymentDetailSN")
                    .IsUnique();

                entity.Property(e => e.PaymentDetailId)
                    .ValueGeneratedNever()
                    .HasColumnName("PaymentDetailID")
                    .HasComment("付款明細編號，資料庫內部唯一識別用");

                entity.Property(e => e.AmountPaid)
                    .HasColumnType("decimal(18, 0)")
                    .HasComment("此次所付總額");

                entity.Property(e => e.Ip)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("IP")
                    .HasComment("付款人交易時的 IP");

                entity.Property(e => e.PayTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("支付完成時間 ");

                entity.Property(e => e.PaymentDetailSn)
                    .IsRequired()
                    .HasMaxLength(18)
                    .IsUnicode(false)
                    .HasColumnName("PaymentDetailSN")
                    .IsFixedLength(true)
                    .HasComment("付款明細的有規則編號，應用程式唯一識別用，於藍新金流用於 MerchantOrderNo");

                entity.Property(e => e.PaymentId)
                    .HasColumnName("PaymentID")
                    .HasComment("此付款明細所屬付款（Payment）");

                entity.Property(e => e.TradeNo)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasComment("藍新金流在此筆交易成功時所產生的序號");

                entity.HasOne(d => d.Payment)
                    .WithMany(p => p.PaymentDetails)
                    .HasForeignKey(d => d.PaymentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PaymentDetail_Payment");
            });

            modelBuilder.Entity<PaymentOrder>(entity =>
            {
                entity.HasKey(e => new { e.PaymentId, e.OrderId });

                entity.ToTable("PaymentOrder");

                entity.HasIndex(e => e.OrderId, "UQ_PaymentOrder_OrderID")
                    .IsUnique();

                entity.Property(e => e.PaymentId)
                    .HasColumnName("PaymentID")
                    .HasComment("關聯用，付款（Payment）編號");

                entity.Property(e => e.OrderId)
                    .HasColumnName("OrderID")
                    .HasComment("關聯用，訂單（Order）編號");

                entity.HasOne(d => d.Order)
                    .WithOne(p => p.PaymentOrder)
                    .HasForeignKey<PaymentOrder>(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PaymentOrder_Order");

                entity.HasOne(d => d.Payment)
                    .WithMany(p => p.PaymentOrders)
                    .HasForeignKey(d => d.PaymentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PaymentOrder_Payment");
            });

            modelBuilder.Entity<Questionnaire>(entity =>
            {
                entity.HasKey(e => e.QuestionnaireId)
                    .IsClustered(false);

                entity.ToTable("Questionnaire");

                entity.Property(e => e.QuestionnaireId)
                    .ValueGeneratedNever()
                    .HasColumnName("QuestionnaireID")
                    .HasComment("滿意度問卷編號，資料庫內部唯一識別用");

                entity.Property(e => e.Feedback)
                    .HasMaxLength(200)
                    .HasComment("使用者意見回饋");

                entity.Property(e => e.OrderId)
                    .HasColumnName("OrderID")
                    .HasComment("此問卷所評分的訂單（Order）");

                entity.Property(e => e.SatisfactionScore).HasComment("分數");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Questionnaires)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("UQ_Questionnaire_Order");
            });

            modelBuilder.Entity<QuestionnaireToken>(entity =>
            {
                entity.HasKey(e => e.TokenId);

                entity.ToTable("QuestionnaireToken");

                entity.HasIndex(e => e.HashedToken, "UQ_QuestionnaireToken_HashedToken")
                    .IsUnique();

                entity.Property(e => e.TokenId)
                    .HasColumnName("TokenID")
                    .HasComment("滿意度問卷權杖流水號，資料庫內部識別用");

                entity.Property(e => e.ExpireTime)
                    .HasColumnType("datetime")
                    .HasComment("滿意度問卷權杖失效時間");

                entity.Property(e => e.HashedToken)
                    .IsRequired()
                    .HasMaxLength(64)
                    .IsFixedLength(true)
                    .HasComment("權杖經 SHA512 而得的雜湊碼");

                entity.Property(e => e.OrderId)
                    .HasColumnName("OrderID")
                    .HasComment("此權杖所要評分的訂單（Order）");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.QuestionnaireTokens)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QuestionnaireToken_Order");
            });

            modelBuilder.Entity<Report>(entity =>
            {
                entity.HasKey(e => e.ReportId)
                    .IsClustered(false);

                entity.ToTable("Report");

                entity.HasIndex(e => e.ReportSn, "UQ_Report_ReportSN")
                    .IsUnique()
                    .IsClustered();

                entity.Property(e => e.ReportId)
                    .ValueGeneratedNever()
                    .HasColumnName("ReportID")
                    .HasComment("問題反映編號，資料庫內部唯一識別用");

                entity.Property(e => e.AdminId)
                    .HasColumnName("AdminID")
                    .HasComment("讓此問題反映結案的管理員編號");

                entity.Property(e => e.CloseTime)
                    .HasColumnType("datetime")
                    .HasComment("問題結案時間");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasComment("使用者問題描述");

                entity.Property(e => e.Note)
                    .HasMaxLength(100)
                    .HasComment("管理員結案註記");

                entity.Property(e => e.OrderDetailId)
                    .HasColumnName("OrderDetailID")
                    .HasComment("此反映所針對的訂單明細（Order Detail）");

                entity.Property(e => e.ReportSn)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ReportSN")
                    .HasComment("問題反映流水號，應用程式唯一識別用");

                entity.Property(e => e.ReportTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("問題反映時間");

                entity.HasOne(d => d.Admin)
                    .WithMany(p => p.Reports)
                    .HasForeignKey(d => d.AdminId)
                    .HasConstraintName("FK_Report_Admin");

                entity.HasOne(d => d.OrderDetail)
                    .WithMany(p => p.Reports)
                    .HasForeignKey(d => d.OrderDetailId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Report_OrderDetail");
            });

            modelBuilder.Entity<ResetPasswordToken>(entity =>
            {
                entity.HasKey(e => e.TokenId);

                entity.ToTable("ResetPasswordToken");

                entity.HasIndex(e => e.HashedToken, "UQ_ResetPasswordToken_HashedToken")
                    .IsUnique();

                entity.HasIndex(e => e.UserId, "UQ_ResetPasswordToken_UserID")
                    .IsUnique();

                entity.Property(e => e.TokenId)
                    .HasColumnName("TokenID")
                    .HasComment("重設使用者密碼權杖流水號，資料庫內部唯一識別用");

                entity.Property(e => e.ExpireTime)
                    .HasColumnType("datetime")
                    .HasComment("重設密碼權杖的失效時間");

                entity.Property(e => e.HashedToken)
                    .IsRequired()
                    .HasMaxLength(64)
                    .IsFixedLength(true)
                    .HasComment("權杖經 SHA512 而產生的雜湊碼");

                entity.Property(e => e.UserId)
                    .HasColumnName("UserID")
                    .HasComment("權杖所對應的使用者（User）編號");

                entity.HasOne(d => d.User)
                    .WithOne(p => p.ResetPasswordToken)
                    .HasForeignKey<ResetPasswordToken>(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ResetPasswordToken_User");
            });

            modelBuilder.Entity<Response>(entity =>
            {
                entity.HasKey(e => e.ResponseId)
                    .IsClustered(false);

                entity.ToTable("Response");

                entity.Property(e => e.ResponseId)
                    .ValueGeneratedNever()
                    .HasColumnName("ResponseID")
                    .HasComment("訂單回應的編號，資料庫內部唯一識別用");

                entity.Property(e => e.AdminId)
                    .HasColumnName("AdminID")
                    .HasComment("回應訂單的管理員編號");

                entity.Property(e => e.OrderId)
                    .HasColumnName("OrderID")
                    .HasComment("所回應的訂單");

                entity.Property(e => e.Reply)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true)
                    .HasComment("回應。核准或拒絕");

                entity.Property(e => e.ResponseTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("回應時間");

                entity.HasOne(d => d.Admin)
                    .WithMany(p => p.Responses)
                    .HasForeignKey(d => d.AdminId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Response_Admin");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Responses)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Response_Order");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.RoleId)
                    .IsClustered(false);

                entity.ToTable("Role");

                entity.HasIndex(e => e.RoleName, "UQ_Role_RoleName")
                    .IsUnique()
                    .HasFilter("([RoleName] IS NOT NULL)");

                entity.Property(e => e.RoleId)
                    .ValueGeneratedNever()
                    .HasColumnName("RoleID")
                    .HasComment("管理員角色編號，資料庫內部唯一識別用");

                entity.Property(e => e.RoleName)
                    .HasMaxLength(10)
                    .HasComment("管理員角色名稱");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .IsClustered(false);

                entity.ToTable("User");

                entity.HasIndex(e => e.Email, "UQ_User_Email")
                    .IsUnique()
                    .HasFilter("([Email] IS NOT NULL)");

                entity.HasIndex(e => e.PhoneNumber, "UQ_User_PhoneNumber")
                    .IsUnique()
                    .HasFilter("([PhoneNumber] IS NOT NULL)");

                entity.HasIndex(e => e.UserSn, "UQ_User_UserSN")
                    .IsUnique();

                entity.HasIndex(e => e.Username, "UQ_User_Username")
                    .IsUnique()
                    .HasFilter("([Username] IS NOT NULL)");

                entity.Property(e => e.UserId)
                    .ValueGeneratedNever()
                    .HasColumnName("UserID")
                    .HasComment("使用者編號，資料庫內部唯一識別用");

                entity.Property(e => e.Address)
                    .HasMaxLength(50)
                    .HasComment("地址");

                entity.Property(e => e.AllowNotification).HasComment("是否允許收到通知");

                entity.Property(e => e.Banned).HasComment("是否被禁止租借");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("註冊時間");

                entity.Property(e => e.DateOfBirth)
                    .HasColumnType("date")
                    .HasComment("生日");

                entity.Property(e => e.Deleted)
                    .HasDefaultValueSql("((0))")
                    .HasComment("使用者是否被刪除");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasComment("電子信箱");

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasComment("使用者姓名");

                entity.Property(e => e.Gender)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true)
                    .HasComment("性別");

                entity.Property(e => e.HashedPassword)
                    .IsRequired()
                    .HasMaxLength(64)
                    .IsFixedLength(true)
                    .HasComment("密碼經 PBKDF2 產生的雜湊碼");

                entity.Property(e => e.LineId)
                    .HasMaxLength(33)
                    .IsUnicode(false)
                    .HasColumnName("LineID")
                    .HasComment("LIND ID");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasComment("手機號碼");

                entity.Property(e => e.Salt)
                    .IsRequired()
                    .HasMaxLength(64)
                    .IsFixedLength(true)
                    .HasComment("PBKDF2 所使用的鹽");

                entity.Property(e => e.UserSn)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("UserSN")
                    .HasComment("使用者流水號，應用程式唯一識別用");

                entity.Property(e => e.Username)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasComment("使用者帳號");

                entity.Property(e => e.ViolationTimes)
                    .HasDefaultValueSql("((0))")
                    .HasComment("違規次數");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
