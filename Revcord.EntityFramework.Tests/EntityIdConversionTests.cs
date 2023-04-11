using Microsoft.EntityFrameworkCore;
using Revcord.Entities;

namespace Revcord.EntityFramework.Tests;

public class EntityIdConversionTests {
	private TestDbContext? m_DbContext;

	[SetUp]
	public async Task Setup() {
		if (m_DbContext != null) {
			await m_DbContext.DisposeAsync();
			m_DbContext = null;
		}

		m_DbContext = new TestDbContext();

		await m_DbContext.Database.OpenConnectionAsync(); // must be done for some reason
		await m_DbContext.Database.EnsureCreatedAsync();
	}
	
	[OneTimeTearDown]
	public void Teardown() {
		m_DbContext?.Dispose();
	}

	[Test]
	public async Task FindById() {
		EntityId id = EntityId.Of(123);
		EntityId value = EntityId.Of(456);

		m_DbContext.Add(new TestEntity(id, value));
		await m_DbContext.SaveChangesAsync();

		TestEntity? retrieved = await m_DbContext.TestEntities.FindAsync(id);
		
		Assert.Multiple(() => {
			Assert.That(retrieved, Is.Not.Null);
			Assert.That(retrieved.Id, Is.EqualTo(id));
			Assert.That(retrieved.Value, Is.EqualTo(value));
		});
	}

	[Test]
	public async Task FirstOrDefault() {
		EntityId id = EntityId.Of(123);
		EntityId value = EntityId.Of(456);

		m_DbContext.Add(new TestEntity(id, value));
		await m_DbContext.SaveChangesAsync();

		TestEntity? retrieved = await m_DbContext.TestEntities.FirstOrDefaultAsync(te => te.Id == id);
		
		Assert.Multiple(() => {
			Assert.That(retrieved, Is.Not.Null);
			Assert.That(retrieved.Id, Is.EqualTo(id));
			Assert.That(retrieved.Value, Is.EqualTo(value));
		});
	}

	[Test]
	public async Task FirstOrDefaultNonKey() {
		EntityId id = EntityId.Of(123);
		EntityId value = EntityId.Of(456);

		m_DbContext.Add(new TestEntity(id, value));
		await m_DbContext.SaveChangesAsync();

		TestEntity? retrieved = await m_DbContext.TestEntities.FirstOrDefaultAsync(te => te.Value == value);
		
		Assert.Multiple(() => {
			Assert.That(retrieved, Is.Not.Null);
			Assert.That(retrieved.Id, Is.EqualTo(id));
			Assert.That(retrieved.Value, Is.EqualTo(value));
		});
	}
}
