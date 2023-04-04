namespace Revcord.Entities;

public interface IGuild : IEntity {
	string Name { get; }
	
	IReadOnlyList<IChannelCategory> ChannelCategories { get; }

	// todo: member list
}