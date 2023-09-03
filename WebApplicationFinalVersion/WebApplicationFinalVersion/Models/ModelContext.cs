using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

using WebApplicationFinalVersion.Models;

#nullable disable

namespace WebApplicationFinalVersion.Models
{
    public partial class ModelContext : DbContext
    {
        public ModelContext()
        {
        }

        public ModelContext(DbContextOptions<ModelContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AboutU> AboutUs { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<ContactU> ContactUs { get; set; }
        public virtual DbSet<OrderRecipe> OrderRecipes { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Recipe> Recipes { get; set; }
        public virtual DbSet<Reciperequest> Reciperequests { get; set; }
        public virtual DbSet<TemRequest> TemRequests { get; set; }
        public virtual DbSet<Testimonial> Testimonials { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseOracle("User Id=C##mvc;Password=123;Data Source=DESKTOP-GBFMD3N:1521/xeXDB;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("C##MVC")
                .HasAnnotation("Relational:Collation", "USING_NLS_COMP");

            modelBuilder.Entity<AboutU>(entity =>
            {
                entity.HasKey(e => e.AboutUsId)
                    .HasName("ABOUT_US_PK");

                entity.ToTable("ABOUT_US");

                entity.Property(e => e.AboutUsId)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ABOUT_US_ID");

                entity.Property(e => e.AboutUsEmail)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("ABOUT_US_EMAIL");

                entity.Property(e => e.AboutUsPhone)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("ABOUT_US_PHONE");

                entity.Property(e => e.AboutUsPhoto)
                    .HasMaxLength(1000)
                    .IsUnicode(false)
                    .HasColumnName("ABOUT_US_PHOTO");

                entity.Property(e => e.AboutUsText)
                    .HasMaxLength(2000)
                    .IsUnicode(false)
                    .HasColumnName("ABOUT_US_TEXT");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.Categoryid)
                    .HasColumnType("NUMBER(18,2)")
                    .ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<ContactU>(entity =>
            {
                entity.HasKey(e => e.ContactUsId)
                    .HasName("CONTACT_US_ID");

                entity.ToTable("CONTACT_US");

                entity.Property(e => e.Column1)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("COLUMN1");

                entity.Property(e => e.Column2)
                    .HasMaxLength(2000)
                    .IsUnicode(false)
                    .HasColumnName("COLUMN2");

                entity.Property(e => e.ContactUsId)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("CONTACT_US_ID");

                entity.Property(e => e.Name)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("NAME");
            });

            modelBuilder.Entity<OrderRecipe>(entity =>
            {
                entity.HasKey(e => e.OrderId)
                    .HasName("ORDER_RECIPE_PK");

                entity.ToTable("ORDER_RECIPE");

                entity.Property(e => e.OrderId)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ORDER_ID");

                entity.Property(e => e.OrderCreatedDate)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("ORDER_CREATED_DATE");

                entity.Property(e => e.RecipeId)
                    .HasColumnType("NUMBER")
                    .HasColumnName("RECIPE_ID");

                entity.HasOne(d => d.Recipe)
                    .WithMany(p => p.OrderRecipes)
                    .HasForeignKey(d => d.RecipeId)
                    .HasConstraintName("FK_REC_ID");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("PAYMENT");

                entity.Property(e => e.PaymentId)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("PAYMENT_ID");

                entity.Property(e => e.Amount)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("AMOUNT");

                entity.Property(e => e.CardCvc)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("CARD_CVC");

                entity.Property(e => e.CardNumber)
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .HasColumnName("CARD_NUMBER");

                entity.Property(e => e.Column1)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COLUMN1");
            });

            modelBuilder.Entity<Recipe>(entity =>
            {
                entity.HasIndex(e => e.Categoryid, "IX_Recipes_Categoryid");

                entity.HasIndex(e => e.Chefid, "IX_Recipes_Chefid");

                entity.Property(e => e.Recipeid)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Categoryid).HasColumnType("NUMBER(18,2)");

                entity.Property(e => e.Chefid).HasColumnType("NUMBER(18,2)");

                entity.Property(e => e.Creationdate).HasPrecision(7);

                entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("STATUS");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Recipes)
                    .HasForeignKey(d => d.Categoryid);

                entity.HasOne(d => d.Chef)
                    .WithMany(p => p.Recipes)
                    .HasForeignKey(d => d.Chefid);
            });

            modelBuilder.Entity<Reciperequest>(entity =>
            {
                entity.HasKey(e => e.Requestid);

                entity.HasIndex(e => e.Recipeid, "IX_Reciperequests_Recipeid");

                entity.HasIndex(e => e.Userid, "IX_Reciperequests_Userid");

                entity.Property(e => e.Requestid)
                    .HasColumnType("NUMBER(18,2)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Cost).HasColumnType("NUMBER(18,2)");

                entity.Property(e => e.CreatedDate).HasPrecision(7);

                entity.Property(e => e.Preparationtime).HasColumnType("NUMBER(18,2)");

                entity.Property(e => e.Recipeid).HasColumnType("NUMBER(18,2)");

                entity.Property(e => e.Userid).HasColumnType("NUMBER(18,2)");

                entity.HasOne(d => d.Recipe)
                    .WithMany(p => p.Reciperequests)
                    .HasForeignKey(d => d.Recipeid);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Reciperequests)
                    .HasForeignKey(d => d.Userid);
            });

            modelBuilder.Entity<TemRequest>(entity =>
            {
                entity.HasIndex(e => e.ChefId, "IX_TemRequests_ChefId");

                entity.Property(e => e.Id)
                    .HasColumnType("NUMBER(18,2)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.ChefId).HasColumnType("NUMBER(18,2)");

                entity.HasOne(d => d.Chef)
                    .WithMany(p => p.TemRequests)
                    .HasForeignKey(d => d.ChefId);
            });

            modelBuilder.Entity<Testimonial>(entity =>
            {
                entity.HasIndex(e => e.Userid, "IX_Testimonials_Userid");

                entity.Property(e => e.Testimonialid)
                    .HasColumnType("NUMBER(18,2)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Userid).HasColumnType("NUMBER(18,2)");

                // New property for the 'status' column
                

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Testimonials)
                    .HasForeignKey(d => d.Userid);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Userid)
                    .HasColumnType("NUMBER(18,2)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Email).IsRequired();

                entity.Property(e => e.Password).IsRequired();

                entity.Property(e => e.Username).IsRequired();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
