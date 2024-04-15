using Dysnomia.CoursFrontM1.GamesDb.Common;
using Dysnomia.CoursFrontM1.GamesDb.Common.Dao;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Dysnomia.CoursFrontM1.GamesDb.DataAccess {
    public class DatabaseContext : DbContext {
        public DbSet<UserDao> Users { get; set; }

        private readonly string ConnectionString;

        public DatabaseContext(IOptions<AppSettings> options) {
            this.ConnectionString = options.Value?.ConnectionString
                    ?? throw new ArgumentNullException(nameof(ConnectionString));
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                 => optionsBuilder.UseNpgsql(ConnectionString);

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            var userBuilder = modelBuilder.Entity<UserDao>();

            userBuilder.HasKey(x => x.Id);

            userBuilder.Property(x => x.Id);
            userBuilder.Property(x => x.Name)
                .HasMaxLength(255)
                .IsUnicode(true)
                .HasColumnType("varchar");

            userBuilder.HasIndex(x => x.Name).IsUnique();
        }
    }
}