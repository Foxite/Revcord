using Revcord.Entities;

namespace Revcord.Revolt;

internal static class EntityIdExtensions {
	public static string String(this EntityId entityId) => (string) entityId.UnderlyingId;
}