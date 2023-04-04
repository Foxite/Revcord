namespace Revcord.Entities;

public interface IChannelCategory : IChatServiceObject {
	string Name { get; }
	IReadOnlyList<IChannel> Channels { get; }
}