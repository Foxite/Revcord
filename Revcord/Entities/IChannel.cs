namespace Revcord.Entities;

public interface IChannel : IEntity {
	string Name { get; }
	string MentionString { get; }
	// todo: permissions
	// todo: type
}
