using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Revcord.Entities;

namespace Revcord.EntityFramework;

public static class EntityFrameworkExtensions {
	public static void ConfigureEntityIdConversions<TEntity>(this ModelBuilder modelBuilder, params Expression<Func<TEntity, EntityId>>[] expressions) where TEntity : class {
		foreach (Expression<Func<TEntity, EntityId>> expression in expressions) {
			modelBuilder
				.Entity<TEntity>()
				.Property(expression)
				.HasConversion(EntityIdConverter.Instance);
		}
	}
}
