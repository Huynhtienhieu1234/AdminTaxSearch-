using System;
using System.Collections.Generic;
using AdminTaxSearch.Models;
using Microsoft.EntityFrameworkCore;

namespace AdminTaxSearch.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Apikey> Apikeys { get; set; }

    public virtual DbSet<BusinessInfo> BusinessInfos { get; set; }

    public virtual DbSet<ChatLog> ChatLogs { get; set; }

    public virtual DbSet<PersonalInfo> PersonalInfos { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<ScrapeResult> ScrapeResults { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<WardMapping> WardMappings { get; set; }

    public virtual DbSet<SearchHistory> SearchHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Apikey>(entity =>
        {
            entity.HasKey(e => e.ApikeyId).HasName("PK__APIKeys__C5971C76DD876521");

            entity.ToTable("APIKeys");

            entity.HasIndex(e => e.SystemName, "IX_APIKeys_SystemName");

            entity.HasIndex(e => e.Apikey1, "UQ__APIKeys__BB6E626DB9ABEEE5").IsUnique();

            entity.Property(e => e.ApikeyId).HasColumnName("APIKeyID");
            entity.Property(e => e.Apikey1)
                .HasMaxLength(255)
                .HasColumnName("APIKey");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IssuedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LastUsedDate).HasColumnType("datetime");
            entity.Property(e => e.RequestCount).HasDefaultValue(0);
            entity.Property(e => e.SystemName).HasMaxLength(255);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Apikeys)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_APIKeys_Users");
        });

        modelBuilder.Entity<BusinessInfo>(entity =>
        {
            entity.HasKey(e => e.BusinessId);
            entity.ToTable("BusinessInfo");

            entity.HasIndex(e => e.TaxId).IsUnique();

            entity.Property(e => e.BusinessName).HasMaxLength(255);
            entity.Property(e => e.Representative).HasMaxLength(255);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.Status).HasMaxLength(50);

            // Quan hệ với ScrapeResult
            entity.HasOne(d => d.ScrapeResult)
                  .WithMany(p => p.BusinessInfos)
                  .HasForeignKey(d => d.ScrapeResultId)
                  .OnDelete(DeleteBehavior.SetNull);
        });



        modelBuilder.Entity<ChatLog>(entity =>
        {
            entity.HasKey(e => e.ChatId).HasName("PK__ChatLogs__A9FBE626D908D78A");

            entity.HasIndex(e => e.Timestamp, "IX_ChatLogs_Timestamp");

            entity.HasIndex(e => e.UserId, "IX_ChatLogs_UserID");

            entity.Property(e => e.ChatId).HasColumnName("ChatID");
            entity.Property(e => e.ChatType).HasMaxLength(50);
            entity.Property(e => e.IsResolved).HasDefaultValue(true);
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.ChatLogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChatLogs__UserID__59063A47");
        });

        modelBuilder.Entity<PersonalInfo>(entity =>
        {
            entity.HasKey(e => e.PersonId);
            entity.ToTable("PersonalInfo");

            entity.HasIndex(e => e.Idnumber).IsUnique();
            entity.HasIndex(e => e.TaxId).IsUnique();

            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.Idnumber).HasMaxLength(50);
            entity.Property(e => e.TaxId).HasMaxLength(50);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.Occupation).HasMaxLength(100);
            entity.Property(e => e.UpdateDate).HasDefaultValueSql("(getdate())");

            // Quan hệ với ScrapeResult
            entity.HasOne(d => d.ScrapeResult)
                  .WithMany(p => p.PersonalInfos)
                  .HasForeignKey(d => d.ScrapeResultId)
                  .OnDelete(DeleteBehavior.SetNull);
        });


        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3AD40CD144");

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.RoleName).HasMaxLength(100);
        });

        modelBuilder.Entity<ScrapeResult>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("ScrapeResult");
            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("(getdate())");

            //entity.Ignore(e => e.UserId);
        });



        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC951DB859");

            entity.HasIndex(e => e.Email, "IX_Users_Email");

            entity.HasIndex(e => e.Username, "IX_Users_Username");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E4CE76A246").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534DC239EAB").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.Username).HasMaxLength(100);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__RoleID__5AEE82B9");
        });

        modelBuilder.Entity<WardMapping>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__WardMapp__3214EC2767FFB18A");

            entity.HasIndex(e => e.NewWardCode, "IX_WardMappings_NewCode");

            entity.HasIndex(e => e.OldWardCode, "IX_WardMappings_OldCode");
            entity.HasIndex(e => e.OldProvinceName)
                  .HasDatabaseName("IX_WardMappings_OldProvince");

            entity.HasIndex(e => e.OldDistrictName)
                  .HasDatabaseName("IX_WardMappings_OldDistrict");

            entity.HasIndex(e => e.OldWardName)
                  .HasDatabaseName("IX_WardMappings_OldWardName");
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.NewProvinceName).HasMaxLength(255);
            entity.Property(e => e.NewWardCode).HasMaxLength(20);
            entity.Property(e => e.NewWardName).HasMaxLength(255);
            entity.Property(e => e.OldDistrictName).HasMaxLength(255);
            entity.Property(e => e.OldProvinceName).HasMaxLength(255);
            entity.Property(e => e.OldWardCode).HasMaxLength(20);
            entity.Property(e => e.OldWardName).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });
        ////cũ bỏ chỉ có lưu cccd
        //    modelBuilder.Entity<SearchHistory>(entity =>
        //    {
        //        entity.HasKey(e => e.Id);
        //        entity.ToTable("SearchHistory");

        //        entity.Property(e => e.Cccd).HasMaxLength(20);
        //        entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

        //        entity.HasOne(d => d.PersonalInfo)
        //              .WithMany(p => p.SearchHistories)
        //              .HasForeignKey(d => d.PersonalInfoId)
        //              .OnDelete(DeleteBehavior.SetNull);

        //        entity.HasOne(d => d.BusinessInfo)
        //              .WithMany(b => b.SearchHistories)
        //              .HasForeignKey(d => d.BusinessInfoId)
        //              .OnDelete(DeleteBehavior.SetNull);
        //    });

        //    OnModelCreatingPartial(modelBuilder);
        //}
        //modelBuilder.Entity<SearchHistory>(entity =>
        //{
        //    entity.HasKey(e => e.Id);
        //    entity.ToTable("SearchHistory");

        //    entity.Property(e => e.SearchCode)
        //          .HasMaxLength(50)
        //          .HasColumnName("SearchCode");

        //    entity.Property(e => e.Ward)
        //          .HasMaxLength(255)
        //          .HasColumnName("Ward");

        //    entity.Property(e => e.District)
        //          .HasMaxLength(255)
        //          .HasColumnName("District");

        //    entity.Property(e => e.Province)
        //          .HasMaxLength(255)
        //          .HasColumnName("Province");

        //    entity.Property(e => e.SearchType)
        //          .HasMaxLength(50)
        //          .HasColumnName("SearchType");

        //    entity.Property(e => e.CreatedAt)
        //          .HasDefaultValueSql("(getdate())");

        //    entity.HasOne(d => d.PersonalInfo)
        //          .WithMany(p => p.SearchHistories)
        //          .HasForeignKey(d => d.PersonalInfoId)
        //          .OnDelete(DeleteBehavior.SetNull);

        //    entity.HasOne(d => d.BusinessInfo)
        //          .WithMany(b => b.SearchHistories)
        //          .HasForeignKey(d => d.BusinessInfoId)
        //          .OnDelete(DeleteBehavior.SetNull);
        //});
        modelBuilder.Entity<SearchHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("SearchHistory");

            entity.Property(e => e.InputText)
                  .HasColumnType("TEXT")
                  .HasColumnName("input_text");

            entity.Property(e => e.ResultText)
                  .HasColumnType("TEXT")
                  .HasColumnName("result_text");

            entity.Property(e => e.CreatedAt)
                  .HasColumnType("DATETIME")
                  .HasDefaultValueSql("(getdate())")
                  .HasColumnName("created_at");

            entity.Property(e => e.IpAddress)
                  .HasMaxLength(50)
                  .HasColumnName("ip_address");

            entity.HasOne(d => d.User)
                  .WithMany(u => u.SearchHistories)
                  .HasForeignKey(d => d.UserId)
                  .OnDelete(DeleteBehavior.SetNull);
        });


    }
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
