using Revcord.Entities;

namespace Revcord.EntityFramework.Tests;

public class TestEntity {
	public EntityId Id { get; set; }
	public EntityId Value { get; set; }

	public TestEntity() {}
	public TestEntity(EntityId id, EntityId value) {
		Id = id;
		Value = value;
	}
}
