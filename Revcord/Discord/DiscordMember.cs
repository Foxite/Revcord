using Revcord.Entities;

namespace Revcord.Discord;

public class DiscordMember : IGuildMember {
	private readonly DSharpPlus.Entities.DiscordMember m_Entity;
	
	public ChatClient Client { get; }
	public IUser User => new DiscordUser(Client, m_Entity);
	public EntityId UserId => EntityId.Of(m_Entity.Id);
	public IGuild Guild => new DiscordGuild(Client, m_Entity.Guild);
	public EntityId GuildId => EntityId.Of(m_Entity.Guild.Id);
	public string? Nickname => m_Entity.Nickname;
	
	public DiscordMember(ChatClient chatClient, DSharpPlus.Entities.DiscordMember entity) {
		m_Entity = entity;
		Client = chatClient;
	}
}