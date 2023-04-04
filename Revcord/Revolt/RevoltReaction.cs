using Revcord.Entities;

namespace Revcord.Revolt;

public class RevoltReaction : IReaction {
	public RevoltChatClient Client { get; }
	ChatClient IChatServiceObject.Client => Client;
	public IEmoji Emoji { get; }
	public int Count { get; }
	
	public RevoltReaction(RevoltChatClient client, IEmoji emoji, int count) {
		Client = client;
		Emoji = emoji;
		Count = count;
	}
}