using Revcord.Entities;

namespace Revcord.Discord;

public class DiscordMember : IGuildMember {
	public DSharpPlus.Entities.DiscordMember Entity { get; }

	public ChatClient Client { get; }
	public IUser User => new DiscordUser(Client, Entity);
	public EntityId UserId => EntityId.Of(Entity.Id);
	public IGuild Guild => new DiscordGuild(Client, Entity.Guild);
	public EntityId GuildId => EntityId.Of(Entity.Guild.Id);
	public string? Nickname => Entity.Nickname;
	
	public DiscordMember(ChatClient chatClient, DSharpPlus.Entities.DiscordMember entity) {
		Entity = entity;
		Client = chatClient;
	}
}