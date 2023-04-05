using Revcord.Entities;
using DSharpMessage  = DSharpPlus.Entities.DiscordMessage;
using DSharpMember   = DSharpPlus.Entities.DiscordMember;

namespace Revcord.Discord;

public class DiscordMessage : IMessage {
	private readonly DSharpMessage m_Entity;

	public ChatClient Client { get; }
	public EntityId Id => new EntityId(m_Entity.Id);
	public EntityId? GuildId => m_Entity.Channel.GuildId.HasValue ? new EntityId(m_Entity.Channel.GuildId) : null;
	public EntityId ChannelId => new EntityId(m_Entity.ChannelId);
	public EntityId AuthorId => new EntityId(m_Entity.Author.Id);

	public IGuild? Guild => m_Entity.Channel.Guild != null ? new DiscordGuild(Client, m_Entity.Channel.Guild) : null;
	public IChannel Channel => new DiscordChannel(Client, m_Entity.Channel);
	public IUser Author => new DiscordUser(Client, m_Entity.Author);
	public IGuildMember? AuthorMember => m_Entity.Author is DSharpMember member ? new DiscordMember(Client, member) : null;

	public string Content => m_Entity.Content;
	public bool AuthorIsSelf => m_Entity.Author.IsCurrent;
	public DateTimeOffset CreationTimestamp => m_Entity.CreationTimestamp;
	public IReadOnlyCollection<IReaction> Reactions => m_Entity.Reactions.CollectionSelect(reaction => new DiscordReaction(Client, reaction));
	public string JumpLink => m_Entity.JumpLink.ToString();

	public DiscordMessage(ChatClient client, DSharpMessage entity) {
		m_Entity = entity;
		Client = client;
	}
}