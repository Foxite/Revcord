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
		var ret = JsonConvert.SerializeObject(entityId);

		Console.WriteLine("Serialize");
		Console.WriteLine(ret);
		
		return ret;
	}

	private static EntityId DeserializeObject(string str) {
		Console.WriteLine("Deserialize");
		Console.WriteLine(str);
		return JsonConvert.DeserializeObject<EntityId>(str);
	}
}
