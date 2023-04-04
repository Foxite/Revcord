namespace Revcord.Entities;

public interface IEntity : IChatServiceObject {
	EntityId Id { get; }
}