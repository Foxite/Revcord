using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using Revcord.Entities;

namespace Revcord.EntityFramework;

public class EntityIdConverter : ValueConverter<EntityId, string> {
	public static EntityIdConverter Instance { get; } = new EntityIdConverter();

	public EntityIdConverter() : base(
		entityId => SerializeObject(entityId),
		str => DeserializeObject(str)
	) { }

	private static string SerializeObject(EntityId entityId) {
		return JsonConvert.SerializeObject(entityId);
	}

	private static EntityId DeserializeObject(string str) {
		return JsonConvert.DeserializeObject<EntityId>(str);
	}
}
