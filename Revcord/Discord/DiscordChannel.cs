using Revcord.Entities;

namespace Revcord.Discord;

public class DiscordChannel : IChannel {
	private readonly DSharpPlus.Entities.DiscordChannel m_Entity;
	
	public ChatClient Client { get; }
	public EntityId Id => EntityId.Of(m_Entity.Id);
	public string Name => m_Entity.Name;
	public string MentionString => m_Entity.Mention;

	public DiscordChannel(ChatClient chatClient, DSharpPlus.Entities.DiscordChannel entity) {
		m_Entity = entity;
		Client = chatClient;
	}
}