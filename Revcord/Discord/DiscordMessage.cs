using DSharpPlus;
using Revcord.Entities;
using DSharpMessage  = DSharpPlus.Entities.DiscordMessage;
using DSharpMember   = DSharpPlus.Entities.DiscordMember;

namespace Revcord.Discord;

public class DiscordMessage : IMessage {
	public DSharpMessage Entity { get; }

	public ChatClient Client { get; }
	public EntityId Id => EntityId.Of(Entity.Id);
	public EntityId? GuildId => Entity.Channel.GuildId.HasValue ? EntityId.Of(Entity.Channel.GuildId.Value) : null;
	public EntityId ChannelId => EntityId.Of(Entity.ChannelId);
	public EntityId AuthorId => EntityId.Of(Entity.Author.Id);

	public IGuild? Guild => Entity.Channel.Guild != null ? new DiscordGuild(Client, Entity.Channel.Guild) : null;
	public IChannel Channel => new DiscordChannel(Client, Entity.Channel);
	public IUser Author => new DiscordUser(Client, Entity.Author);
	public IGuildMember? AuthorMember => Entity.Author is DSharpMember member ? new DiscordMember(Client, member) : null;

	public string Content => Entity.Content;
	public bool AuthorIsSelf => Entity.Author.IsCurrent;
	public DateTimeOffset CreationTimestamp => Entity.CreationTimestamp;
	public IReadOnlyCollection<IReaction> Reactions => Entity.Reactions.CollectionSelect(reaction => new DiscordReaction(Client, reaction));
	public string JumpLink => Entity.JumpLink.ToString();
	public bool IsSystemMessage => Entity.MessageType is not (MessageType.Default or MessageType.Reply);

	public DiscordMessage(ChatClient client, DSharpMessage entity) {
		Entity = entity;
		Client = client;
	}
}