using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SunriseHotelApp.Models;

public partial class HotelManagementDbContext : DbContext
{
    public HotelManagementDbContext()
    { }

    public HotelManagementDbContext(DbContextOptions<HotelManagementDbContext> options)
        : base(options) { }

    public virtual DbSet<Booking> Bookings { get; set; }
    public virtual DbSet<BookingService> BookingServices { get; set; }
    public virtual DbSet<Customer> Customers { get; set; }
    public virtual DbSet<HotelInformation> HotelInformations { get; set; }
    public virtual DbSet<ImportReceipt> ImportReceipts { get; set; }
    public virtual DbSet<ImportReceiptDetail> ImportReceiptDetails { get; set; }
    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<Promotion> Promotions { get; set; }
    public virtual DbSet<Room> Rooms { get; set; }
    public virtual DbSet<RoomType> RoomTypes { get; set; }
    public virtual DbSet<Supplier> Suppliers { get; set; }
    public virtual DbSet<SystemUser> SystemUsers { get; set; }
    public virtual DbSet<Notification> Notifications { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server=.;Database=HotelManagementDB;Trusted_Connection=True;TrustServerCertificate=True;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId);
            entity.Property(e => e.BookingId).HasColumnName("BookingID");
            entity.Property(e => e.BookingDate).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
            entity.Property(e => e.BookingStatus).HasMaxLength(20);
            entity.Property(e => e.CheckInDate).HasColumnType("datetime");
            entity.Property(e => e.CheckOutDate).HasColumnType("datetime");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.DiscountAmount).HasDefaultValue(0m).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.FinalAmount).HasDefaultValue(0m).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.PaymentStatus).HasMaxLength(50);
            entity.Property(e => e.RoomId).HasColumnName("RoomID");
            entity.Property(e => e.TotalAmount).HasDefaultValue(0m).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Customer).WithMany(p => p.Bookings).HasForeignKey(d => d.CustomerId);
            entity.HasOne(d => d.Room).WithMany(p => p.Bookings).HasForeignKey(d => d.RoomId);
        });


        modelBuilder.Entity<BookingService>(entity =>
        {
            entity.ToTable("BookingServices");
            entity.HasKey(e => e.BookingServiceID);
            entity.Property(e => e.BookingServiceID).HasColumnName("BookingServiceID");
            entity.Property(e => e.BookingId).HasColumnName("BookingID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.PriceAtOrder).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.OrderDate).HasColumnType("datetime");

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingServices)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Product).WithMany().HasForeignKey(d => d.ProductId);
        });


        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId);
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.ProductName).HasMaxLength(100);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CurrentStock).HasDefaultValue(0);
            entity.Property(e => e.MinStockLevel).HasDefaultValue(5);
        });


        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId);
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20).IsUnicode(false);
            entity.Property(e => e.Nationality).HasDefaultValue("Vietnam");
        });


        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomId);
            entity.Property(e => e.RoomId).HasColumnName("RoomID");
            entity.Property(e => e.RoomNumber).HasMaxLength(10).IsUnicode(false);
            entity.Property(e => e.RoomStatus).HasDefaultValue("Available");

            entity.HasOne(d => d.RoomType).WithMany(p => p.Rooms).HasForeignKey(d => d.RoomTypeId);
        });

 
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
        });


        modelBuilder.Entity<RoomType>(entity =>
        {
            entity.HasKey(e => e.RoomTypeID);
            entity.Property(e => e.PricePerNight).HasColumnType("decimal(18, 2)");
        });


        modelBuilder.Entity<ImportReceipt>(entity =>
        {
            entity.HasKey(e => e.ReceiptId);
            entity.Property(e => e.ReceiptId).HasColumnName("ReceiptID");
            entity.Property(e => e.CreatedByUserId).HasColumnName("CreatedByUserID");
            entity.Property(e => e.ImportDate).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
            entity.Property(e => e.SupplierId).HasColumnName("SupplierID");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");
        });

  
        modelBuilder.Entity<ImportReceiptDetail>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ImportPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SubTotal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.ReceiptId).HasColumnName("ReceiptID");

            entity.HasOne(d => d.ImportReceipt).WithMany(p => p.ImportReceiptDetails)
                .HasForeignKey(d => d.ReceiptId).OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Product).WithMany().HasForeignKey(d => d.ProductId);
        });

 
        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId);
            entity.Property(e => e.SupplierName).HasMaxLength(200);
        });


        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.Property(e => e.DiscountPercent).HasColumnType("decimal(18, 2)");
        });

    
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}