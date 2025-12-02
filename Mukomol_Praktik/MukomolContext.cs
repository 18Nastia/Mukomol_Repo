using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Mukomol_Praktik;

public partial class MukomolContext : DbContext
{
    public MukomolContext()
    {
    }

    public MukomolContext(DbContextOptions<MukomolContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Flour> Flours { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Partner> Partners { get; set; }

    public virtual DbSet<Pastum> Pasta { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-RKQAB90\\SQLEXPRESS;Initial Catalog=Mukomol;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Flour>(entity =>
        {
            entity.HasKey(e => e.IdFlour);

            entity.ToTable("Flour");

            entity.Property(e => e.IdFlour).HasColumnName("ID_flour");
            entity.Property(e => e.Gluten)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("gluten");
            entity.Property(e => e.Gost)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("gost");
            entity.Property(e => e.Idk)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idk");
            entity.Property(e => e.NameFlour)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name_flour");
            entity.Property(e => e.White)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("white");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.IdOrder);

            entity.ToTable("Order");

            entity.Property(e => e.IdOrder).HasColumnName("ID_order");
            entity.Property(e => e.CommentOrder)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("comment_order");
            entity.Property(e => e.DateOrder).HasColumnName("date_order");
            entity.Property(e => e.DateShipping).HasColumnName("date_shipping");
            entity.Property(e => e.IdPartner).HasColumnName("ID_partner");
            entity.Property(e => e.StatusOrder)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("Принят")
                .HasColumnName("status_order");

            entity.HasOne(d => d.IdPartnerNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.IdPartner)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_Partner");
        });

        modelBuilder.Entity<Partner>(entity =>
        {
            entity.HasKey(e => e.IdPartner);

            entity.ToTable("Partner");

            entity.Property(e => e.IdPartner).HasColumnName("ID_partner");
            entity.Property(e => e.ContactNumber)
                .HasMaxLength(11)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("contact_number");
            entity.Property(e => e.LogoPartner)
                .IsUnicode(false)
                .HasColumnName("logo_partner");
            entity.Property(e => e.NameCompany)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name_company");
        });

        modelBuilder.Entity<Pastum>(entity =>
        {
            entity.HasKey(e => e.IdPasta);

            entity.Property(e => e.IdPasta).HasColumnName("ID_pasta");
            entity.Property(e => e.Brand)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("brand");
            entity.Property(e => e.ImagePasta)
                .IsUnicode(false)
                .HasColumnName("image_pasta");
            entity.Property(e => e.Packaging).HasColumnName("packaging");
            entity.Property(e => e.TypePasta)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("type_pasta");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.IdProduct);

            entity.ToTable("Product");

            entity.Property(e => e.IdProduct).HasColumnName("ID_product");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.IdFlour).HasColumnName("ID_flour");
            entity.Property(e => e.IdOrder).HasColumnName("ID_order");
            entity.Property(e => e.IdPasta).HasColumnName("ID_pasta");

            entity.HasOne(d => d.IdFlourNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.IdFlour)
                .HasConstraintName("FK_Product_Flour");

            entity.HasOne(d => d.IdOrderNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.IdOrder)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_Order");

            entity.HasOne(d => d.IdPastaNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.IdPasta)
                .HasConstraintName("FK_Product_Pasta");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
