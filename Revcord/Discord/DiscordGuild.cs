using Revcord.Entities;

namespace Revcord.Discord;

public class DiscordGuild : IGuild {
	private readonly DSharpPlus.Entities.DiscordGuild m_Entity;
	
	public ChatClient Client { get; }
	public EntityId Id => new EntityId(m_Entity.Id);
	public string Name => m_Entity.Name;
	public IReadOnlyList<IChannelCategory> ChannelCategories => throw new NotImplementedException(); // TODO
	
	public DiscordGuild(ChatClient chatClient, DSharpPlus.Entities.DiscordGuild entity) {
		m_Entity = entity;
		Client = chatClient;
	}
}