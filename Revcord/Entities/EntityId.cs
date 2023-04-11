using System.Collections.Concurrent;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Revcord.Entities;

[JsonConverter(typeof(EntityIdJsonConverter))]
public readonly record struct EntityId {
	public object UnderlyingId { get; init; }
	
	private EntityId(object underlyingId) {
		if (underlyingId == null) {
			throw new InvalidOperationException("Attempt to wrap an null in an EntityId");
		}

		if (underlyingId is EntityId) {
			throw new InvalidOperationException("Attempt to wrap an EntityId in an EntityId");
		}
		
		UnderlyingId = underlyingId;
	}

	public static EntityId Of<T>(T id) where T : notnull => new EntityId(id);
};

public class EntityIdJsonConverter : JsonConverter<EntityId> {
	private readonly ConcurrentDictionary<string, Type> m_TypeCache = new();

	public override void WriteJson(JsonWriter writer, EntityId value, JsonSerializer serializer) {
		serializer.Serialize(writer, new SerializedEntityId(value.UnderlyingId.GetType().FullName ?? throw new Exception("what"), JToken.FromObject(value.UnderlyingId)));
	}

	public override EntityId ReadJson(JsonReader reader, Type objectType, EntityId existingValue, bool hasExistingValue, JsonSerializer serializer) {
		var serialized = serializer.Deserialize<SerializedEntityId>(reader)!;
		Type type = m_TypeCache.GetOrAdd(serialized.TypeName, FindType);
		return EntityId.Of(serialized.Value.ToObject(type)!);
	}

	private static Type FindType(string fullname) {
		return AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.ExportedTypes).FirstOrDefault(type => type.FullName == fullname) ?? throw new TypeLoadException($"Unknown type {fullname}");
	}
	
	private record SerializedEntityId(string TypeName, JToken Value);
}
