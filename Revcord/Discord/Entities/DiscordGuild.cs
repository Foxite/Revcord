using Revcord.Entities;

namespace Revcord.Discord;

public class DiscordGuild : IGuild {
	public DSharpPlus.Entities.DiscordGuild Entity { get; }

	public ChatClient Client { get; }
	public EntityId Id => EntityId.Of(Entity.Id);
	public string Name => Entity.Name;
	public IReadOnlyList<IChannelCategory> ChannelCategories => throw new NotImplementedException(); // TODO
	
	public DiscordGuild(ChatClient chatClient, DSharpPlus.Entities.DiscordGuild entity) {
		Entity = entity;
		Client = chatClient;
	}
}