using Revcord.Entities;
using RevoltSharp;

namespace Revcord.Revolt;

public class RevoltMessage : IMessage {
	private readonly Message m_Entity;

	public RevoltChatClient Client { get; }
	ChatClient IChatServiceObject.Client => Client;
	public EntityId Id => EntityId.Of(m_Entity.Id);
	public EntityId? GuildId => m_Entity.Channel is ServerChannel sc ? EntityId.Of(sc.ServerId) : null;
	public EntityId ChannelId => EntityId.Of(m_Entity.ChannelId);
	public EntityId AuthorId => EntityId.Of(m_Entity.AuthorId);
	public IGuild? Guild => m_Entity.Channel is ServerChannel sc ? new RevoltGuild(Client, sc.Server) : null;
	public IChannel Channel => new RevoltChannel(Client, m_Entity.Channel);
	public IUser Author => new RevoltUser(Client, m_Entity.Author);
	public IGuildMember? AuthorMember => m_Entity.Channel is ServerChannel sc ? new RevoltGuildMember(Client, sc.Server.GetCachedMember(m_Entity.AuthorId), sc.Server) : null;
	public string? Content => (m_Entity as UserMessage)?.Content;
	public bool AuthorIsSelf => m_Entity.Author.Client.CurrentUser.Id == m_Entity.AuthorId;
	public DateTimeOffset CreationTimestamp => Ulid.Parse(m_Entity.Id).Time;
	public IReadOnlyCollection<IReaction> Reactions => m_Entity is UserMessage um ? um.Reactions.CollectionSelect(kvp => new RevoltReaction(Client, new RevoltEmoji(Client, kvp.Key), kvp.Value.Length)) : Array.Empty<IReaction>();
	
	public string JumpLink => $"{Client.FrontendUrl}/{(m_Entity.Channel is ServerChannel sc ? $"server/{sc.ServerId}/" : "")}channel/{m_Entity.Channel.Id}/{m_Entity.Id}";
	public bool IsSystemMessage => m_Entity is not UserMessage;

	public RevoltMessage(RevoltChatClient client, Message entity) {
		Client = client;
		m_Entity = entity;
	}
}
