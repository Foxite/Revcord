namespace Revcord.Entities;

public interface IReaction : IChatServiceObject {
	IEmoji Emoji { get; }
	int Count { get; }
	// todo: user list
}