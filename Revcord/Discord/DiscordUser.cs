using Revcord.Entities;

namespace Revcord.Discord;

public class DiscordUser : IUser {
	private readonly DSharpPlus.Entities.DiscordUser m_Entity;
	
	public ChatClient Client { get; }
	public EntityId Id => new EntityId(m_Entity.Id);
	
	public DiscordUser(ChatClient chatClient, DSharpPlus.Entities.DiscordUser entity) {
		m_Entity = entity;
		Client = chatClient;
	}

	public bool IsSelf => m_Entity.IsCurrent;
	public string DisplayName => m_Entity.Username;
	public string Username => m_Entity.Username;
	public string DiscriminatedUsername => $"{m_Entity.Username}#{m_Entity.Discriminator}";
}