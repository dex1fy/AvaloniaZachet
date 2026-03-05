using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AvaloniaZachet.Models;

public partial class _41pProductsContext : DbContext
{
    public _41pProductsContext(DbContextOptions<_41pProductsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<MaterialType> MaterialTypes { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductType> ProductTypes { get; set; }

    public virtual DbSet<ProductWorkshop> ProductWorkshops { get; set; }

    public virtual DbSet<Workshop> Workshops { get; set; }

    public virtual DbSet<WorkshopType> WorkshopTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MaterialType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("material_type_pkey");

            entity.ToTable("material_type");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.MaterialType1)
                .HasColumnType("character varying")
                .HasColumnName("material_type");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("products_pkey");

            entity.ToTable("products");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Article)
                .HasColumnType("character varying")
                .HasColumnName("article");
            entity.Property(e => e.IdMaterialType).HasColumnName("id_material_type");
            entity.Property(e => e.IdProductType).HasColumnName("id_product_type");
            entity.Property(e => e.MinCostPartner).HasColumnName("min_cost_partner");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");

            entity.HasOne(d => d.IdMaterialTypeNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.IdMaterialType)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("products_id_material_type_fkey");

            entity.HasOne(d => d.IdProductTypeNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.IdProductType)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("products_id_product_type_fkey");
        });

        modelBuilder.Entity<ProductType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("product_type_pkey");

            entity.ToTable("product_type");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ProductType1)
                .HasColumnType("character varying")
                .HasColumnName("product_type");
        });

        modelBuilder.Entity<ProductWorkshop>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("product_workshop_pkey");

            entity.ToTable("product_workshop");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IdProduct).HasColumnName("id_product");
            entity.Property(e => e.IdWorkshops).HasColumnName("id_workshops");
            entity.Property(e => e.Time).HasColumnName("time");

            entity.HasOne(d => d.IdProductNavigation).WithMany(p => p.ProductWorkshops)
                .HasForeignKey(d => d.IdProduct)
                .HasConstraintName("product_workshop_id_product_fkey");

            entity.HasOne(d => d.IdWorkshopsNavigation).WithMany(p => p.ProductWorkshops)
                .HasForeignKey(d => d.IdWorkshops)
                .HasConstraintName("product_workshop_id_workshops_fkey");
        });

        modelBuilder.Entity<Workshop>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("workshops_pkey");

            entity.ToTable("workshops");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CountPerson).HasColumnName("count_person");
            entity.Property(e => e.IdWorkshopType).HasColumnName("id_workshop_type");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");

            entity.HasOne(d => d.IdWorkshopTypeNavigation).WithMany(p => p.Workshops)
                .HasForeignKey(d => d.IdWorkshopType)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("workshops_id_workshop_type_fkey");
        });

        modelBuilder.Entity<WorkshopType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("workshop_type_pkey");

            entity.ToTable("workshop_type");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.WorkshopType1)
                .HasColumnType("character varying")
                .HasColumnName("workshop_type");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
