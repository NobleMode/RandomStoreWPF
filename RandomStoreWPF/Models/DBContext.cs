using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace RandomStoreWPF.Models;

public partial class DBContext : DbContext
{
    public DBContext()
    {
    }

    public DBContext(DbContextOptions<DBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<GameType> GameTypes { get; set; }

    public virtual DbSet<Ownership> Ownerships { get; set; }

    public virtual DbSet<RoleTable> RoleTables { get; set; }

    public virtual DbSet<SecurityQuestion> SecurityQuestions { get; set; }

    public virtual DbSet<UserCode> UserCodes { get; set; }

    public virtual DbSet<UserSecurity> UserSecurities { get; set; }

    public virtual DbSet<UserTable> UserTables { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=LAPTOP-BNRIT7SD\\BASESERVER;Initial Catalog=randomStoreWPF; Trusted_Connection=SSPI;Encrypt=false");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("SQL_Latin1_General_CP1_CS_AS");

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.GameId }).HasName("PK__cart__564026F354A08927");

            entity.ToTable("cart");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.GameId).HasColumnName("game_id");
            entity.Property(e => e.CartId)
                .ValueGeneratedOnAdd()
                .HasColumnName("cart_id");

            entity.HasOne(d => d.Game).WithMany(p => p.Carts)
                .HasForeignKey(d => d.GameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__cart__game_id__71D1E811");

            entity.HasOne(d => d.User).WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__cart__user_id__70DDC3D8");
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.GameId).HasName("PK__game__FFE11FCFCF5FBC24");

            entity.ToTable("game");

            entity.Property(e => e.GameId).HasColumnName("game_id");
            entity.Property(e => e.GameDescription)
                .HasMaxLength(200)
                .IsUnicode(false)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS")
                .HasColumnName("game_description");
            entity.Property(e => e.GameDeveloper).HasColumnName("game_developer");
            entity.Property(e => e.GameName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS")
                .HasColumnName("game_name");
            entity.Property(e => e.GameStatus).HasColumnName("game_status");
            entity.Property(e => e.GameTypeId).HasColumnName("game_type_id");
            entity.Property(e => e.JsonDataLoc)
                .IsUnicode(false)
                .HasColumnName("json_data_loc");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.Size).HasColumnName("size");

            entity.HasOne(d => d.GameDeveloperNavigation).WithMany(p => p.Games)
                .HasForeignKey(d => d.GameDeveloper)
                .HasConstraintName("FK__game__game_devel__3F466844");

            entity.HasOne(d => d.GameType).WithMany(p => p.Games)
                .HasForeignKey(d => d.GameTypeId)
                .HasConstraintName("FK__game__game_type___3E52440B");
        });

        modelBuilder.Entity<GameType>(entity =>
        {
            entity.HasKey(e => e.GameTypeId).HasName("PK__game_typ__327215CAF29497BA");

            entity.ToTable("game_type");

            entity.Property(e => e.GameTypeId).HasColumnName("game_type_id");
            entity.Property(e => e.GameTypeDescription)
                .HasMaxLength(100)
                .IsUnicode(false)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS")
                .HasColumnName("game_type_description");
            entity.Property(e => e.GameTypeName)
                .HasMaxLength(3)
                .IsUnicode(false)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS")
                .HasColumnName("game_type_name");
        });

        modelBuilder.Entity<Ownership>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.GameId }).HasName("PK__ownershi__564026F310D1438B");

            entity.ToTable("ownership");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.GameId).HasColumnName("game_id");
            entity.Property(e => e.BuyDate)
                .HasColumnType("datetime")
                .HasColumnName("buy_date");
            entity.Property(e => e.OwnershipId)
                .ValueGeneratedOnAdd()
                .HasColumnName("ownership_id");
            entity.Property(e => e.PersonalCode)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("personal_code");

            entity.HasOne(d => d.Game).WithMany(p => p.Ownerships)
                .HasForeignKey(d => d.GameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ownership__game___4316F928");

            entity.HasOne(d => d.User).WithMany(p => p.Ownerships)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ownership__user___4222D4EF");
        });

        modelBuilder.Entity<RoleTable>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__role_tab__760965CC072F6A4C");

            entity.ToTable("role_table");

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Perm)
                .HasMaxLength(50)
                .IsUnicode(false)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS")
                .HasColumnName("perm");
            entity.Property(e => e.RoleName)
                .HasMaxLength(10)
                .IsUnicode(false)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS")
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<SecurityQuestion>(entity =>
        {
            entity.HasKey(e => e.QuestionId).HasName("PK__security__2EC2154947ACCCCD");

            entity.ToTable("security_question");

            entity.Property(e => e.QuestionId).HasColumnName("question_id");
            entity.Property(e => e.QuestionText)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("question_text");
            entity.Property(e => e.Severity).HasColumnName("severity");
        });

        modelBuilder.Entity<UserCode>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__user_cod__B9BE370F43730FA6");

            entity.ToTable("user_code");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");
            entity.Property(e => e.Code)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("code");
            entity.Property(e => e.Downloaded).HasColumnName("downloaded");

            entity.HasOne(d => d.User).WithOne(p => p.UserCode)
                .HasForeignKey<UserCode>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__user_code__user___09A971A2");
        });

        modelBuilder.Entity<UserSecurity>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.QuestionId }).HasName("PK__user_sec__3B52165BC61AC3AC");

            entity.ToTable("user_security");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.QuestionId).HasColumnName("question_id");
            entity.Property(e => e.Answer)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("answer");

            entity.HasOne(d => d.Question).WithMany(p => p.UserSecurities)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__user_secu__quest__06CD04F7");

            entity.HasOne(d => d.User).WithMany(p => p.UserSecurities)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__user_secu__user___05D8E0BE");
        });

        modelBuilder.Entity<UserTable>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__user_tab__B9BE370FF2617DAD");

            entity.ToTable("user_table");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS")
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(30)
                .IsUnicode(false)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS")
                .HasColumnName("full_name");
            entity.Property(e => e.Password)
                .HasMaxLength(32)
                .IsUnicode(false)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS")
                .HasColumnName("password");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UserName)
                .HasMaxLength(30)
                .IsUnicode(false)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS")
                .HasColumnName("user_name");

            entity.HasOne(d => d.Role).WithMany(p => p.UserTables)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__user_tabl__role___398D8EEE");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
