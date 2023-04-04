using Revcord.Entities;
using RevoltSharp;

namespace Revcord.Revolt;

public class RevoltMessage : IMessage {
	private readonly Message m_Entity;

	public RevoltChatClient Client { get; }
	ChatClient IChatServiceObject.Client => Client;
	public EntityId Id => new EntityId(m_Entity.Id);
	public EntityId? GuildId => m_Entity.Channel is ServerChannel sc ? new EntityId(sc.ServerId) : null;
	public EntityId ChannelId => new EntityId(m_Entity.ChannelId);
	public EntityId AuthorId => new EntityId(m_Entity.AuthorId);
	public IGuild? Guild => m_Entity.Channel is ServerChannel sc ? new RevoltGuild(Client, sc.Server) : null;
	public IChannel Channel => new RevoltChannel(Client, m_Entity.Channel);
	public IUser Author => new RevoltUser(Client, m_Entity.Author);
	public IGuildMember? AuthorMember => m_Entity.Channel is ServerChannel sc ? new RevoltGuildMember(Client, sc.Server.GetCachedMember(m_Entity.AuthorId)) : null;
	public string? Content => (m_Entity as UserMessage)?.Content;
	public bool AuthorIsSelf => m_Entity.Author.Client.CurrentUser.Id == m_Entity.Id;
	public DateTimeOffset CreationTimestamp => Ulid.Parse(m_Entity.Id).Time;
	public IReadOnlyCollection<IReaction> Reactions => m_Entity is UserMessage um ? um.Reactions.CollectionSelect(kvp => new RevoltReaction(Client, new RevoltEmoji(Client, kvp.Key), kvp.Value.Length)) : Array.Empty<IReaction>();
	
	public RevoltMessage(RevoltChatClient client, Message entity) {
		Client = client;
		m_Entity = entity;
	}
}