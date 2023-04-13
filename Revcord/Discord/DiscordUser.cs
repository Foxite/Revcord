using Revcord.Entities;

namespace Revcord.Discord;

public class DiscordUser : IUser {
	private readonly DSharpPlus.Entities.DiscordUser m_Entity;
	
	public ChatClient Client { get; }
	public EntityId Id => EntityId.Of(m_Entity.Id);
	public bool IsSelf => m_Entity.IsCurrent;
	public string DisplayName => m_Entity.Username;
	public string Username => m_Entity.Username;
	public string DiscriminatedUsername => $"{m_Entity.Username}#{m_Entity.Discriminator}";
	public string MentionString => $"<@{m_Entity.Id}>";
	public string AvatarUrl => m_Entity.AvatarUrl;
	public bool IsBot => m_Entity.IsBot;

	public DiscordUser(ChatClient chatClient, DSharpPlus.Entities.DiscordUser entity) {
		m_Entity = entity;
		Client = chatClient;
	}
}
