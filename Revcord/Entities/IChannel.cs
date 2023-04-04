namespace Revcord.Entities;

public interface IChannel : IEntity {
	string Name { get; }
	// todo: permissions
	// todo: type
}
