using Microsoft.EntityFrameworkCore;
using VideoGames.Core.Models;

namespace VideoGames.Infrasctructure;

public partial class VideoGamesContext : DbContext
{
    public VideoGamesContext()
    {
    }

    public VideoGamesContext(DbContextOptions<VideoGamesContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<GamePlatform> GamePlatforms { get; set; }

    public virtual DbSet<GamePublisher> GamePublishers { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Platform> Platforms { get; set; }

    public virtual DbSet<Publisher> Publishers { get; set; }

    public virtual DbSet<Region> Regions { get; set; }

    public virtual DbSet<RegionSale> RegionSales { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseMySql("server=localhost;port=3306;database=video_games;user=root", ServerVersion.Parse("10.4.32-mariadb"));
    }
         

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("game");

            entity.HasIndex(e => e.GenreId, "fk_gm_gen");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.GameName)
                .HasMaxLength(200)
                .HasColumnName("game_name");
            entity.Property(e => e.GenreId)
                .HasColumnType("int(11)")
                .HasColumnName("genre_id");

            entity.HasOne(d => d.Genre).WithMany(p => p.Games)
                .HasForeignKey(d => d.GenreId)
                .HasConstraintName("fk_gm_gen");
        });

        modelBuilder.Entity<GamePlatform>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("game_platform");

            entity.HasIndex(e => e.GamePublisherId, "fk_gpl_gp");

            entity.HasIndex(e => e.PlatformId, "fk_gpl_pla");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.GamePublisherId)
                .HasColumnType("int(11)")
                .HasColumnName("game_publisher_id");
            entity.Property(e => e.PlatformId)
                .HasColumnType("int(11)")
                .HasColumnName("platform_id");
            entity.Property(e => e.ReleaseYear)
                .HasColumnType("int(11)")
                .HasColumnName("release_year");

            entity.HasOne(d => d.GamePublisher).WithMany(p => p.GamePlatforms)
                .HasForeignKey(d => d.GamePublisherId)
                .HasConstraintName("fk_gpl_gp");

            entity.HasOne(d => d.Platform).WithMany(p => p.GamePlatforms)
                .HasForeignKey(d => d.PlatformId)
                .HasConstraintName("fk_gpl_pla");
        });

        modelBuilder.Entity<GamePublisher>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("game_publisher");

            entity.HasIndex(e => e.GameId, "fk_gpu_gam");

            entity.HasIndex(e => e.PublisherId, "fk_gpu_pub");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.GameId)
                .HasColumnType("int(11)")
                .HasColumnName("game_id");
            entity.Property(e => e.PublisherId)
                .HasColumnType("int(11)")
                .HasColumnName("publisher_id");

            entity.HasOne(d => d.Game).WithMany(p => p.GamePublishers)
                .HasForeignKey(d => d.GameId)
                .HasConstraintName("fk_gpu_gam");

            entity.HasOne(d => d.Publisher).WithMany(p => p.GamePublishers)
                .HasForeignKey(d => d.PublisherId)
                .HasConstraintName("fk_gpu_pub");
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("genre");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.GenreName)
                .HasMaxLength(50)
                .HasColumnName("genre_name");
        });

        modelBuilder.Entity<Platform>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("platform");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.PlatformName)
                .HasMaxLength(50)
                .HasColumnName("platform_name");
        });

        modelBuilder.Entity<Publisher>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("publisher");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.PublisherName)
                .HasMaxLength(100)
                .HasColumnName("publisher_name");
        });

        modelBuilder.Entity<Region>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("region");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.RegionName)
                .HasMaxLength(50)
                .HasColumnName("region_name");
        });

        modelBuilder.Entity<RegionSale>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("region_sales");

            entity.HasIndex(e => e.GamePlatformId, "fk_rs_gp");

            entity.HasIndex(e => e.RegionId, "fk_rs_reg");

            entity.Property(e => e.GamePlatformId)
                .HasColumnType("int(11)")
                .HasColumnName("game_platform_id");
            entity.Property(e => e.NumSales)
                .HasPrecision(5, 2)
                .HasColumnName("num_sales");
            entity.Property(e => e.RegionId)
                .HasColumnType("int(11)")
                .HasColumnName("region_id");

            entity.HasOne(d => d.GamePlatform).WithMany()
                .HasForeignKey(d => d.GamePlatformId)
                .HasConstraintName("fk_rs_gp");

            entity.HasOne(d => d.Region).WithMany()
                .HasForeignKey(d => d.RegionId)
                .HasConstraintName("fk_rs_reg");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
