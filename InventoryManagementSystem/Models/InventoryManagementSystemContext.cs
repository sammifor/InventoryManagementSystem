using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace InventoryManagementSystem.Models
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
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<ItemLog> ItemLogs { get; set; }
        public virtual DbSet<LineNotification> LineNotifications { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<OrderDetailStatus> OrderDetailStatuses { get; set; }
        public virtual DbSet<OrderStatus> OrderStatuses { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<PaymentCategory> PaymentCategories { get; set; }
        public virtual DbSet<PaymentDetail> PaymentDetails { get; set; }
        public virtual DbSet<PaymentOrder> PaymentOrders { get; set; }
        public virtual DbSet<Questionnaire> Questionnaires { get; set; }
        public virtual DbSet<Report> Reports { get; set; }
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
                entity.ToTable("Admin");

                entity.HasIndex(e => e.Username, "UQ_Admin_Username")
                    .IsUnique();

                entity.Property(e => e.AdminId).HasColumnName("AdminID");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(64)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50);

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

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.CancelTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description).HasMaxLength(100);

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
                    .IsFixedLength(true);

                entity.Property(e => e.ConditionName)
                    .IsRequired()
                    .HasMaxLength(10);
            });

            modelBuilder.Entity<EquipCategory>(entity =>
            {
                entity.ToTable("EquipCategory");

                entity.Property(e => e.EquipCategoryId).HasColumnName("EquipCategoryID");

                entity.Property(e => e.CategoryName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Equipment>(entity =>
            {
                entity.HasIndex(e => e.EquipmentSn, "UQ_Equipment_EquipmentSN")
                    .IsUnique();

                entity.Property(e => e.EquipmentId).HasColumnName("EquipmentID");

                entity.Property(e => e.Brand).HasMaxLength(50);

                entity.Property(e => e.Description).HasMaxLength(100);

                entity.Property(e => e.EquipmentCategoryId).HasColumnName("EquipmentCategoryID");

                entity.Property(e => e.EquipmentName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.EquipmentSn)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("EquipmentSN");

                entity.Property(e => e.Model).HasMaxLength(50);

                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 0)");

                entity.HasOne(d => d.EquipmentCategory)
                    .WithMany(p => p.Equipment)
                    .HasForeignKey(d => d.EquipmentCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Equipment_EquipCategory");
            });

            modelBuilder.Entity<Item>(entity =>
            {
                entity.ToTable("Item");

                entity.HasIndex(e => e.ItemSn, "UQ_Item_ItemSN")
                    .IsUnique();

                entity.Property(e => e.ItemId).HasColumnName("ItemID");

                entity.Property(e => e.ConditionId)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("ConditionID")
                    .IsFixedLength(true);

                entity.Property(e => e.Description).HasMaxLength(100);

                entity.Property(e => e.EquipmentId).HasColumnName("EquipmentID");

                entity.Property(e => e.ItemSn)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("ItemSN");

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
                entity.ToTable("ItemLog");

                entity.Property(e => e.ItemLogId).HasColumnName("ItemLogID");

                entity.Property(e => e.AdminId).HasColumnName("AdminID");

                entity.Property(e => e.ConditionId)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("ConditionID")
                    .IsFixedLength(true);

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description).HasMaxLength(100);

                entity.Property(e => e.ItemId).HasColumnName("ItemID");

                entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetailID");

                entity.HasOne(d => d.Admin)
                    .WithMany(p => p.ItemLogs)
                    .HasForeignKey(d => d.AdminId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
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

            modelBuilder.Entity<LineNotification>(entity =>
            {
                entity.ToTable("LineNotification");

                entity.Property(e => e.LineNotificationId).HasColumnName("LineNotificationID");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetailID");

                entity.HasOne(d => d.OrderDetail)
                    .WithMany(p => p.LineNotifications)
                    .HasForeignKey(d => d.OrderDetailId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LineNotification_OrderDetail");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.EquipmentId).HasColumnName("EquipmentID");

                entity.Property(e => e.EstimatedPickupTime).HasColumnType("datetime");

                entity.Property(e => e.OrderStatusId)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("OrderStatusID")
                    .HasDefaultValueSql("('P')")
                    .IsFixedLength(true);

                entity.Property(e => e.OrderTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UserId).HasColumnName("UserID");

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
                entity.ToTable("OrderDetail");

                entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetailID");

                entity.Property(e => e.ItemId).HasColumnName("ItemID");

                entity.Property(e => e.OrderDetailStatusId)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("OrderDetailStatusID")
                    .IsFixedLength(true);

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

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
                    .IsFixedLength(true);

                entity.Property(e => e.StatusName)
                    .IsRequired()
                    .HasMaxLength(10);
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
                    .IsFixedLength(true);

                entity.Property(e => e.StatusName)
                    .IsRequired()
                    .HasMaxLength(10);
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("Payment");

                entity.Property(e => e.PaymentId).HasColumnName("PaymentID");

                entity.Property(e => e.Fee).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.PaymentCategoryId)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("PaymentCategoryID")
                    .IsFixedLength(true);

                entity.HasOne(d => d.PaymentCategory)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.PaymentCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Payment_PaymentCategory");
            });

            modelBuilder.Entity<PaymentCategory>(entity =>
            {
                entity.ToTable("PaymentCategory");

                entity.Property(e => e.PaymentCategoryId)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("PaymentCategoryID")
                    .IsFixedLength(true);

                entity.Property(e => e.PaymentCategoryName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<PaymentDetail>(entity =>
            {
                entity.HasKey(e => e.PaymentDetail1);

                entity.ToTable("PaymentDetail");

                entity.Property(e => e.PaymentDetail1).HasColumnName("PaymentDetail");

                entity.Property(e => e.AmountPaid).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.PayTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.PaymentId).HasColumnName("PaymentID");

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

                entity.Property(e => e.PaymentId).HasColumnName("PaymentID");

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.PaymentOrders)
                    .HasForeignKey(d => d.OrderId)
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
                entity.ToTable("Questionnaire");

                entity.Property(e => e.QuestionnaireId).HasColumnName("QuestionnaireID");

                entity.Property(e => e.Feedback).HasMaxLength(200);

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Questionnaires)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("UQ_Questionnaire_Order");
            });

            modelBuilder.Entity<Report>(entity =>
            {
                entity.ToTable("Report");

                entity.Property(e => e.ReportId).HasColumnName("ReportID");

                entity.Property(e => e.CloseTime).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetailID");

                entity.Property(e => e.ReportTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.OrderDetail)
                    .WithMany(p => p.Reports)
                    .HasForeignKey(d => d.OrderDetailId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Report_OrderDetail");
            });

            modelBuilder.Entity<Response>(entity =>
            {
                entity.ToTable("Response");

                entity.Property(e => e.ResponseId).HasColumnName("ResponseID");

                entity.Property(e => e.AdminId).HasColumnName("AdminID");

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.Reply)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.ResponseTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

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
                entity.ToTable("Role");

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(10);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.HasIndex(e => e.Email, "UQ_User_Email")
                    .IsUnique();

                entity.HasIndex(e => e.PhoneNumber, "UQ_User_PhoneNumber")
                    .IsUnique();

                entity.HasIndex(e => e.Username, "UQ_User_Username")
                    .IsUnique();

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.Address).HasMaxLength(50);

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DateOfBirth).HasColumnType("date");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Gender)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.LineAccount)
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(64)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ViolationTimes).HasDefaultValueSql("((0))");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
