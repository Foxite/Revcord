using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Revcord.Entities;

namespace Revcord.EntityFramework.Tests;

public class TestDbContext : DbContext {
	public DbSet<TestEntity> TestEntities { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
		optionsBuilder.UseSqlite("DataSource=:memory:");
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder) {
		base.OnModelCreating(modelBuilder);
		
		modelBuilder.ConfigureEntityIdConversions<TestEntity>(te => te.Id, te => te.Value);
	}
}
