using Revcord.Entities;

namespace Revcord.Discord;

public class DiscordUser : IUser {
	public DSharpPlus.Entities.DiscordUser Entity { get; }

	public ChatClient Client { get; }
	public EntityId Id => EntityId.Of(Entity.Id);
	public bool IsSelf => Entity.IsCurrent;
	public string DisplayName => Entity.Username;
	public string Username => Entity.Username;
	public string DiscriminatedUsername => $"{Entity.Username}#{Entity.Discriminator}";
	public string MentionString => $"<@{Entity.Id}>";
	public string AvatarUrl => Entity.AvatarUrl;
	public bool IsBot => Entity.IsBot;

	public DiscordUser(ChatClient chatClient, DSharpPlus.Entities.DiscordUser entity) {
		Entity = entity;
		Client = chatClient;
	}
}
