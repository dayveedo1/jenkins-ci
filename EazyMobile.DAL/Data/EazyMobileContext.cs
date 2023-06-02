using System;
using EazyMobile.DAL.Data.Config;
using EazyMobile.DAL.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace EazyMobile.DAL.Data.Models
{
    public partial class EazyMobileContext : IdentityDbContext<User>
    {
        public EazyMobileContext()
        {
        }

        public EazyMobileContext(DbContextOptions<EazyMobileContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Beneficiary> Beneficiaries { get; set; }
        public virtual DbSet<Device> Devices { get; set; }
        public virtual DbSet<LoginHist> LoginHists { get; set; }
        public virtual DbSet<PrmType> PrmTypes { get; set; }
        public virtual DbSet<PrmTypesDetail> PrmTypesDetails { get; set; }
        public virtual DbSet<TransferLimit> TransferLimits { get; set; }
        public virtual DbSet<UserMaintHist> UserMaintHists { get; set; }
        public virtual DbSet<User> AspNetUsers { get; set; }
        public virtual DbSet<Otp> Otps { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new RoleConfiguration());

            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(e => e.SeqNo);

                entity.Property(e => e.AccountName)
                   .IsRequired()
                   .HasMaxLength(50);

                entity.Property(e => e.AccountType)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.AccountStatus);
                        //.ValueGeneratedNever();

                entity.Property(e => e.BranchCode)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.BranchCode)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.BranchName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasIndex(e => e.UserId, "IX_Accounts_UserId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<Beneficiary>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.BeneficiaryAccountNo });

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.BeneficiaryAccountNo).HasMaxLength(20);

                entity.Property(e => e.BeneficiaryAccountName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.BeneficiaryBank)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Beneficiaries)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Beneficiaries_AspNetUsers");
            });

            modelBuilder.Entity<Device>(entity =>
            {
                entity.Property(e => e.DeviceId)
                    .HasMaxLength(100)
                    .HasColumnName("DeviceID");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450)
                    .HasColumnName("UserID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Devices)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Devices_AspNetUsers");
            });

            modelBuilder.Entity<LoginHist>(entity =>
            {
                entity.HasKey(e => e.SeqNo);

                entity.ToTable("LoginHist");

                entity.Property(e => e.DeviceId)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("DeviceID");

                entity.Property(e => e.Location)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.LoginDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450)
                    .HasColumnName("UserID");

                entity.HasOne(d => d.Device)
                    .WithMany(p => p.LoginHists)
                    .HasForeignKey(d => d.DeviceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LoginHist_Devices");
            });

            modelBuilder.Entity<PrmType>(entity =>
            {
                entity.HasKey(e => e.TypeCode);

                entity.Property(e => e.TypeCode).ValueGeneratedNever();

                entity.Property(e => e.LabelCode).HasMaxLength(10);

                entity.Property(e => e.LabelDesc).HasMaxLength(100);

                entity.Property(e => e.TypeDesc)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<PrmTypesDetail>(entity =>
            {
                entity.HasKey(e => new { e.TypeCode, e.Code });

                entity.Property(e => e.Code).HasMaxLength(10);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Display)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.HasOne(d => d.TypeCodeNavigation)
                    .WithMany(p => p.PrmTypesDetails)
                    .HasForeignKey(d => d.TypeCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PrmTypesDetails_PrmTypes");
            });

            modelBuilder.Entity<TransferLimit>(entity =>
            {
                entity.HasKey(e => e.SeqNo);

                entity.ToTable("TransferLimit");

                entity.HasIndex(e => e.UserId, "IX_TransferLimit_1")
                    .IsUnique();

                entity.Property(e => e.CurrentLimit).HasColumnType("decimal(32, 2)");

                entity.Property(e => e.DateTimeModified)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.PreviousLimit).HasColumnType("decimal(32, 2)");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("UserID");

                entity.HasOne(d => d.User)
                    .WithOne(p => p.TransferLimit)
                    .HasForeignKey<TransferLimit>(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TransferLimit_AspNetUsers");
            });

            //modelBuilder.Entity<TransferLimit>(entity =>
            //{
            //    entity.HasKey(e => e.SeqNo);

            //    entity.ToTable("TransferLimit");

            //    entity.Property(e => e.CurrentLimit).HasColumnType("decimal(32, 2)");

            //    entity.Property(e => e.DateTimeModified)
            //        .HasColumnType("datetime")
            //        .HasDefaultValueSql("(getdate())");

            //    entity.Property(e => e.PreviousLimit).HasColumnType("decimal(32, 2)");

            //    entity.Property(e => e.UserId)
            //        .IsRequired()
            //        .HasMaxLength(450)
            //        .HasColumnName("UserID");

            //    entity.HasOne(d => d.User)
            //        .WithMany(p => p.TransferLimits)
            //        .HasForeignKey(d => d.UserId)
            //        .OnDelete(DeleteBehavior.ClientSetNull)
            //        .HasConstraintName("FK_TransferLimit_AspNetUsers");
            //});

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedUserName] IS NOT NULL)");

                entity.Property(e => e.BlockedBy).HasMaxLength(50);

                entity.Property(e => e.Bvn).HasMaxLength(50);

                entity.Property(e => e.CustomerId).HasMaxLength(50);

                entity.Property(e => e.DateCreated).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.FullName).HasMaxLength(50);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(50);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(100);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Pin).HasMaxLength(100);

                entity.Property(e => e.UserStatus)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.UserId).HasMaxLength(50);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<UserMaintHist>(entity =>
            {
                entity.HasKey(e => e.SeqNo)
                    .HasName("PK__UserMaintHist");

                entity.ToTable("UserMaintHist");

                entity.Property(e => e.ActivityDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.BlockedBy)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.MaintFlagCode)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450)
                    .HasColumnName("UserID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserMaintHists)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserMaintHist_AspNetUsers");
            });

            modelBuilder.Entity<Otp>(entity =>
            {
                entity.HasKey(e => e.SeqNo);

                entity.ToTable("Otp");

                entity.Property(e => e.AccountNo)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.OtpCode).HasMaxLength(100);

                entity.Property(e => e.OtpTimeCreated).HasColumnType("datetime");

                entity.Property(e => e.PhoneNumber).HasMaxLength(50);

                entity.Property(e => e.Status).HasMaxLength(50);
            });

            //OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
