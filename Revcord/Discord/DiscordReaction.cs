using Revcord.Entities;

namespace Revcord.Discord;

public class DiscordReaction : IReaction {
	public DSharpPlus.Entities.DiscordReaction Entity { get; }

	public ChatClient Client { get; }
	public IEmoji Emoji => new DiscordEmoji(Client, Entity.Emoji);
	public int Count => Entity.Count;
	
	public DiscordReaction(ChatClient chatClient, DSharpPlus.Entities.DiscordReaction entity) {
		Entity = entity;
		Client = chatClient;
	}
}