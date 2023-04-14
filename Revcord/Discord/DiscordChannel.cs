using Revcord.Entities;

namespace Revcord.Discord;

public class DiscordChannel : IChannel {
	public DSharpPlus.Entities.DiscordChannel Entity { get; }

	public ChatClient Client { get; }
	public EntityId Id => EntityId.Of(Entity.Id);
	public string Name => Entity.Name;
	public string MentionString => Entity.Mention;

	public DiscordChannel(ChatClient chatClient, DSharpPlus.Entities.DiscordChannel entity) {
		Entity = entity;
		Client = chatClient;
	}
}