using Revcord.Entities;

namespace Revcord.Discord;

public class DiscordReaction : IReaction {
	private readonly DSharpPlus.Entities.DiscordReaction m_Entity;
	
	public ChatClient Client { get; }
	public IEmoji Emoji => new DiscordEmoji(Client, m_Entity.Emoji);
	public int Count => m_Entity.Count;
	
	public DiscordReaction(ChatClient chatClient, DSharpPlus.Entities.DiscordReaction entity) {
		m_Entity = entity;
		Client = chatClient;
	}
}